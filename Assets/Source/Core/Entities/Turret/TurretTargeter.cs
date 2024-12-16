using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretTargeter : MonoBehaviour
{
    public Action onFirstTargetEnter;
    public Action onLastTargetExit;
    
    private HashSet<UnitBase> _targets = new();
    
    public int Count => _targets.Count;
    
    private void OnTriggerEnter(Collider other)
    {
        UnitBase unit = other.GetComponent<UnitBase>();
        if (unit == null)
            return;
        _targets.Add(unit);
        unit.OnTargeted();
        unit.onDeath += TargetOnDeathHandler;
        
        if (_targets.Count == 1)
            onFirstTargetEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        UnitBase unit = other.GetComponent<UnitBase>();
        if (unit == null)
            return;
        _targets.Remove(unit);
        unit.OnDetargeted();
        
        if (_targets.Count == 0)
            onLastTargetExit?.Invoke();
    }

    public UnitBase GetTarget()
    {
        switch (_targets.Count)
        {
            case 0:
                return null;
            case 1:
                return _targets.Single();
            default:
                float minSqrDistance = float.MaxValue;
                UnitBase closest = null;
                Vector3 selfPosition = transform.position;
                foreach (UnitBase target in _targets)
                {
                    float sqrDistance = (target.Position - selfPosition).sqrMagnitude;
                    if (sqrDistance >= minSqrDistance)
                        continue;
                    minSqrDistance = sqrDistance;
                    closest = target;
                }

                return closest;
        }
    }
    
    private void TargetOnDeathHandler(UnitBase unit)
    {
        _targets.Remove(unit);
        unit.OnDetargeted();
        
        if (_targets.Count == 0)
            onLastTargetExit?.Invoke();
    }
}
