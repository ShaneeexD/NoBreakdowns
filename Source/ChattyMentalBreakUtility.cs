using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace ColonyChatter
{
    /// <summary>
    /// Utility class for handling mental break modifications for chatty colonists
    /// </summary>
    [StaticConstructorOnStartup]
    public static class ChattyMentalBreakUtility
    {
        // The distance to check for nearby colonists
        private const float NearbyColonistRadius = 10f;
        
        // Mental break chance multiplier when a chatty colonist is around others
        private const float AroundOthersMultiplier = 0.7f; // 30% reduction in mental break chance
        
        // Mental break chance multiplier when a chatty colonist is alone
        private const float AloneMultiplier = 1.3f; // 30% increase in mental break chance
        
        // Grace period in ticks before a state change takes effect (30 seconds = 1800 ticks)
        private const int StateChangeGracePeriod = 1800;
        
        // Cache to avoid checking for nearby colonists too frequently
        private static readonly Dictionary<int, CachedNearbyCheck> nearbyColonistsCache = new Dictionary<int, CachedNearbyCheck>();
        
        // How long to cache the nearby colonists check (in ticks)
        private const int CacheDuration = 500; // About 8.3 seconds
        
        // Structure to cache nearby colonist checks and track state changes
        private class CachedNearbyCheck
        {
            public int lastCheckTick;
            public bool hasNearbyColonists;
            public bool effectiveHasNearbyColonists; // The actual state used for mental break calculations after grace period
            public int stateChangeTick; // When the current state started
        }
        
        /// <summary>
        /// Applies the mental break chance multiplier for a chatty colonist
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <param name="chance">The original mental break chance</param>
        /// <returns>The modified mental break chance</returns>
        public static float GetModifiedMentalBreakChance(Pawn pawn, float chance)
        {
            // Only apply to chatty colonists
            if (!HasChattyTrait(pawn))
            {
                return chance;
            }
            
            // Check if the pawn has nearby colonists (with grace period applied)
            bool effectiveHasNearbyColonists = HasNearbyColonists(pawn, true);
            
            // Apply the appropriate multiplier
            if (effectiveHasNearbyColonists)
            {
                return chance * AroundOthersMultiplier;
            }
            else
            {
                return chance * AloneMultiplier;
            }
        }
        
        /// <summary>
        /// Checks if a pawn has the chatty trait
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <returns>True if the pawn has the chatty trait, false otherwise</returns>
        private static bool HasChattyTrait(Pawn pawn)
        {
            if (pawn?.story?.traits == null)
                return false;
            
            return pawn.story.traits.HasTrait(ColonyChatterDefOf.ColonyChatter);
        }
        
        /// <summary>
        /// Checks if a pawn has nearby colonists
        /// </summary>
        /// <param name="pawn">The pawn to check</param>
        /// <param name="useEffectiveState">Whether to use the effective state (after grace period)</param>
        /// <returns>True if the pawn has nearby colonists, false otherwise</returns>
        private static bool HasNearbyColonists(Pawn pawn, bool useEffectiveState = false)
        {
            if (pawn == null || pawn.Map == null || pawn.Dead || !pawn.Spawned)
            {
                return false;
            }
            
            // Check cache first
            int currentTick = Find.TickManager.TicksGame;
            if (nearbyColonistsCache.TryGetValue(pawn.thingIDNumber, out CachedNearbyCheck cachedCheck))
            {
                if (currentTick - cachedCheck.lastCheckTick < CacheDuration)
                {
                    return useEffectiveState ? cachedCheck.effectiveHasNearbyColonists : cachedCheck.hasNearbyColonists;
                }
            }
            
            // Find nearby colonists
            bool hasNearby = false;
            foreach (Pawn otherPawn in pawn.Map.mapPawns.AllPawnsSpawned)
            {
                if (otherPawn != pawn && otherPawn.RaceProps.Humanlike && 
                    otherPawn.Faction == pawn.Faction &&
                    otherPawn.Position.InHorDistOf(pawn.Position, NearbyColonistRadius))
                {
                    hasNearby = true;
                    break;
                }
            }
            
            // Check if we need to create a new cache entry or update an existing one
            if (!nearbyColonistsCache.TryGetValue(pawn.thingIDNumber, out CachedNearbyCheck existingCache))
            {
                // First time checking this pawn, create new entry
                nearbyColonistsCache[pawn.thingIDNumber] = new CachedNearbyCheck
                {
                    lastCheckTick = currentTick,
                    hasNearbyColonists = hasNearby,
                    effectiveHasNearbyColonists = hasNearby, // Initially the same
                    stateChangeTick = currentTick
                };
            }
            else
            {
                // Update existing entry
                existingCache.lastCheckTick = currentTick;
                
                // If the state has changed, record when it changed
                if (existingCache.hasNearbyColonists != hasNearby)
                {
                    existingCache.hasNearbyColonists = hasNearby;
                    existingCache.stateChangeTick = currentTick;
                }
                
                // Check if we need to update the effective state (after grace period)
                if (existingCache.effectiveHasNearbyColonists != hasNearby && 
                    currentTick - existingCache.stateChangeTick >= StateChangeGracePeriod)
                {
                    existingCache.effectiveHasNearbyColonists = hasNearby;
                }
                
                // Keep the cache entry
                nearbyColonistsCache[pawn.thingIDNumber] = existingCache;
            }
            
            // Return the appropriate state
            return useEffectiveState ? 
                nearbyColonistsCache[pawn.thingIDNumber].effectiveHasNearbyColonists : 
                hasNearby;
        }
    }
}
