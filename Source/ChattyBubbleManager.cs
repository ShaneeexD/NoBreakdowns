using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ColonyChatter
{
    /// <summary>
    /// Manages the creation and display of speech bubbles for chatty colonists
    /// </summary>
    [StaticConstructorOnStartup]
    public static class ChattyBubbleManager
    {
        // Dictionary to track when pawns last had a bubble
        private static readonly Dictionary<int, float> lastBubbleTime = new Dictionary<int, float>();
        
        // List of active bubbles
        private static readonly List<ChattyBubble> activeBubbles = new List<ChattyBubble>();
        
        // Minimum time between bubbles for the same pawn (in seconds)
        private const float MinTimeBetweenBubbles = 10f;
        
        // List of possible chat snippets for random bubbles
        private static readonly List<string> ChatSnippets = new List<string>
        {
            "Did you hear about...",
            "So I was thinking...",
            "You won't believe this!",
            "Let me tell you...",
            "The other day I...",
            "Isn't it amazing?",
            "What do you think about...",
            "I've been wondering...",
            "Have you ever noticed...",
            "Between you and me..."
        };

        // List of jokes for joke bubbles
        private static readonly List<string> Jokes = new List<string>
        {
            "Why did the colonist bring a ladder to the bar? They heard the drinks were on the house!",
            "What do you call a lazy colonist? A bed-rester!",
            "Why don't raiders use bathrooms? They prefer to be in-continent!",
            "How many colonists does it take to change a light bulb? None, they prefer the darkness mood buff!",
            "What's a colonist's favorite exercise? The mental break!",
            "Why was the mechanoid feeling down? It had a chip on its shoulder!"
        };
        
        // Textures for speech bubbles
        public static readonly Texture2D BubbleInner;
        public static readonly Texture2D BubbleOuter;
        public static readonly Texture2D BubbleIcon;
        
        // Static constructor to load textures
        static ChattyBubbleManager()
        {
            // Load textures from the mod's Textures folder
            BubbleInner = ContentFinder<Texture2D>.Get("Bubbles/Inner");
            BubbleOuter = ContentFinder<Texture2D>.Get("Bubbles/Outer");
            BubbleIcon = ContentFinder<Texture2D>.Get("Bubbles/Icon");
        }
        
        /// <summary>
        /// Shows an enthusiastic bubble above a pawn
        /// </summary>
        /// <param name="pawn">The pawn to show the bubble above</param>
        public static void ShowEnthusiasticBubble(Pawn pawn)
        {
            if (pawn == null || !CanShowBubble(pawn))
            {
                return;
            }
            
            try
            {
                // Play a sound
                SoundDefOf.Tick_High.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
                
                // Create a new bubble
                ChattyBubble bubble = new ChattyBubble(pawn, "So excited to chat!", ChattyBubble.BubbleType.Enthusiastic);
                activeBubbles.Add(bubble);
                
                // Update the last bubble time
                RecordBubbleTime(pawn);
            }
            catch (Exception ex)
            {
                Log.Error($"[Colony Chatter] Error showing enthusiastic bubble: {ex}");
            }
        }
        
        /// <summary>
        /// Shows a chat snippet bubble above a pawn
        /// </summary>
        /// <param name="pawn">The pawn to show the bubble above</param>
        /// <param name="text">The text to show in the bubble</param>
        public static void ShowChatBubble(Pawn pawn, string text = null)
        {
            if (pawn == null || !CanShowBubble(pawn))
            {
                return;
            }
            
            try
            {
                // Play a sound
                SoundDefOf.Tick_Low.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
                
                // Use random chat snippet if no text provided
                string bubbleText = text;
                if (bubbleText.NullOrEmpty())
                {
                    bubbleText = ChatSnippets.RandomElement();
                }
                
                // Create a new bubble
                ChattyBubble bubble = new ChattyBubble(pawn, bubbleText, ChattyBubble.BubbleType.Chat);
                activeBubbles.Add(bubble);
                
                // Update the last bubble time
                RecordBubbleTime(pawn);
            }
            catch (Exception ex)
            {
                Log.Error($"[Colony Chatter] Error showing chat bubble: {ex}");
            }
        }
        
        /// <summary>
        /// Shows a joke bubble above a pawn
        /// </summary>
        /// <param name="pawn">The pawn to show the bubble above</param>
        public static void ShowJokeBubble(Pawn pawn)
        {
            if (pawn == null || !CanShowBubble(pawn))
            {
                return;
            }
            
            try
            {
                // Play a sound
                SoundDefOf.Tick_Tiny.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
                
                // Get a random joke
                string joke = Jokes.RandomElement();
                
                // Create a new bubble
                ChattyBubble bubble = new ChattyBubble(pawn, joke, ChattyBubble.BubbleType.Joke);
                activeBubbles.Add(bubble);
                
                // Update the last bubble time
                RecordBubbleTime(pawn);
            }
            catch (Exception ex)
            {
                Log.Error($"[Colony Chatter] Error showing joke bubble: {ex}");
            }
        }
        
        /// <summary>
        /// Updates all active bubbles and removes expired ones
        /// </summary>
        public static void Update()
        {
            // Update all active bubbles
            for (int i = activeBubbles.Count - 1; i >= 0; i--)
            {
                ChattyBubble bubble = activeBubbles[i];
                bubble.Update();
                
                // Remove expired bubbles
                if (bubble.Expired)
                {
                    activeBubbles.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Draws all active bubbles
        /// </summary>
        public static void DrawBubbles()
        {
            // Draw all active bubbles
            foreach (ChattyBubble bubble in activeBubbles)
            {
                bubble.Draw();
            }
        }
        
        /// <summary>
        /// Checks if a pawn can show a bubble
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the pawn can show a bubble, false otherwise</returns>
        private static bool CanShowBubble(Pawn pawn)
        {
            // Check if the pawn is valid
            if (pawn == null || pawn.Map == null || pawn.Dead || !pawn.Spawned)
            {
                return false;
            }
            
            // Check if the pawn has had a bubble recently
            if (lastBubbleTime.TryGetValue(pawn.thingIDNumber, out float lastTime))
            {
                if (Time.realtimeSinceStartup - lastTime < MinTimeBetweenBubbles)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Records the time a pawn had a bubble
        /// </summary>
        /// <param name="pawn">The pawn that had a bubble</param>
        private static void RecordBubbleTime(Pawn pawn)
        {
            if (pawn != null)
            {
                lastBubbleTime[pawn.thingIDNumber] = Time.realtimeSinceStartup;
            }
        }
        
        /// <summary>
        /// Gets a random chat snippet
        /// </summary>
        /// <returns>A random chat snippet</returns>
        public static string GetRandomChatSnippet()
        {
            return ChatSnippets.RandomElement();
        }
        
        /// <summary>
        /// Gets a random joke
        /// </summary>
        /// <returns>A random joke</returns>
        public static string GetRandomJoke()
        {
            return Jokes.RandomElement();
        }
        
        /// <summary>
        /// Shows a chat snippet bubble above a pawn with random text
        /// </summary>
        /// <param name="pawn">The pawn to show the bubble above</param>
        public static void ShowChatSnippetBubble(Pawn pawn)
        {
            string chatText = GetRandomChatSnippet();
            ShowChatBubble(pawn, chatText);
        }
        
        /// <summary>
        /// Shows an enthusiastic chat bubble above a pawn
        /// </summary>
        /// <param name="pawn">The pawn to show the bubble above</param>
        public static void ShowEnthusiasticChatBubble(Pawn pawn)
        {
            ShowEnthusiasticBubble(pawn);
        }
    }
}
