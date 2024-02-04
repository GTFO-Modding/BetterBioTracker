using BetterBioTracker.Comps;
using Gear;
using GTFO.API;
using HarmonyLib;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterBioTracker;
[HarmonyPatch]
internal static class Inject_BioSpawn
{
    static bool s_ScreenPrefabProcessed = false;

    [HarmonyPatch(typeof(GearPartSpawner), nameof(GearPartSpawner.OnShardsLoaded))]
    [HarmonyPrefix]
    static void Pre_ShardsLoaded()
    {
        if (s_ScreenPrefabProcessed)
            return;

        var bioScreen = AssetAPI.GetLoadedAsset<GameObject>("Assets/AssetPrefabs/Items/Gear/Parts/Tools/Screen/Screen_2.prefab");
        if (bioScreen == null)
            return;

        var vScene = bioScreen.GetComponentInChildren<GUIX_VirtualScene>();
        vScene.useDepth = true;
        var guixRoot = vScene.transform.Find("GUIX_layer/GUIX_Display_EnemyScanner_a");
        
        var radarCircleTr = guixRoot.Find("RadarCircle");
        var displayOverlay = bioScreen.transform.Find("Screen_Display_2").gameObject;
        Renderer wideRadarImageRenderer;
        if (CFG.UseWideRadar)
        {
            radarCircleTr.Find("arrow_Element (1)").parent = guixRoot;
            radarCircleTr.Find("arrow_Element (4)").parent = guixRoot;
            radarCircleTr.localPosition = new Vector3(2.941f, -2.315f, 0.0f);
            radarCircleTr.Find("Circle_1 (1)").gameObject.SetActive(false);
            radarCircleTr.Find("Circle_1 (2)").gameObject.SetActive(false);

            var radarImagePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            radarImagePlane.name = "Wide Radar Gizmo";
            UnityEngine.Object.Destroy(radarImagePlane.GetComponent<Collider>());
            radarImagePlane.transform.parent = radarCircleTr;
            radarImagePlane.transform.localPosition = new Vector3(0.0f, 1.7f, 0.0f);
            radarImagePlane.transform.localEulerAngles = new Vector3(90.0f, 0.0f, 180.0f);
            radarImagePlane.transform.localScale = new Vector3(0.65856f, 1.0f, 0.3298462f);
            wideRadarImageRenderer = radarImagePlane.GetComponent<Renderer>();

            DisableChild(guixRoot, "ClampsCircle (1)");
            DisableChild(guixRoot, "MeeterBar_c (1)");
            DisableChild(guixRoot, "TopFrame (1)");
            DisableChild(guixRoot, "BackgroundDots_a");
            DisableChild(guixRoot, "gradientLines");
            DisableChild(guixRoot, "gradientLines (1)");
            DisableChild(guixRoot, "Button_Arrow_Bounce");
            DisableChild(guixRoot, "Button_SpinningCutCircle");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (1)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (2)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (3)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (4)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (5)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (6)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (7)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (8)");
            DisableChild(guixRoot, "ARIndicator/ARIndicatorText (9)");
        }
        else
        {
            wideRadarImageRenderer = radarCircleTr.Find("Circle_1 (1)").GetComponent<Renderer>();
        }

        var radarItems = radarCircleTr.GetComponentsInChildren<Renderer>();
        foreach (var radarItem in radarItems)
        {
            if (radarItem.gameObject.name.Equals("Wide Radar Gizmo"))
                continue;

            var item = radarItem.gameObject.AddComponent<RadarScreenItem>();
            item.Renderer.Value = radarItem;
            item.DefaultMat.Value = radarItem.sharedMaterial;
        }

        var screen = bioScreen.GetComponent<EnemyScannerScreen>();
        var screenRef = screen.gameObject.AddComponent<BioScreenRef>();
        screenRef.VirtScene.Value = vScene;
        screenRef.RadarImageRenderer.Value = wideRadarImageRenderer;
        screenRef.ScreenRenderer.Value = displayOverlay.GetComponent<Renderer>();

        s_ScreenPrefabProcessed = true;
    }

    [HarmonyPatch(typeof(EnemyScanner), nameof(EnemyScanner.OnGearSpawnComplete))]
    [HarmonyPostfix]
    static void Post_ScannerSpawned(EnemyScanner __instance)
    {
        __instance.gameObject.AddComponent<BetterBio>();
    }

    [HarmonyPatch(typeof(EnemyScannerGraphics), nameof(EnemyScannerGraphics.Start))]
    [HarmonyPrefix]
    static void Pre_GraphicsStart(EnemyScannerGraphics __instance)
    {
        if (CFG.UseWideRadar)
        {
            var displayTr = __instance.m_display.transform;
            displayTr.localPosition = new Vector3(0.0494f, 0.0f, 0.1315f);
            displayTr.localScale = new Vector3(0.3395262f, 1.102f, 0.166f);
        }
    }

    static void DisableChild(Transform guixRoot, string path)
    {
        var child = guixRoot.Find(path);
        if (child == null)
        {
            Logger.Error($"Unable to find: {path} from tracker screen!");
            return;
        }

        child.gameObject.SetActiveRecursively(false);
    }
}
