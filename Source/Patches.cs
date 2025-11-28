using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace PrisonBreaks
{
    public class Patches
    {
        private static volatile PrisonBreaksSettings cachedSettings;
        
        // Cached values for performance optimization
        private static bool ideologyActive;
        private static StatDef cachedTerrorStat;
        private static StatDef cachedTerrorSourceStat;

        public static void SetSettings(PrisonBreaksSettings settings)
        {
            cachedSettings = settings;
        }

        public static void InitializeCachedValues()
        {
            // Cache IdeologyActive check
            ideologyActive = ModsConfig.IdeologyActive;
            
            // Cache Terror StatDef if Ideology is active
            if (ideologyActive)
            {
                cachedTerrorStat = DefDatabase<StatDef>.GetNamedSilentFail("Terror");
                cachedTerrorSourceStat = DefDatabase<StatDef>.GetNamedSilentFail("TerrorSource");
            }
        }

        /// <summary>
        /// Manually calculates terror from nearby terror sources for prisoners.
        /// Vanilla Terror stat only works for slaves, so we need to calculate it ourselves for prisoners.
        /// Based on RimWorld mechanics: terror sources affect pawns within ~5 cells, max 3 sources.
        /// </summary>
        private static float CalculateTerrorFromSources(Pawn pawn)
        {
            if (pawn?.Map == null || cachedTerrorSourceStat == null)
                return 0f;

            const int terrorRadius = 5; // ~5 cells radius
            const int maxTerrorSources = 3; // Max terror sources that can affect a pawn

            var pawnPos = pawn.Position;
            var terrorSources = new List<float>();
            var checkedBuildings = new HashSet<Building>(); // Avoid checking same building multiple times

            // Find all buildings within terror radius by checking cells
            foreach (var cell in GenRadial.RadialCellsAround(pawnPos, terrorRadius, true))
            {
                if (!cell.InBounds(pawn.Map))
                    continue;

                var building = cell.GetFirstBuilding(pawn.Map);
                if (building != null && building.Spawned && !checkedBuildings.Contains(building))
                {
                    checkedBuildings.Add(building);
                    
                    // Check if building has TerrorSource stat
                    float terrorSourceValue = building.GetStatValue(cachedTerrorSourceStat);
                    if (terrorSourceValue > 0f)
                    {
                        terrorSources.Add(terrorSourceValue);
                    }
                }
            }

            // Sort descending and take top 3 (max sources)
            terrorSources.Sort((a, b) => b.CompareTo(a));
            terrorSources = terrorSources.Take(maxTerrorSources).ToList();

            // Sum up terror from sources (capped at 100%)
            float totalTerror = terrorSources.Sum();
            return totalTerror > 100f ? 100f : totalTerror;
        }

        public static bool CanParticipateInPrisonBreakPrefixPatch(ref bool __result, Pawn pawn)
        {
            // Null check - safety first
            if (pawn == null)
            {
                return true; // Let vanilla handle null pawns
            }

            if (cachedSettings == null)
            {
                // Fallback if settings not initialized yet
                return true;
            }

            // Early exit optimization: if both checks are disabled, allow prison break immediately
            if (!cachedSettings.useMood && (!ideologyActive || !cachedSettings.useTerror))
            {
                return true;
            }

            // Check mood threshold if enabled
            if (cachedSettings.useMood)
            {
                if ((int)MoodThresholdExtensions.CurrentMoodThresholdFor(pawn) < cachedSettings.val)
                {
                    __result = false;
                    return false;
                }
            }

            // Check Terror if Ideology is active and terror checking is enabled
            if (ideologyActive && cachedSettings.useTerror)
            {
                float terrorValue = 0f;

                // Try to get Terror stat (works for slaves)
                if (cachedTerrorStat != null)
                {
                    terrorValue = pawn.GetStatValue(cachedTerrorStat);
                }

                // If Terror stat is 0, manually calculate from nearby sources (for prisoners)
                // This handles the case where Terror stat only works for slaves
                if (terrorValue <= 0f && cachedTerrorSourceStat != null)
                {
                    terrorValue = CalculateTerrorFromSources(pawn);
                }

                if (terrorValue >= cachedSettings.terrorThreshold)
                {
                    // Terror is high enough, prevent prison break
                    __result = false;
                    return false;
                }
            }

            return true;
        }
    }
}
