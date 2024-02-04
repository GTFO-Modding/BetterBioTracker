using AssetShards;
using BepInEx;
using Gear;
using GTFO.API;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace BetterBioTracker.Comps;
internal sealed class BioScreenRef : MonoBehaviour
{
    public static Material Mat_Invisible;
    public static Material Mat_Thermal;
    public static Material Mat_NonThermal;
    public static Material Mat_RadarImage;
    private static bool s_IsResourceSetupDone = false;

    public Il2CppReferenceField<GUIX_VirtualScene> VirtScene;
    public Il2CppReferenceField<Renderer> RadarImageRenderer;
    public Il2CppReferenceField<Renderer> ScreenRenderer;

    private RadarScreenItem[] _RadarItems;

    private bool _ThermalRenderingEnabled = false;
    void Start()
    {
        _RadarItems = GetComponentsInChildren<RadarScreenItem>(includeInactive: true);
        if (CFG.UseWideRadar)
        {
            RadarImageRenderer.Value.sharedMaterial = Mat_RadarImage;
        }
        
        SetThermalEnabled(false);
        SetRadarGizmoEnabled(true);
    }

    public static void LoadResources()
    {
        if (s_IsResourceSetupDone)
            return;

        var clearTexture = new Texture2D(1, 1, TextureFormat.ARGB4444, false)
        {
            name = "ClearTexture",
            hideFlags = HideFlags.HideAndDontSave
        };
        clearTexture.SetPixel(0, 0, Color.clear);
        clearTexture.Apply();

        var tex2d = new Texture2D(512, 256, TextureFormat.ARGB32, false)
        {
            name = "RadarBGTexture",
            hideFlags = HideFlags.HideAndDontSave
        };
        ImageConversion.LoadImage(tex2d, Resource.Radar);

        Mat_RadarImage = new Material(Shader.Find("Unlit/Transparent"))
        {
            name = "BetterBio - Radar Image Material",
            mainTexture = tex2d,
            hideFlags = HideFlags.HideAndDontSave
        };

        Mat_Invisible = new Material(Shader.Find("Unlit/Transparent"))
        {
            name = "BetterBio - Invisible Material",
            mainTexture = clearTexture,
            hideFlags = HideFlags.HideAndDontSave
        };

        var thermalSight = AssetAPI.GetLoadedAsset<GameObject>("Assets/AssetPrefabs/Items/Gear/Parts/Sights/Sight_19_t.prefab");
        var thermalMat = thermalSight.transform.Find("Sight_19_Thermal").GetComponent<Renderer>().sharedMaterial;
        Mat_Thermal = new Material(thermalMat)
        {
            name = "BetterBio - Thermal Material",
            hideFlags = HideFlags.HideAndDontSave,
            renderQueue = 2000
        };
        Mat_Thermal.SetTexture("_ReticuleA", clearTexture);
        Mat_Thermal.SetTexture("_ReticuleB", clearTexture);
        Mat_Thermal.SetTexture("_ReticuleC", clearTexture);
        Mat_Thermal.SetColor("_ReticuleColorA", ColorExt.Hex("#82f3ff") * 0.25f);
        Mat_Thermal.SetFloat("_OffAngleFade", 0.0f);
        Mat_Thermal.SetFloat("_AmbientTemp", CFG.AmbientHeat);
        Mat_Thermal.SetFloat("_AmbientColorFactor", CFG.AmbientColorFactor);
        Mat_Thermal.SetFloat("_AlbedoColorFactor", CFG.WorldAlbedoFactor);
        Mat_Thermal.SetFloat("_ProjDist1", 0.0f);
        Mat_Thermal.SetFloat("_ProjSize1", 1.0f);
        Mat_Thermal.SetFloat("_RatioAdjust", 0.58f);
        Mat_Thermal.SetFloat("_ScreenIntensity", 0.05f * CFG.Brightness);
        Mat_Thermal.SetFloat("_Zoom", 0.67f);

        var assmPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var palettePath = Path.Combine(assmPath, "palette", $"{CFG.ThermalPaletteName}.png");
        if (File.Exists(palettePath))
        {
            var imageBytes = File.ReadAllBytes(palettePath);
            var paletteTex = new Texture2D(256, 64, TextureFormat.RGBAFloat, false);
            var result = ImageConversion.LoadImage(paletteTex, imageBytes);
            if (result)
            {
                Mat_Thermal.SetTexture("_HeatTex", paletteTex);
            }
            else
            {
                Logger.Warn($"Unable to convert file to texture: {palettePath}");
                Logger.Warn($"This will default color palette to Ironbow style!");
            }
        }
        else
        {
            Logger.Warn($"Unable to find Palette file in path: {palettePath}");
            Logger.Warn($"This will default color palette to Ironbow style!");
        }
        

        var screen = AssetAPI.GetLoadedAsset<GameObject>("Assets/AssetPrefabs/Items/Gear/Parts/Tools/Screen/Screen_2.prefab");
        var screenMat = screen.transform.Find("Screen_Display_2").GetComponent<Renderer>().sharedMaterial;
        Mat_NonThermal = new Material(screenMat)
        {
            name = "BetterBio - Screen Material",
            hideFlags = HideFlags.HideAndDontSave
        };

        s_IsResourceSetupDone = true;
    }

    public void SetRadarGizmoEnabled(bool enabled)
    {
        if (CFG.UseWideRadar)
        {
            RadarImageRenderer.Value.sharedMaterial = enabled ? Mat_RadarImage : Mat_Invisible;
        }

        foreach (var radarItem in _RadarItems)
        {
            radarItem.SetVisible(enabled);
        }
    }

    public void SetThermalEnabled(bool enabled)
    {
        _ThermalRenderingEnabled = enabled;

        if (RadarImageRenderer.Value != null)
        {
            ScreenRenderer.Value.sharedMaterial = enabled ? Mat_Thermal : Mat_NonThermal;
        }

        if (enabled)
        {
            var rt = VirtScene.Value.virtualCamera.target;
            Mat_Thermal.SetTexture("_ReticuleA", rt);
        }
    }
}
