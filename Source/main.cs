using HarmonyLib;
using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;


namespace PrisonBreaks
{
    public class PrisonBreaksSettings : ModSettings
    {
        //Mood threshold settings
        public bool useMood = true; // Enable/disable mood-based checking
        public int val; // Mood threshold value

        //Terror settings (Ideology only)
        public bool useTerror = false;
        public int terrorThreshold = 50; // 0-100, if terror >= this value, prevent prison break

        public string getlevel()
        {
            switch ((MoodThreshold)val)
            {
                case MoodThreshold.Extreme:
                    return "Extreme";
                case MoodThreshold.Minor:
                    return "Minor";
                case MoodThreshold.Major:
                    return "Major";
                default:
                    return "Never";
            }
        }

        //save settings
        public override void ExposeData()
        {
            Scribe_Values.Look(ref useMood, "useMood", true);
            Scribe_Values.Look(ref val, "val", 1);
            Scribe_Values.Look(ref useTerror, "useTerror", false);
            Scribe_Values.Look(ref terrorThreshold, "terrorThreshold", 50);
            base.ExposeData();
        }
    }

    public class PrisonBreaks : Mod
    {
        PrisonBreaksSettings settings;

        public PrisonBreaks(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("celphcs30.prisonbreaksupgraded");
            HarmonyPatcher.instance = harmony;
            LongEventHandler.QueueLongEvent(Init, "PrisonBreaksUpgraded.Init", true, null);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            
            // Mood threshold settings
            listingStandard.CheckboxLabeled("PrisonBreaksUpgraded_UseMoodLabel".Translate(), ref settings.useMood, "PrisonBreaksUpgraded_UseMoodTooltip".Translate());
            
            if (settings.useMood)
            {
                listingStandard.Label($"{"PrisonBreaksUpgraded_ThresholdLabel".Translate()}: " + settings.getlevel());
                listingStandard.IntSetter(ref settings.val, (int)MoodThreshold.Extreme + 1, "PrisonBreaksUpgraded_NeverButton".Translate());
                listingStandard.IntSetter(ref settings.val, (int)MoodThreshold.Minor, "PrisonBreaksUpgraded_MinorButton".Translate());
                listingStandard.IntSetter(ref settings.val, (int)MoodThreshold.Major, "PrisonBreaksUpgraded_MajorButton".Translate());
                listingStandard.IntSetter(ref settings.val, (int)MoodThreshold.Extreme, "PrisonBreaksUpgraded_ExtremeButton".Translate());
            }
            
            listingStandard.Gap();
            
            // Terror settings (only show if Ideology is active)
            if (ModsConfig.IdeologyActive)
            {
                listingStandard.CheckboxLabeled("PrisonBreaksUpgraded_UseTerrorLabel".Translate(), ref settings.useTerror, "PrisonBreaksUpgraded_UseTerrorTooltip".Translate());
                
                if (settings.useTerror)
                {
                    listingStandard.Label($"{"PrisonBreaksUpgraded_TerrorThresholdLabel".Translate()}: {settings.terrorThreshold}%");
                    settings.terrorThreshold = (int)listingStandard.Slider(settings.terrorThreshold, 0f, 100f);
                    listingStandard.Label("PrisonBreaksUpgraded_TerrorThresholdTooltip".Translate());
                }
            }
            
            // Warning if both are disabled
            if (!settings.useMood && (!ModsConfig.IdeologyActive || !settings.useTerror))
            {
                listingStandard.Gap();
                GUI.color = Color.red;
                listingStandard.Label("PrisonBreaksUpgraded_NoChecksWarning".Translate());
                GUI.color = Color.white;
            }
            
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Prison Breaks Upgraded";
        }

        public void Init()
        {
            this.settings = GetSettings<PrisonBreaksSettings>();
            Patches.SetSettings(this.settings);
            HarmonyPatcher.PatchVanillaMethods();
        }
    }
}