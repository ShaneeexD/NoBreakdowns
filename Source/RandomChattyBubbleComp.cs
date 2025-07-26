using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ColonyChatter
{
    public class RandomChattyBubbleComp : GameComponent
    {
        private int ticksUntilNextBubble = 1000;
        private const int MinTicksBetweenBubbles = 2000; // ~33 seconds
        private const int MaxTicksBetweenBubbles = 5000; // ~83 seconds
        private const float BubbleChance = 0.4f;

        public RandomChattyBubbleComp(Game game) : base() { }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            // Only process every 250 ticks (about 4 seconds) to save performance
            if (Find.TickManager.TicksGame % 250 != 0)
                return;

            ticksUntilNextBubble -= 250;

            if (ticksUntilNextBubble <= 0)
            {
                // Reset timer
                ticksUntilNextBubble = Rand.Range(MinTicksBetweenBubbles, MaxTicksBetweenBubbles);
                
                // Try to show random bubbles
                TryShowRandomBubbles();
            }
        }

        private void TryShowRandomBubbles()
        {
            // Get all chatty colonists
            List<Pawn> chattyColonists = new List<Pawn>();
            
            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
            {
                if (pawn.story?.traits?.HasTrait(ColonyChatterDefOf.ColonyChatter) == true)
                {
                    chattyColonists.Add(pawn);
                }
            }

            if (chattyColonists.Count == 0)
                return;

            // For each chatty colonist, there's a chance they'll show a bubble
            foreach (Pawn chattyPawn in chattyColonists)
            {
                if (Rand.Chance(BubbleChance))
                {
                    // Check if the pawn is in a social gathering or near others
                    bool nearOthers = false;
                    
                    foreach (Pawn otherPawn in chattyPawn.Map.mapPawns.AllPawnsSpawned)
                    {
                        if (otherPawn != chattyPawn && otherPawn.RaceProps.Humanlike && 
                            otherPawn.Position.InHorDistOf(chattyPawn.Position, 5f))
                        {
                            nearOthers = true;
                            break;
                        }
                    }

                    // Choose which bubble to show based on whether they're near others
                    if (nearOthers)
                    {
                        // Near others - show chat or joke
                        float rand = Rand.Value;
                        if (rand < 0.6f)
                            ChattyBubbleManager.ShowChatSnippetBubble(chattyPawn);
                        else
                            ChattyBubbleManager.ShowJokeBubble(chattyPawn);
                    }
                    else
                    {
                        // Alone - just show a thought bubble occasionally
                        if (Rand.Chance(0.3f))
                            ChattyBubbleManager.ShowChatSnippetBubble(chattyPawn);
                    }
                }
            }
        }
    }
}
