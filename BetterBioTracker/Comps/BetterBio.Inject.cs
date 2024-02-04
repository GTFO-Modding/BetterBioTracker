using Enemies;
using Gear;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterBioTracker.Comps;
internal sealed partial class BetterBio : MonoBehaviour
{
    [HarmonyPatch(typeof(EnemyScanner), nameof(EnemyScanner.Update))]
    static class Inject_EnemyScanner
    {
        static void Prefix()
        {
            OnPreEnemyTargetRender?.Invoke();
        }

        static void Postfix()
        {
            OnPostEnemyTargetRender?.Invoke();
        }
    }

    [HarmonyPatch(typeof(EnemyScannerGraphics), nameof(EnemyScannerGraphics.IsDetected))]
    static class Inject_EnemyScannerGraphics_Culling
    {
        static void Postfix(EnemyScannerGraphics __instance, EnemyAgent enemyAgent, ref bool __result)
        {
            __result = !IsTargetCulled(__instance, enemyAgent.Position);
        }
    }

    [HarmonyPatch(typeof(MapDetails), nameof(MapDetails.OnNavMeshGenerationDone))]
    static class Inject_MapDetails
    {
        static void Prefix()
        {
            OnNavGenerationDone?.Invoke();
        }
    }
}
