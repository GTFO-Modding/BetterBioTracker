using FluffyUnderware.DevTools.Extensions;
using Gear;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterBioTracker.Comps;
internal sealed partial class BetterBio : MonoBehaviour
{
    private static Transform s_CullTransform;

    private void Setup_CullTransform()
    {
        s_CullTransform = new GameObject("BioTracker Culling Transform").transform;
    }

    private void Clear_CullTransform()
    {
        if (s_CullTransform != null)
        {
            UnityEngine.Object.Destroy(s_CullTransform.gameObject);
            s_CullTransform = null;
        }
    }

    private void Update_CullTransform()
    {
        if (_Scanner != null && _Scanner.Owner != null)
        {
            s_CullTransform.position = _Scanner.Owner.Position;
            if (_Graphics.m_scanFwd.NotApproximately(Vector3.zero))
            {
                var rotation = Quaternion.identity;
                rotation.SetLookRotation(_Graphics.m_scanFwd, Vector3.up);
                s_CullTransform.rotation = rotation;
            }
            else
            {
                s_CullTransform.rotation = Quaternion.identity;
            }
        }
    }

    static bool IsTargetCulled(EnemyScannerGraphics graphics, Vector3 position)
    {
        var heightFactor = graphics.m_display.transform.localScale.z / graphics.m_display.transform.localScale.x;
        var widthHalf = graphics.m_width * 0.5f;
        var height = graphics.m_width * heightFactor;
        var relPos = s_CullTransform.InverseTransformPoint(position);
        return Mathf.Abs(relPos.x) > widthHalf || relPos.z > height || relPos.z < 0.0f;
    }
}
