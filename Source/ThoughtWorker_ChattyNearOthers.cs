using RimWorld;
using System;
using Verse;

namespace ColonyChatter
{
    /// <summary>
    /// ThoughtWorker that gives chatty colonists a mood buff when they're near other colonists
    /// </summary>
    public class ThoughtWorker_ChattyNearOthers : ThoughtWorker
    {
        // The distance to check for nearby colonists
        private const float NearbyColonistRadius = 10f;
        
        protected override ThoughtState CurrentStateInternal(Pawn pawn)
        {
            // Only apply to chatty colonists
            if (!HasChattyTrait(pawn))
            {
                return ThoughtState.Inactive;
            }
            
            // Check if the pawn has nearby colonists
            bool hasNearbyColonists = HasNearbyColonists(pawn);
            
            // Return the appropriate thought stage
            if (hasNearbyColonists)
            {
                return ThoughtState.ActiveAtStage(0); // Near others - positive mood
            }
            else
            {
                return ThoughtState.ActiveAtStage(1); // Alone - negative mood
            }
        }
        
        /// <summary>
        /// Checks if a pawn has the chatty trait
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the pawn has the chatty trait, false otherwise</returns>
        private bool HasChattyTrait(Pawn pawn)
        {
            if (pawn?.story?.traits == null)
                return false;
            
            return pawn.story.traits.HasTrait(ColonyChatterDefOf.ColonyChatter);
        }
        
        /// <summary>
        /// Checks if a pawn has nearby colonists
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the pawn has nearby colonists, false otherwise</returns>
        private bool HasNearbyColonists(Pawn pawn)
        {
            if (pawn == null || pawn.Map == null || pawn.Dead || !pawn.Spawned)
            {
                return false;
            }
            
            // Find nearby colonists
            foreach (Pawn otherPawn in pawn.Map.mapPawns.AllPawnsSpawned)
            {
                if (otherPawn != pawn && otherPawn.RaceProps.Humanlike && 
                    otherPawn.Faction == pawn.Faction &&
                    otherPawn.Position.InHorDistOf(pawn.Position, NearbyColonistRadius))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
