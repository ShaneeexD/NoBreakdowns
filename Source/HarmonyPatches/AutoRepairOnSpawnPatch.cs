using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NoBreakdowns
{
    [HarmonyPatch(typeof(CompBreakdownable), nameof(CompBreakdownable.PostSpawnSetup))]
    public static class AutoRepairOnSpawnPatch
    {
        static void Postfix(CompBreakdownable __instance)
        {
            var parent = __instance?.parent;
            if (parent == null)
                return;

            var power = parent.GetComp<CompPowerTrader>();
            if (power == null)
                return;

            var def = parent.def;
            var cost = def?.costList;
            if (cost == null || cost.Count == 0)
                return;

            bool usesComponents = cost.Any(c => c.thingDef == ThingDefOf.ComponentIndustrial || c.thingDef == ThingDefOf.ComponentSpacer);
            if (!usesComponents)
                return;

            if (__instance.BrokenDown)
            {
                __instance.Notify_Repaired();
            }
        }
    }
}
