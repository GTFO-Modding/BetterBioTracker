using AK;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BetterBioTracker.Comps.BetterBio;

namespace BetterBioTracker.Comps;
internal sealed partial class BetterBio : MonoBehaviour
{
    public enum ScreenState
    {
        Default,
        Thermal_Radar,
        Thermal
    }

    private int _ScreenStateIndex = 0;
    private ScreenState[] _ScreenStatePool;
    
    private void Setup_ThermalVision()
    {
        var statePool = new List<ScreenState>
        {
            ScreenState.Default
        };

        if (CFG.UseThermalRadarMode)
            statePool.Add(ScreenState.Thermal_Radar);

        if (CFG.UseThermalOnlyMode)
            statePool.Add(ScreenState.Thermal);

        _ScreenStatePool = statePool.ToArray();
        statePool.Clear();
    }

    private void Update_ThermalVision_Input()
    {
        if (_ScreenStatePool.Length <= 1)
            return;

        if (InputMapper.GetButtonDownKeyMouseGamepad(InputAction.Reload, eFocusState.FPS))
        {
            _ScreenStateIndex = (_ScreenStateIndex + 1) % _ScreenStatePool.Length;
            SetThermalScreen(_ScreenStatePool[_ScreenStateIndex]);
        }
    }

    private void SetThermalScreen(ScreenState state)
    {
        switch(state)
        {
            case ScreenState.Default:
                _Graphics.m_display.forceRenderingOff = false;
                _ScreenRef.SetRadarGizmoEnabled(true);
                _ScreenRef.SetThermalEnabled(false);
                _Scanner.Sound.Post(EVENTS.BUTTONGENERICDEACTIVATE);
                break;

            case ScreenState.Thermal_Radar:
                _Graphics.m_display.forceRenderingOff = false;
                _ScreenRef.SetRadarGizmoEnabled(true);
                _ScreenRef.SetThermalEnabled(true);
                _Scanner.Sound.Post(EVENTS.BUTTONGENERICBLIPONE);
                break;

            case ScreenState.Thermal:
                _Graphics.m_display.forceRenderingOff = true;
                _ScreenRef.SetRadarGizmoEnabled(false);
                _ScreenRef.SetThermalEnabled(true);
                _Scanner.Sound.Post(EVENTS.BUTTONGENERICBLIPTWO);
                break;
        }
    }
}
