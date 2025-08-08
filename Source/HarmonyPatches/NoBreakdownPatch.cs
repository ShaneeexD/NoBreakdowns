using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NoBreakdowns
{
    [HarmonyPatch(typeof(CompBreakdownable))]
    [HarmonyPatch("DoBreakdown")]
    public static class NoBreakdownPatch
    {
        // Skip breakdown if: powered machinery AND was built with components (industrial or spacer)
        static bool Prefix(CompBreakdownable __instance)
        {
            var parent = __instance?.parent;
            if (parent == null)
                return true; // run original just in case

            // Must be a powered building
            var power = parent.GetComp<CompPowerTrader>();
            if (power == null)
                return true;

            // Check if its build cost includes vanilla components
            var def = parent.def;
            var cost = def?.costList;
            if (cost == null || cost.Count == 0)
                return true;

            bool usesComponents = cost.Any(c => c.thingDef == ThingDefOf.ComponentIndustrial || c.thingDef == ThingDefOf.ComponentSpacer);
            if (!usesComponents)
                return true;

            // Prevent breakdown
            return false;
        }
    }
}
