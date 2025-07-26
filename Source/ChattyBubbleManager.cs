using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ColonyChatter
{
    [StaticConstructorOnStartup]
    public static class ChattyBubbleManager
    {
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

        // Show an enthusiastic chat bubble above a pawn
        public static void ShowEnthusiasticChatBubble(Pawn pawn)
        {
            if (pawn?.Map == null || !pawn.Spawned)
                return;

            // Just throw text above the pawn
            MoteMaker.ThrowText(pawn.DrawPos + new Vector3(0f, 0f, 0.5f), pawn.Map, "!!", 3.5f);
            
            // Play an enthusiastic sound
            SoundDefOf.Tick_High.PlayOneShot(new TargetInfo(pawn));
        }

        // Show a random chat snippet bubble above a pawn
        public static void ShowChatSnippetBubble(Pawn pawn)
        {
            if (pawn?.Map == null || !pawn.Spawned)
                return;

            // Get a random chat snippet
            string chatText = ChatSnippets.RandomElement();
            
            // Show floating text for the actual content
            MoteMaker.ThrowText(pawn.DrawPos + new Vector3(0f, 0f, 0.5f), pawn.Map, chatText, 3.5f);
            
            // Play a chat sound
            SoundDefOf.Tick_Low.PlayOneShot(new TargetInfo(pawn));
        }

        // Show a joke bubble above a pawn
        public static void ShowJokeBubble(Pawn pawn)
        {
            if (pawn?.Map == null || !pawn.Spawned)
                return;

            // Get a random joke
            string jokeText = Jokes.RandomElement();
            
            // Show floating text for the actual joke
            MoteMaker.ThrowText(pawn.DrawPos + new Vector3(0f, 0f, 0.5f), pawn.Map, jokeText, 4f);
            
            // Play a joke sound
            SoundDefOf.Tick_Tiny.PlayOneShot(new TargetInfo(pawn));
        }
    }
}
