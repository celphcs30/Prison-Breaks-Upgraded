using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace PrisonBreaks
{
    public class HarmonyPatcher
    {
        public static Harmony instance;
        public static void PatchVanillaMethods()
        {
            // Initialize cached values for performance optimization
            Patches.InitializeCachedValues();
            
            // Patch PrisonBreakUtility.CanParticipateInPrisonBreak method
            MethodInfo canParticipateInPrisonBreakMethod = AccessTools.Method(typeof(PrisonBreakUtility), "CanParticipateInPrisonBreak");
            HarmonyMethod canParticipateInPrisonBreakPrefixPatch = new HarmonyMethod(typeof(Patches).GetMethod("CanParticipateInPrisonBreakPrefixPatch"));
            instance.Patch(canParticipateInPrisonBreakMethod, canParticipateInPrisonBreakPrefixPatch);
        }
    }
}
