using UnityEngine;

public class UnitAnimationEventHandler : MonoBehaviour
{
    [SerializeField] private UnitBase _unitBase;

    private void Destroy()
    {
        _unitBase.Destroy();
    }
}
