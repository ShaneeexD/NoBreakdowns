using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ColonyChatter
{
    // Custom mote class that displays a speech bubble with text inside it
    public class Mote_SpeechBubbleWithText : ThingWithComps
    {
        // The background texture for the speech bubble
        private static readonly Material bubbleMat = MaterialPool.MatFrom("Things/Mote/Speech", ShaderDatabase.Transparent);
        
        // Store the text to display inside the bubble
        public string textContent;
        
        // Store the type of bubble (enthusiastic, chat, joke)
        public BubbleType bubbleType = BubbleType.Chat;
        
        // Bubble types
        public enum BubbleType
        {
            Enthusiastic,
            Chat,
            Joke
        }
        
        // Properties for mote behavior
        public float solidTime = 2f;
        public float fadeOutTime = 0.5f;
        public float fadeInTime = 0.15f;
        public Vector3 velocity = Vector3.zero;
        public float Scale = 1f;
        private float spawnedTime;
        
        // Text rendering properties
        private Material textMaterial;
        private float textWidth;
        private Vector2 textSize;
        
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            spawnedTime = Time.time;
        }
        
        public float AgeSecs
        {
            get
            {
                return Time.time - spawnedTime;
            }
        }
        
        public new Vector3 DrawPos
        {
            get
            {
                Vector3 result = Position.ToVector3Shifted();
                result.y = Altitudes.AltitudeFor(AltitudeLayer.MoteOverhead);
                result += velocity * AgeSecs;
                return result;
            }
        }

        // Draw the bubble and text
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            // Don't draw if not visible
            if (!Find.CameraDriver.CurrentViewRect.Contains(Position))
            {
                return;
            }

            // Calculate the position and scale
            Vector3 drawPos = DrawPos;
            
            // Calculate the alpha based on the age of the mote
            float alpha = 1f;
            float totalLifespan = fadeInTime + solidTime + fadeOutTime;
            
            if (AgeSecs < fadeInTime)
            {
                alpha = AgeSecs / fadeInTime;
            }
            else if (AgeSecs > fadeInTime + solidTime)
            {
                alpha = 1f - ((AgeSecs - fadeInTime - solidTime) / fadeOutTime);
            }
            
            // Clamp alpha between 0 and 1
            alpha = Mathf.Clamp01(alpha);
            
            // If fully faded out, destroy the mote
            if (AgeSecs > totalLifespan)
            {
                Destroy();
                return;
            }
            
            // Set the color based on bubble type
            Color color = Color.white;
            switch (bubbleType)
            {
                case BubbleType.Enthusiastic:
                    color = new Color(1f, 0.7f, 0.7f, alpha); // Light red/pink
                    break;
                case BubbleType.Chat:
                    color = new Color(1f, 1f, 1f, alpha); // White
                    break;
                case BubbleType.Joke:
                    color = new Color(0.7f, 1f, 0.7f, alpha); // Light green
                    break;
            }
            
            // Draw the speech bubble background
            float bubbleScale = Scale;
            if (!textContent.NullOrEmpty())
            {
                // Scale the bubble based on text length
                bubbleScale = Mathf.Max(1f, Mathf.Min(textContent.Length / 10f, 2f)) * Scale;
            }
            
            // Draw the bubble
            Matrix4x4 matrix = Matrix4x4.TRS(drawPos, Quaternion.identity, new Vector3(bubbleScale, 1f, bubbleScale));
            Graphics.DrawMesh(MeshPool.plane10, matrix, bubbleMat, 0);

            // Instead of drawing text directly, create a separate floating text mote
            // This avoids GUI calls in the Draw method
            if (!textContent.NullOrEmpty() && AgeSecs < 0.1f) // Only spawn once at the beginning
            {
                // Use RimWorld's built-in floating text system
                MoteMaker.ThrowText(drawPos + new Vector3(0f, 0.5f, 0f), Map, textContent, 2f);
            }
        }
        
        protected override void Tick()
        {
            base.Tick();
            
            // Check if the mote should be destroyed
            if (AgeSecs > fadeInTime + solidTime + fadeOutTime)
            {
                Destroy();
            }
        }
    }
}
