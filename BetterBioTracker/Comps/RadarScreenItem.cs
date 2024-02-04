using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterBioTracker.Comps;
internal sealed class RadarScreenItem : MonoBehaviour
{
    public Il2CppReferenceField<Material> DefaultMat;
    public Il2CppReferenceField<Renderer> Renderer;

    public void SetVisible(bool visible)
    {
        Renderer.Value.sharedMaterial = visible ? DefaultMat : BioScreenRef.Mat_Invisible;
        gameObject.SetActive(visible);
    }
}
