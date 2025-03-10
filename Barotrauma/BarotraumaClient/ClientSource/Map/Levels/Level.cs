﻿using Barotrauma.Networking;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Barotrauma
{
    partial class Level
    {
        private LevelRenderer renderer;

        private BackgroundCreatureManager backgroundCreatureManager;

        public BackgroundCreatureManager BackgroundCreatureManager => backgroundCreatureManager;

        public LevelRenderer Renderer => renderer;

        public void ReloadTextures()
        {
            renderer.ReloadTextures();

            HashSet<Texture2D> uniqueTextures = new HashSet<Texture2D>();
            HashSet<Sprite> uniqueSprites = new HashSet<Sprite>();
            var allLevelObjects = LevelObjectManager.GetAllObjects();
            foreach (var levelObj in allLevelObjects)
            {
                foreach (Sprite sprite in levelObj.Prefab.Sprites)
                {
                    if (!uniqueTextures.Contains(sprite.Texture))
                    {
                        uniqueTextures.Add(sprite.Texture);
                        uniqueSprites.Add(sprite);
                    }
                }
            }

            foreach (Sprite sprite in uniqueSprites)
            {
                sprite.ReloadTexture();
            }
        }
        
        public void DrawDebugOverlay(SpriteBatch spriteBatch, Camera cam)
        {
            if (renderer == null) { return; }
            renderer.DrawDebugOverlay(spriteBatch, cam);

            if (GameMain.DebugDraw)
            {
                if (Screen.Selected.Cam.Zoom > 0.1f)
                {
                    foreach (InterestingPosition pos in PositionsOfInterest)
                    {
                        Color color = Color.Yellow;
                        if (pos.PositionType == PositionType.Cave || pos.PositionType == PositionType.AbyssCave)
                        {
                            color = Color.DarkOrange;
                        }
                        else if (pos.PositionType == PositionType.Ruin)
                        {
                            color = Color.LightGray;
                        }
                        if (!pos.IsValid)
                        {
                            color = Color.Red;
                        }
                    
                        GUI.DrawRectangle(spriteBatch, new Vector2(pos.Position.X - 15.0f, -pos.Position.Y - 15.0f), new Vector2(30.0f, 30.0f), color, true);
                    }

                    foreach (RuinGeneration.Ruin ruin in Ruins)
                    {
                        Rectangle ruinArea = ruin.Area;
                        ruinArea.Y = -ruinArea.Y - ruinArea.Height;

                        GUI.DrawRectangle(spriteBatch, ruinArea, Color.DarkSlateBlue, false, 0, 5);
                    }
                }
                
                float zoomFactor = MathHelper.Lerp(20, 1, MathUtils.InverseLerp(Screen.Selected.Cam.MinZoom, Screen.Selected.Cam.DefaultZoom, Screen.Selected.Cam.Zoom));
                foreach ((string debugInfo, List<Vector2> positions) in positionHistory)
                {
                    for (int i = 0; i < positions.Count; i++)
                    {
                        float t = (i + 1) / (float)positions.Count;
                        float multiplier = MathHelper.Lerp(0.1f, 1, t);
                        Color color = Color.Red * multiplier;
                        var pos = positions[i];
                        pos.Y = -pos.Y;
                        var size = new Vector2(200);
                        if (i == 0)
                        {
                            GUI.DrawRectangle(spriteBatch, pos - size, size * 2, Color.Red, thickness: 2 * zoomFactor);
                            GUI.DrawString(spriteBatch, pos - new Vector2(10, 20), debugInfo, Color.White, font: GUIStyle.LargeFont, forceUpperCase: ForceUpperCase.Yes);
                        }
                        if (i < positions.Count - 1)
                        {
                            if (i > 0)
                            {
                                GUI.DrawRectangle(spriteBatch, pos - size / 2, size, Color.Red, isFilled: true);   
                            }
                            var nextPos = positions[i + 1];
                            nextPos.Y = -nextPos.Y;
                            GUI.DrawLine(spriteBatch, pos, nextPos, color, width: 4 * zoomFactor);
                        }
                    }
                }

                foreach ((Submarine sub, List<Rectangle> rects) in blockedRects)
                {
                    foreach (Rectangle t in rects)
                    {
                        Rectangle newRect = t;
                        newRect.Y = -newRect.Y;
                        GUI.DrawRectangle(spriteBatch, newRect, Color.Red * 0.1f, isFilled: true);
                        GUI.DrawString(spriteBatch, newRect.Center.ToVector2(), $"{sub.Info.Name}", Color.White, font: GUIStyle.LargeFont, forceUpperCase: ForceUpperCase.Yes);
                    }
                }
            }
        }

        public void DrawBack(GraphicsDevice graphics, SpriteBatch spriteBatch, Camera cam)
        {
            float brightness = MathHelper.Clamp(1.1f + (cam.Position.Y - Size.Y) / 100000.0f, 0.1f, 1.0f);
            var lightColorHLS = GenerationParams.AmbientLightColor.RgbToHLS();
            lightColorHLS.Y *= brightness;

            GameMain.LightManager.AmbientLight = ToolBox.HLSToRGB(lightColorHLS);

            graphics.Clear(BackgroundColor);

            if (renderer != null)
            {
                GameMain.LightManager.AmbientLight = GameMain.LightManager.AmbientLight.Add(renderer.FlashColor);
                renderer?.DrawBackground(spriteBatch, cam, LevelObjectManager, backgroundCreatureManager);
            }
        }

        public void DrawFront(SpriteBatch spriteBatch, Camera cam)
        {
            renderer?.DrawForeground(spriteBatch, cam, LevelObjectManager);
        }
        public void ClientEventRead(IReadMessage msg, float sendingTime)
        {
            EventType eventType = (EventType)msg.ReadByte();
            
            switch (eventType)
            {
                case EventType.GlobalDestructibleWall:
                {
                    foreach (LevelWall levelWall in ExtraWalls)
                    {
                        if (levelWall.Body.BodyType == BodyType.Static) { continue; }

                        Vector2 bodyPos = new Vector2(
                            msg.ReadSingle(),
                            msg.ReadSingle());
                        levelWall.MoveState = msg.ReadRangedSingle(0.0f, MathHelper.TwoPi, 16);
                        if (Vector2.DistanceSquared(bodyPos, levelWall.Body.Position) > 0.5f
                            && !(levelWall is DestructibleLevelWall { Destroyed: true }))
                        {
                            levelWall.Body.SetTransformIgnoreContacts(ref bodyPos, levelWall.Body.Rotation);
                        }
                    }
                }
                break;
                case EventType.SingleDestructibleWall:
                {
                    int index = msg.ReadUInt16();
                    float damageByte = msg.ReadByte();
                    if (index < ExtraWalls.Count && ExtraWalls[index] is DestructibleLevelWall destructibleWall)
                    {
                        destructibleWall.SetDamage(destructibleWall.MaxHealth * damageByte / 255.0f);
                    }
                }
                break;
                default:
                    throw new Exception($"Malformed incoming level event: {eventType} is not a supported event type");
            }
        }
    }
}
