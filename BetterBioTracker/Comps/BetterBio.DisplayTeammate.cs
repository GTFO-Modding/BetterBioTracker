using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterBioTracker.Comps;
internal sealed partial class BetterBio : MonoBehaviour
{
    private MaterialPropertyBlock _PlayerCircleMatProps;

    private void Setup_DisplayTeammate()
    {
        _PlayerCircleMatProps = new MaterialPropertyBlock();
        _PlayerCircleMatProps.SetColor("_Color", Color.cyan.AlphaMultiplied(0.25f));

        OnPostEnemyTargetRender += PostRender_DisplayTeammate;
    }

    private void Clear_DisplayTeammate()
    {
        if (_PlayerCircleMatProps != null)
        {
            _PlayerCircleMatProps.Dispose();
            _PlayerCircleMatProps = null;
        }

        OnPostEnemyTargetRender -= PostRender_DisplayTeammate;
    }

    private void PostRender_DisplayTeammate()
    {
        if (!CFG.AddPlayersToTracker)
            return;

        var graphics = _Graphics;
        foreach (var player in PlayerManager.PlayerAgentsInLevel)
        {
            if (player.IsLocallyOwned)
                continue;

            if (IsTargetCulled(_Graphics, player.Position))
                continue;

            var trs = Matrix4x4.TRS(player.Position, Quaternion.identity, Vector3.one);
            var col = player.Owner.PlayerColor;
            Color.RGBToHSV(col, out var h, out var s, out _);
            col = Color.HSVToRGB(h, s, 1.0f).AlphaMultiplied(0.5f * CFG.TargetSizeMult);
            _PlayerCircleMatProps.SetColor("_Color", col);
            graphics.m_cmd.DrawMesh(graphics.m_targetMesh, trs, graphics.m_targetMaterial, 0, 0, _PlayerCircleMatProps);
        }
    }
}
