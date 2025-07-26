using System;
using RimWorld;
using Verse;

namespace ColonyChatter
{
    public class ColonyChatterMod : Mod
    {
        public ColonyChatterMod(ModContentPack content) : base(content)
        {
            // No need for Harmony patching since we're using the <icon> tag in XML
            Log.Message("[Colony Chatter] Mod initialized successfully!");
        }
    }
    
    // No need for Harmony patching or custom components since we're using XML for trait icon
}
