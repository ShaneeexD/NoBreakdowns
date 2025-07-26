using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace ColonyChatter
{
    public class ColonyChatterMod : Mod
    {
        /// <summary>
        /// Harmony instance for the mod
        /// </summary>
        public static Harmony HarmonyInstance { get; private set; }
        
        public ColonyChatterMod(ModContentPack content) : base(content)
        {
            try
            {
                // Initialize Harmony
                HarmonyInstance = new Harmony("com.shaneexd.colonychatter");
                
                // Log that the mod is initializing
                Log.Message("[Colony Chatter] Initializing mod with Harmony patches...");
                
                // Apply patches
                HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                
                // Log that the mod has been initialized
                Log.Message("[Colony Chatter] Mod initialized successfully with Harmony patches!");
            }
            catch (Exception ex)
            {
                Log.Error($"[Colony Chatter] Error initializing mod: {ex}");
            }
        }
    }
}
