using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TenCC.Utils.ResilientTask;

namespace BetterBioTracker;
public static class CFG
{
    public static bool UseWideRadar { get; private set; }
    public static bool UseColoredTargets { get; private set; }
    public static bool AddPlayersToTracker { get; private set; }
    public static bool UseThermalRadarMode { get; private set; }
    public static bool UseThermalOnlyMode { get; private set; }
    public static bool UseOldZoom { get; private set; }
    public static float TargetSizeMult { get; private set; }

    public static float WorldAlbedoFactor { get; private set; }
    public static float AmbientHeat { get; private set; }
    public static float AmbientColorFactor { get; private set; }
    public static float Brightness { get; private set; }
    public static string ThermalPaletteName { get; private set; }

    public const string SEC_FEAT = "1. Features";
    public const string SEC_MODES = "2. Screen Modes";
    public const string SEC_THERMAL = "3. Thermal Vision";
    public const string SEC_MISC = "4. Miscellaneous";
    

    internal static void BindAll(ConfigFile cfg)
    {
        UseWideRadar = cfg.Bind(SEC_FEAT, "Use Wide Radar", true, "Remove aesthetic components from Bio-tracker screen and widen the radar?").Value;
        UseColoredTargets = cfg.Bind(SEC_FEAT, "Use Colored Targets", true, "Dynamically change Enemy Targets color in Bio-Tracker screen?").Value;
        AddPlayersToTracker = cfg.Bind(SEC_FEAT, "Add Players to Tracker", true, "Add Players to Bio-tracker radar?").Value;
        UseThermalRadarMode = cfg.Bind(SEC_MODES, "Use Thermal-Radar Mode", true, "Add Thermal-Radar Mode to your mode cycle?").Value;
        UseThermalOnlyMode = cfg.Bind(SEC_MODES, "Use Thermal-Only Mode", true, "Use Thermal-Only Mode to your mode cycle?").Value;
        

        ThermalPaletteName = cfg.Bind(SEC_THERMAL, "Thermal Screen Palette Name", "Ironbow", "Filename of Thermal Screen Color Palette (Under the plugins/BetterBioTracker/palette folder, Only Accept PNG file)").Value;
        WorldAlbedoFactor = cfg.Bind(SEC_THERMAL, "Albedo Factor", 0.1f).Value;
        AmbientHeat = cfg.Bind(SEC_THERMAL, "Ambient Heat", 0.02f).Value;
        AmbientColorFactor = cfg.Bind(SEC_THERMAL, "Ambient Color Factor", 5.0f).Value;
        Brightness = cfg.Bind(SEC_THERMAL, "Screen Brightness", 0.5f).Value;

        UseOldZoom = cfg.Bind(SEC_MISC, "Use Pre R6 Zoom Animation", false, "Use Old Zooming Animation?").Value;
        TargetSizeMult = cfg.Bind(SEC_MISC, "Target Size Multiplier", 1.0f, "Multiplier for Targets size in Bio-tracker screen").Value;
    }
}
