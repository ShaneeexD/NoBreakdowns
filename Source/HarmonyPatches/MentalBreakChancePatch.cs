using HarmonyLib;
using RimWorld;
using System;
using System.Reflection;
using Verse;
using Verse.AI;

namespace ColonyChatter
{
    /// <summary>
    /// Harmony patches for modifying mental break chances for chatty colonists
    /// </summary>
    [StaticConstructorOnStartup]
    public static class MentalBreakChancePatch
    {
        static MentalBreakChancePatch()
        {
            // Log that we're initializing the mental break patches
            Log.Message("[Colony Chatter] Initializing mental break chance patches");
            
            try
            {
                // Find the method to patch - targeting MentalBreaker.BreakThresholdExtreme property getter
                MethodInfo targetMethod = AccessTools.PropertyGetter(typeof(MentalBreaker), "BreakThresholdExtreme");
                if (targetMethod == null)
                {
                    Log.Error("[Colony Chatter] Could not find MentalBreaker.BreakThresholdExtreme property to patch!");
                    return;
                }
                
                // Create our patch
                MethodInfo postfixMethod = AccessTools.Method(typeof(MentalBreakChancePatch), nameof(MentalBreakThresholdExtreme_Postfix));
                
                // Apply the patch
                ColonyChatterMod.HarmonyInstance.Patch(targetMethod, null, new HarmonyMethod(postfixMethod));
                
                // Also patch the major and minor thresholds
                MethodInfo targetMethodMajor = AccessTools.PropertyGetter(typeof(MentalBreaker), "BreakThresholdMajor");
                if (targetMethodMajor != null)
                {
                    ColonyChatterMod.HarmonyInstance.Patch(targetMethodMajor, null, 
                        new HarmonyMethod(AccessTools.Method(typeof(MentalBreakChancePatch), nameof(MentalBreakThresholdMajor_Postfix))));
                }
                else
                {
                    Log.Warning("[Colony Chatter] Could not find MentalBreaker.BreakThresholdMajor property to patch");
                }
                
                MethodInfo targetMethodMinor = AccessTools.PropertyGetter(typeof(MentalBreaker), "BreakThresholdMinor");
                if (targetMethodMinor != null)
                {
                    ColonyChatterMod.HarmonyInstance.Patch(targetMethodMinor, null, 
                        new HarmonyMethod(AccessTools.Method(typeof(MentalBreakChancePatch), nameof(MentalBreakThresholdMinor_Postfix))));
                }
                else
                {
                    Log.Warning("[Colony Chatter] Could not find MentalBreaker.BreakThresholdMinor property to patch");
                }
                
                Log.Message("[Colony Chatter] Mental break chance patches applied successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"[Colony Chatter] Error applying mental break chance patches: {ex}");
            }
        }
        
        /// <summary>
        /// Postfix for extreme mental break threshold
        /// </summary>
        public static void MentalBreakThresholdExtreme_Postfix(MentalBreaker __instance, ref float __result)
        {
            // Get the pawn field via reflection since it's private
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null)
            {
                __result = ChattyMentalBreakUtility.GetModifiedMentalBreakChance(pawn, __result);
            }
        }
        
        /// <summary>
        /// Postfix for major mental break threshold
        /// </summary>
        public static void MentalBreakThresholdMajor_Postfix(MentalBreaker __instance, ref float __result)
        {
            // Get the pawn field via reflection since it's private
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null)
            {
                __result = ChattyMentalBreakUtility.GetModifiedMentalBreakChance(pawn, __result);
            }
        }
        
        /// <summary>
        /// Postfix for minor mental break threshold
        /// </summary>
        public static void MentalBreakThresholdMinor_Postfix(MentalBreaker __instance, ref float __result)
        {
            // Get the pawn field via reflection since it's private
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn != null)
            {
                __result = ChattyMentalBreakUtility.GetModifiedMentalBreakChance(pawn, __result);
            }
        }
    }
}
