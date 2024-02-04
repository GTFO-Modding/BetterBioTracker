using Dissonance.Demo;
using Enemies;
using Gear;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterBioTracker;
[HarmonyPatch(typeof(EnemyAgent), nameof(EnemyAgent.UpdateScannerData))]
internal class Inject_ColoredBio
{
    private static readonly Color s_GlueCol = new Color(0.5f, 0.5f, 0.5f, 1f);
    private static readonly Color s_HibernateCol = new Color(0.7f, 0.7f, 0.7f, 1f);
    private static readonly Color s_HeartbeatOnCol = Color.yellow.RGBMultiplied(0.7f);
    private static readonly Color s_HeartbeatOffCol = Color.yellow.RGBMultiplied(0.6f);
    private static readonly Color s_HibernateWakeupCol = new Color(0.8f, 0.4549f, 0.0392f, 1.0f);
    private static readonly Color s_ScoutRoamingCol = Color.yellow.RGBMultiplied(0.7f);
    private static readonly Color s_ScoutFeelerCol = new Color(1f, 0.1f, 0.1f, 1f);
    private static readonly Color s_ScoutScreamCol = Color.cyan;
    private static readonly Color s_ActiveCol = new Color(1f, 0.1f, 0.1f, 1f);

    static void Postfix(EnemyAgent __instance)
    {
        if (!CFG.UseColoredTargets)
        {
            var col = __instance.m_scannerColor.AlphaMultiplied(CFG.TargetSizeMult);
            __instance.m_scannerData.m_matProp.SetColor(EnemyScannerGraphics._Color, col);
            return;
        }

        var color = GetStateColor(__instance).AlphaMultiplied(CFG.TargetSizeMult);
        __instance.m_scannerData.m_matProp.SetColor(EnemyScannerGraphics._Color, color);
    }

    static Color GetStateColor(EnemyAgent agent)
    {
        var locomo = agent.Locomotion;
        var heartbeatActive = locomo.Hibernate.m_heartbeatActive;
        switch (agent.Locomotion.CurrentStateEnum)
        {
            case ES_StateEnum.Hibernate:
                if (agent.IsHibernationDetecting)
                {
                    return heartbeatActive ? s_HeartbeatOnCol : s_HeartbeatOffCol;
                }
                return s_HibernateCol;

            case ES_StateEnum.HibernateWakeUp:
                return s_HibernateWakeupCol;

            case ES_StateEnum.ScoutDetection:
                return s_ScoutFeelerCol;

            case ES_StateEnum.ScoutScream:
                return s_ScoutScreamCol;

            case ES_StateEnum.PathMove:
            case ES_StateEnum.PathMoveFlyer:
                if (agent.AI.m_scoutPath == null)
                    return s_ActiveCol;

                if (agent.Locomotion.ScoutScream.m_state == ES_ScoutScream.ScoutScreamState.Done)
                    return s_ActiveCol;

                return s_ScoutRoamingCol;

            case ES_StateEnum.StuckInGlue:
                return s_GlueCol;

            default:
                return s_ActiveCol;
        }
    }
}
