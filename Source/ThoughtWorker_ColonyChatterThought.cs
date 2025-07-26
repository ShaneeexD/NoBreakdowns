using RimWorld;
using Verse;

namespace ColonyChatter
{
    public class ThoughtWorker_ColonyChatterThought : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            if (!p.Spawned || p.Map == null) return false;

            foreach (Pawn other in p.Map.mapPawns.SpawnedPawnsInFaction(p.Faction))
            {
                if (other != p && !other.Dead && p.Position.DistanceTo(other.Position) < 5f)
                {
                    return ThoughtState.ActiveDefault;
                }
            }

            return ThoughtState.Inactive;
        }
        
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            if (!p.Spawned || p.Map == null || !otherPawn.Spawned || otherPawn.Map == null) return false;
            
            // Check if the pawns are close to each other
            if (p.Position.DistanceTo(otherPawn.Position) < 5f)
            {
                return ThoughtState.ActiveDefault;
            }
            
            return ThoughtState.Inactive;
        }
    }
}