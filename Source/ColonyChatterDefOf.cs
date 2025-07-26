using RimWorld;
using System;
using Verse;

namespace ColonyChatter
{
    [DefOf]
    public static class ColonyChatterDefOf
    {
        public static TraitDef ColonyChatter;
        
        // Thought for chatty colonists based on whether they're near others
        public static ThoughtDef ChattyNearOthers;

        static ColonyChatterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ColonyChatterDefOf));
        }
    }
}
