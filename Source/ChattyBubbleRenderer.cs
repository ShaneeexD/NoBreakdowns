using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ColonyChatter
{
    /// <summary>
    /// Handles rendering text for speech bubbles in the OnGUI event
    /// </summary>
    public static class ChattyBubbleRenderer
    {
        // Dictionary to track active text bubbles
        private static readonly Dictionary<int, BubbleTextData> ActiveBubbles = new Dictionary<int, BubbleTextData>();
        
        // Class to store bubble text data
        public class BubbleTextData
        {
            public string Text;
            public Vector3 Position;
            public float CreationTime;
            public float Duration;
            public Color TextColor;
            public BubbleType Type;
            
            public BubbleTextData(string text, Vector3 position, float duration, Color textColor, BubbleType type)
            {
                Text = text;
                Position = position;
                CreationTime = Time.time;
                Duration = duration;
                TextColor = textColor;
                Type = type;
            }
            
            public bool IsExpired => Time.time > CreationTime + Duration;
            
            public float Alpha
            {
                get
                {
                    float age = Time.time - CreationTime;
                    float fadeInTime = 0.15f;
                    float fadeOutTime = 0.5f;
                    
                    if (age < fadeInTime)
                    {
                        return age / fadeInTime;
                    }
                    else if (age > Duration - fadeOutTime)
                    {
                        return 1f - ((age - (Duration - fadeOutTime)) / fadeOutTime);
                    }
                    
                    return 1f;
                }
            }
        }
        
        // Bubble types
        public enum BubbleType
        {
            Enthusiastic,
            Chat,
            Joke
        }
        
        // Add a new bubble to be rendered
        public static void AddBubble(string text, Vector3 position, float duration, BubbleType type)
        {
            Color textColor = Color.white;
            switch (type)
            {
                case BubbleType.Enthusiastic:
                    textColor = new Color(1f, 0.7f, 0.7f); // Light red/pink
                    break;
                case BubbleType.Chat:
                    textColor = Color.white;
                    break;
                case BubbleType.Joke:
                    textColor = new Color(0.7f, 1f, 0.7f); // Light green
                    break;
            }
            
            int id = GetNextId();
            ActiveBubbles[id] = new BubbleTextData(text, position, duration, textColor, type);
        }
        
        // Get the next available ID
        private static int GetNextId()
        {
            int id = 1;
            while (ActiveBubbles.ContainsKey(id))
            {
                id++;
            }
            return id;
        }
        
        // Clean up expired bubbles
        private static void CleanupExpiredBubbles()
        {
            List<int> toRemove = new List<int>();
            
            foreach (var kvp in ActiveBubbles)
            {
                if (kvp.Value.IsExpired)
                {
                    toRemove.Add(kvp.Key);
                }
            }
            
            foreach (int id in toRemove)
            {
                ActiveBubbles.Remove(id);
            }
        }
        
        // Draw all active bubbles - called during OnGUI
        public static void DrawBubbles()
        {
            CleanupExpiredBubbles();
            
            foreach (var kvp in ActiveBubbles)
            {
                BubbleTextData data = kvp.Value;
                
                // Skip if not visible on screen
                if (!Find.CameraDriver.CurrentViewRect.Contains(data.Position.ToIntVec3()))
                {
                    continue;
                }
                
                // Convert world position to screen position
                Vector3 screenPos = Find.Camera.WorldToScreenPoint(data.Position);
                
                // Adjust for UI scaling
                screenPos.y = Screen.height - screenPos.y;
                
                // Create style for text
                GUIStyle style = new GUIStyle(Text.CurFontStyle);
                style.normal.textColor = new Color(data.TextColor.r, data.TextColor.g, data.TextColor.b, data.Alpha);
                style.alignment = TextAnchor.MiddleCenter;
                
                // Calculate text size
                Vector2 textSize = style.CalcSize(new GUIContent(data.Text));
                
                // Add padding for bubble
                float bubblePadding = 12f;
                float bubbleWidth = textSize.x + bubblePadding * 2;
                float bubbleHeight = textSize.y + bubblePadding * 2;
                
                // Calculate bubble position
                float bubbleX = screenPos.x - bubbleWidth / 2f;
                float bubbleY = screenPos.y - bubbleHeight - 10f;
                
                // Draw bubble background with custom textures
                Color bubbleColor = new Color(1f, 1f, 1f, data.Alpha);
                
                // Draw outer bubble
                GUI.color = bubbleColor;
                GUI.DrawTexture(new Rect(bubbleX, bubbleY, bubbleWidth, bubbleHeight), ChattyBubbleTextures.Outer);
                
                // Draw inner bubble
                GUI.DrawTexture(new Rect(bubbleX + 2f, bubbleY + 2f, bubbleWidth - 4f, bubbleHeight - 4f), ChattyBubbleTextures.Inner);
                
                // Draw text
                GUI.color = new Color(data.TextColor.r, data.TextColor.g, data.TextColor.b, data.Alpha);
                GUI.Label(new Rect(bubbleX, bubbleY, bubbleWidth, bubbleHeight), data.Text, style);
                
                // Reset color
                GUI.color = Color.white;
            }
        }
    }
}
