using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NoBreakdowns
{
    // Disable random battery short-circuit ("Zzztt") incident.
    // Note: Rain-caused short circuits are handled by IncidentWorker_ShortCircuitRain and remain unaffected.
    [HarmonyPatch]
    public static class Patch_DisableShortCircuit
    {
        static MethodBase TargetMethod()
        {
            var t = AccessTools.TypeByName("RimWorld.IncidentWorker_ShortCircuit");
            return AccessTools.Method(t, "TryExecuteWorker", new Type[] { typeof(IncidentParms) });
        }

        static bool Prefix(ref bool __result)
        {
            __result = false;
            return false; // skip original to prevent the incident
        }
    }

    // Disable solar flare incidents entirely
    [HarmonyPatch]
    public static class Patch_DisableSolarFlare
    {
        static MethodBase TargetMethod()
        {
            var t = AccessTools.TypeByName("RimWorld.IncidentWorker_SolarFlare");
            return AccessTools.Method(t, "TryExecuteWorker", new Type[] { typeof(IncidentParms) });
        }

        static bool Prefix(ref bool __result)
        {
            __result = false;
            return false; // skip original to prevent the incident
        }
    }
}
