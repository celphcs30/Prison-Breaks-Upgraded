# Prison Breaks Upgraded

A RimWorld mod that allows you to configure prison breaks based on mood threshold and terror level (Ideology DLC). Toggle mood checking, use terror-based prevention, or combine both approaches.

## Features

### Mood-Based Prevention
- Configure prison breaks based on prisoner mood thresholds (Minor, Major, Extreme)
- Default prevents breaks when prisoners are happy (above Minor threshold)
- Can be toggled on/off independently

### Terror-Based Prevention (Ideology DLC)
- **NEW**: Terror-based prison break prevention for both slaves and prisoners
- Works with vanilla Terror stat for slaves
- **Custom implementation** for prisoners (since vanilla Terror only works for slaves)
- Calculates terror from nearby terror sources (sculptures, skullspikes, gibbet cages, etc.)
- Configurable terror threshold (default: 50%)
- Can be toggled on/off independently

### Flexible Configuration
- Use mood checking only
- Use terror checking only
- Use both checks together (both must pass for prison break to be allowed)
- Disable both to allow all prison breaks (vanilla behavior)

## How It Works

### Mood Checking
When enabled, prisoners must be at or above the selected mood threshold to attempt prison breaks. If their mood is below the threshold, they won't attempt to break out.

### Terror Checking
When enabled, the mod checks the prisoner's terror level:

- **For Slaves**: Uses the vanilla RimWorld Terror stat (works automatically)
- **For Prisoners**: Manually calculates terror from nearby terror sources:
  - Searches within 5 cells radius
  - Finds buildings with `TerrorSource` stat (terror sculptures, skullspikes, gibbet cages, etc.)
  - Uses up to 3 terror sources (matching vanilla behavior)
  - Sums the terror values (capped at 100%)
  - If terror ≥ threshold, prison breaks are prevented

### Combined Checks
When both mood and terror checks are enabled, **both conditions must pass** for a prison break to be allowed:
- Mood must be at/above threshold **AND**
- Terror must be below threshold

## Installation

1. Subscribe on Steam Workshop (when available) or download from GitHub
2. Place in your RimWorld `Mods` folder
3. Enable in RimWorld mod list
4. **Important**: Settings changes require a game restart to take effect

## Requirements

- RimWorld 1.6
- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077) (required dependency)
- Ideology DLC (optional, required for terror-based features)

## Configuration

Access settings via: **Options → Mod Settings → Prison Breaks Upgraded**

### Settings:
- **Use mood threshold**: Enable/disable mood-based checking
- **Mood threshold**: Select Minor, Major, or Extreme
- **Use Terror**: Enable/disable terror-based checking (requires Ideology)
- **Terror threshold**: Set percentage (0-100%, default: 50%)

### Warning
If both mood and terror checks are disabled, all prisoners will be able to attempt prison breaks (vanilla behavior).

## Improvements Over Original

This is a fork of ["Configurable Prison Breaks" by nigel](https://steamcommunity.com/sharedfiles/filedetails/?id=3227325698), with the following improvements:

1. **Terror Support for Prisoners**: Vanilla RimWorld's Terror stat only works for slaves. This mod implements custom terror calculation for prisoners by scanning nearby terror sources.

2. **Independent Toggles**: Both mood and terror checks can be enabled/disabled independently, allowing for flexible configurations.

3. **Performance Optimizations**:
   - Cached settings for faster access
   - Cached Ideology active check
   - Cached Terror StatDef lookups
   - Early exit optimizations

4. **RimWorld 1.6 Only**: Cleaned up and optimized for RimWorld 1.6, removing legacy 1.5 support.

5. **Better Code Organization**: Improved structure and maintainability.

## Technical Details

### Terror Calculation for Prisoners
The mod manually calculates terror for prisoners because RimWorld's `StatWorker_Terror` only applies to slaves. The implementation:
- Scans cells within 5-cell radius of the prisoner
- Identifies buildings with `TerrorSource` stat > 0
- Takes the top 3 terror sources (matching vanilla behavior)
- Sums the values (capped at 100%)

### Performance
- Settings are cached in memory for fast access
- StatDef lookups are cached on initialization
- Early exit if both checks are disabled
- Minimal overhead during prison break checks

## Credits

- **Original Mod**: [Configurable Prison Breaks](https://steamcommunity.com/sharedfiles/filedetails/?id=3227325698) by nigel
- **Fork/Upgrade**: celphcs30
- **License**: CC0 1.0 Universal (Public Domain)

## Links

- **GitHub**: https://github.com/celphcs30/Prison-Breaks-Upgraded
- **Original Mod**: https://steamcommunity.com/sharedfiles/filedetails/?id=3227325698
- **Harmony (Required)**: https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077

## Version History

### v2.0.0
- Added terror-based prison break prevention
- Added independent toggles for mood and terror checks
- Implemented custom terror calculation for prisoners
- Performance optimizations
- RimWorld 1.6 only support
- Updated mod identity and metadata

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests on GitHub.

## License

This work is released into the public domain under CC0 1.0 Universal. See LICENSE file for details.

