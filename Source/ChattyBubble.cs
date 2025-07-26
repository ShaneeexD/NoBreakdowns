using System;
using UnityEngine;
using Verse;

namespace ColonyChatter
{
    /// <summary>
    /// Represents a speech bubble with text that can be drawn above a pawn
    /// </summary>
    public class ChattyBubble
    {
        // The pawn this bubble is attached to
        private readonly Pawn pawn;
        
        // The text content of the bubble
        private readonly string text;
        
        // The type of bubble
        private readonly BubbleType bubbleType;
        
        // When the bubble was created
        private readonly float creationTime;
        
        // How long the bubble should last
        private readonly float duration;
        
        // Cached text for performance
        private string cachedText;
        
        // Cached dimensions for performance
        private int width;
        private int height;
        
        // Bubble types
        public enum BubbleType
        {
            Enthusiastic,
            Chat,
            Joke
        }
        
        /// <summary>
        /// Creates a new ChattyBubble
        /// </summary>
        /// <param name="pawn">The pawn this bubble is attached to</param>
        /// <param name="text">The text content of the bubble</param>
        /// <param name="type">The type of bubble</param>
        /// <param name="duration">How long the bubble should last in seconds</param>
        public ChattyBubble(Pawn pawn, string text, BubbleType type, float duration = 4f)
        {
            this.pawn = pawn;
            this.text = text;
            this.bubbleType = type;
            this.duration = duration;
            this.creationTime = Time.time;
            
            // Calculate dimensions
            CalculateDimensions();
        }
        
        /// <summary>
        /// Calculates the dimensions of the bubble based on the text content
        /// </summary>
        private void CalculateDimensions()
        {
            // Get the text to display
            cachedText = GetText();
            
            // Create a GUIContent with the text
            GUIContent content = new GUIContent(cachedText);
            
            // Calculate the size using the current font style
            Vector2 size = Text.CurFontStyle.CalcSize(content);
            
            // Set the dimensions
            width = Mathf.RoundToInt(Mathf.Min(size.x, 200f)); // Max width of 200
            height = Mathf.RoundToInt(Text.CurFontStyle.CalcHeight(content, width));
        }
        
        /// <summary>
        /// Gets the text to display in the bubble
        /// </summary>
        private string GetText()
        {
            return text;
        }
        
        /// <summary>
        /// Calculates the alpha value for fading the bubble in and out
        /// </summary>
        private float GetAlpha()
        {
            float elapsed = Time.time - creationTime;
            
            // Fade in
            float fadeInTime = 0.15f;
            if (elapsed < fadeInTime)
            {
                return elapsed / fadeInTime;
            }
            
            // Fade out
            float fadeOutStart = duration - 0.5f;
            if (elapsed > fadeOutStart)
            {
                return 1f - ((elapsed - fadeOutStart) / 0.5f);
            }
            
            // Full opacity during the middle of the duration
            return 1f;
        }
        
        /// <summary>
        /// Draws the bubble atlas (9-slice) at the specified position
        /// </summary>
        private void DrawBubbleAtlas(Rect rect, Texture2D atlas, Color color)
        {
            // Adjust coordinates to avoid texture bleeding and handle UI scaling
            rect.xMin = Mathf.Floor(rect.xMin);
            rect.yMin = Mathf.Floor(rect.yMin);
            rect.xMax = Mathf.Ceil(rect.xMax);
            rect.yMax = Mathf.Ceil(rect.yMax);
            
            // Calculate scale for corners (25% of the atlas width/height)
            int scale = Mathf.RoundToInt(Mathf.Min(atlas.width * 0.25f, rect.height * 0.25f, rect.width * 0.25f));
            
            // Save the current GUI color
            Color prevColor = GUI.color;
            GUI.color = color;
            
            // Begin group to contain the drawing
            GUI.BeginGroup(rect);
            
            // Draw the corners using Widgets.DrawTexturePart
            Widgets.DrawTexturePart(new Rect(0, 0, scale, scale), new Rect(0, 0, 0.25f, 0.25f), atlas);
            Widgets.DrawTexturePart(new Rect(rect.width - scale, 0, scale, scale), new Rect(0.75f, 0, 0.25f, 0.25f), atlas);
            Widgets.DrawTexturePart(new Rect(0, rect.height - scale, scale, scale), new Rect(0, 0.75f, 0.25f, 0.25f), atlas);
            Widgets.DrawTexturePart(new Rect(rect.width - scale, rect.height - scale, scale, scale), new Rect(0.75f, 0.75f, 0.25f, 0.25f), atlas);
            
            // Draw the edges
            Widgets.DrawTexturePart(new Rect(scale, 0, rect.width - (scale * 2f), scale), new Rect(0.25f, 0, 0.5f, 0.25f), atlas);
            Widgets.DrawTexturePart(new Rect(scale, rect.height - scale, rect.width - (scale * 2f), scale), new Rect(0.25f, 0.75f, 0.5f, 0.25f), atlas);
            Widgets.DrawTexturePart(new Rect(0, scale, scale, rect.height - (scale * 2f)), new Rect(0, 0.25f, 0.25f, 0.5f), atlas);
            Widgets.DrawTexturePart(new Rect(rect.width - scale, scale, scale, rect.height - (scale * 2f)), new Rect(0.75f, 0.25f, 0.25f, 0.5f), atlas);
            
            // Draw the center
            Widgets.DrawTexturePart(new Rect(scale, scale, rect.width - (scale * 2f), rect.height - (scale * 2f)), new Rect(0.25f, 0.25f, 0.5f, 0.5f), atlas);
            
            // End the group
            GUI.EndGroup();
            
            // Restore the previous GUI color
            GUI.color = prevColor;
        }
        
