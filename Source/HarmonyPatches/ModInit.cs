using HarmonyLib;
using Verse;

namespace NoBreakdowns
{
    [StaticConstructorOnStartup]
    public static class ModInit
    {
        static ModInit()
        {
            var harmony = new Harmony("shaneeexd.nobreakdowns");
            harmony.PatchAll();
        }
    }
}
