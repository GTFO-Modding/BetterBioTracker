using GameData;
using Gear;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Player;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace BetterBioTracker.Comps;
internal sealed partial class BetterBio : MonoBehaviour
{
    private EnemyScanner _Scanner;
    private EnemyScannerGraphics _Graphics;
    private BioScreenRef _ScreenRef;

    private static event Action OnPreEnemyTargetRender;
    private static event Action OnPostEnemyTargetRender;
    private static event Action OnNavGenerationDone;

    void Start()
    {
        _Scanner = GetComponent<EnemyScanner>();
        _Graphics = _Scanner.m_graphics;
        _ScreenRef = _Scanner.m_screen.GetComponent<BioScreenRef>();

        if(CFG.UseOldZoom)
        {
            var fpsBlock = _Scanner.ItemFPSData;
            fpsBlock.localPosZoom = new Vector3(0.01f, -0.065f, 0.42f);
            fpsBlock.localRotZoom = new Vector3(-14.76f, 0.19f, 1f);

            var itemBlock = _Scanner.ItemDataBlock;
            itemBlock.ShowCrosshairWhenAiming = false;
        }

        Setup_CullTransform();
        Setup_DisplayTeammate();
        Setup_ThermalVision();
    }

    void OnDestroy()
    {
        Clear_DisplayTeammate();
        Clear_CullTransform();
    }

    private void Update()
    {
        Update_ThermalVision_Input();
        Update_CullTransform();
    }
}