        /// <summary>
        /// Draws the bubble at the specified position
        /// </summary>
        /// <param name="pos">The position to draw the bubble at</param>
        /// <returns>True if the bubble is still active, false if it has expired</returns>
        public bool Draw(Vector2 pos)
        {
            // Check if the bubble has expired
            if (Time.time > creationTime + duration)
            {
                return false;
            }
            
            // Calculate alpha for fading
            float alpha = GetAlpha();
            if (alpha <= 0f)
            {
                return false;
            }
            
            // Calculate colors based on bubble type
            Color backgroundColor = Color.white;
            Color foregroundColor = Color.black;
            
            switch (bubbleType)
            {
                case BubbleType.Enthusiastic:
                    backgroundColor = new Color(1f, 0.7f, 0.7f, alpha); // Light red/pink
                    break;
                case BubbleType.Chat:
                    backgroundColor = new Color(1f, 1f, 1f, alpha); // White
                    break;
                case BubbleType.Joke:
                    backgroundColor = new Color(0.7f, 1f, 0.7f, alpha); // Light green
                    break;
            }
            
            // Calculate padding
            int paddingX = 10;
            int paddingY = 5;
            
            // Create the bubble rectangle
            Rect rect = new Rect(
                pos.x - (width / 2f),
                pos.y - height,
                width + (paddingX * 2),
                height + (paddingY * 2)
            );
            
            // Draw the bubble background and outline
            DrawBubbleAtlas(rect, ChattyBubbleTextures.Inner, backgroundColor);
            DrawBubbleAtlas(rect, ChattyBubbleTextures.Outer, foregroundColor.WithAlpha(alpha));
            
            // Draw the text
            Rect textRect = new Rect(
                rect.x + paddingX,
                rect.y + paddingY,
                rect.width - (paddingX * 2),
                rect.height - (paddingY * 2)
            );
            // Save the current color
            Color oldColor = GUI.color;
            
            // Create a custom style for centered text
            var style = new GUIStyle(Text.CurFontStyle);
            // Use Text.CurTextFieldStyle for alignment instead of TextAnchor
            style.alignment = Text.CurTextFieldStyle.alignment;
            style.normal.textColor = foregroundColor.WithAlpha(alpha);
            
            // Draw the text using GUI.Label with our custom style
            GUI.color = foregroundColor.WithAlpha(alpha);
            GUI.Label(textRect, cachedText, style);
            
            // Restore the previous color
            GUI.color = oldColor;
            
            return true;
        }
        
        /// <summary>
        /// Checks if the bubble is still active
        /// </summary>
        public bool IsActive => Time.time <= creationTime + duration;
        
        /// <summary>
        /// Checks if the bubble has expired
        /// </summary>
        public bool Expired => Time.time > creationTime + duration;
        
        /// <summary>
        /// Updates the bubble state
        /// </summary>
        public void Update()
        {
            // Nothing to update for now, but this method is required by ChattyBubbleManager
        }
        
        /// <summary>
        /// Draws the bubble at the current pawn position
        /// </summary>
        public void Draw()
        {
            if (pawn == null || pawn.Map == null || pawn.Dead || !pawn.Spawned)
            {
                return;
            }
            
            // Calculate position above the pawn
            Vector3 drawPos = pawn.DrawPos;
            drawPos.z += 0.5f;
            
            // Convert to screen position
            Vector2 screenPos = Find.Camera.WorldToScreenPoint(drawPos);
            // Adjust for screen height (invert Y coordinate)
            screenPos.y = Screen.height - screenPos.y;
            
            // Draw the bubble
            Draw(screenPos);
        }
    }
}
