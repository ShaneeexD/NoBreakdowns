using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ColonyChatter
{
    [StaticConstructorOnStartup]
    public static class ChattyInteractionInitiator
    {
        static ChattyInteractionInitiator()
        {
            // We don't need to do anything here - the GameComponent will be automatically loaded
            // by RimWorld when a game is loaded or started
        }
    }

    // Game component to handle checking for interactions between chatty pawns
    public class ChattyInteractionGameComponent : GameComponent
    {
        private int ticksUntilNextCheck = 0;
        private const int CheckInterval = 2500; // Check every 2500 ticks (about 41.6 seconds)
        
        public ChattyInteractionGameComponent()
        {
        }
        
        public ChattyInteractionGameComponent(Game game)
        {
        }
        
        public override void GameComponentTick()
        {
            base.GameComponentTick();
            
            // Count down until next check
            ticksUntilNextCheck--;
            
            if (ticksUntilNextCheck <= 0)
            {
                // Reset timer
                ticksUntilNextCheck = CheckInterval;
                
                // Check for potential interactions
                TryInitiateChattyInteractions();
            }
        }
        
        private void TryInitiateChattyInteractions()
        {
            // Get all pawns in the player faction that are chatty
            List<Pawn> chattyPawns = GetChattyColonists();
            
            if (chattyPawns.Count < 2)
                return; // Need at least 2 chatty pawns for an interaction
                
            // For each chatty pawn, try to find another chatty pawn nearby to interact with
            foreach (Pawn initiator in chattyPawns)
            {
                // Skip if pawn is busy, drafted, etc.
                if (!CanInitiateInteraction(initiator))
                    continue;
                    
                // Find nearby chatty pawns
                List<Pawn> nearbyChattyPawns = chattyPawns
                    .Where(p => p != initiator && p.Position.DistanceTo(initiator.Position) <= 10f && CanReceiveInteraction(p))
                    .ToList();
                    
                if (nearbyChattyPawns.Count == 0)
                    continue;
                    
                // Select a random nearby chatty pawn
                Pawn recipient = nearbyChattyPawns.RandomElement();
                
                // 30% chance to trigger the interaction
                if (Rand.Chance(0.3f))
                {
                    // Get our custom interaction def
                    InteractionDef interactionDef = DefDatabase<InteractionDef>.GetNamed("EnthusiasticChat");
                    
                    if (interactionDef != null)
                    {
                        // Initiate the interaction
                        initiator.interactions.TryInteractWith(recipient, interactionDef);
                        
                        // Only do one interaction per check to avoid spamming
                        break;
                    }
                }
            }
        }

        private List<Pawn> GetChattyColonists()
        {
            List<Pawn> result = new List<Pawn>();
            
            // Get all free colonists from all maps
            foreach (Pawn pawn in PawnsFinder.AllMaps_FreeColonistsSpawned)
            {
                if (pawn.story?.traits?.HasTrait(TraitDef.Named("ColonyChatter")) ?? false)
                {
                    result.Add(pawn);
                }
            }
            
            return result;
        }
        
        private bool CanInitiateInteraction(Pawn pawn)
        {
            return pawn.Awake() && 
                !pawn.Drafted && 
                !pawn.InMentalState && 
                pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) &&
                !pawn.jobs.curDriver.asleep;
        }
        
        private bool CanReceiveInteraction(Pawn pawn)
        {
            return pawn.Awake() && 
                !pawn.Drafted && 
                !pawn.InMentalState && 
                pawn.health.capacities.CapableOf(PawnCapacityDefOf.Hearing) &&
                !pawn.jobs.curDriver.asleep;
        }
    }
}
