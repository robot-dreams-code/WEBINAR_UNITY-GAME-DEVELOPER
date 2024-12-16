using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.XR;

public class MirrorViewController : MonoBehaviour
{
    private void Awake()
    {
        XRSystem.GetActiveDisplay().SetPreferredMirrorBlitMode(XRMirrorViewBlitMode.None);
    }
}
