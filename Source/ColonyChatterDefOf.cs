using RimWorld;
using System;
using Verse;

namespace ColonyChatter
{
    [DefOf]
    public static class ColonyChatterDefOf
    {
        public static TraitDef ColonyChatter;

        static ColonyChatterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ColonyChatterDefOf));
        }
    }
}
