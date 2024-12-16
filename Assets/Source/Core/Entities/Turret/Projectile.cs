using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;

    private UnitBase _target;
    
    public void Begin(UnitBase target)
    {
        _target = target;
        _target.onDeath += TargetOnDeathHandler;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        float positionDelta = _speed * Time.deltaTime;
        Vector3 direction = _target.Position - currentPosition;
        if (direction.sqrMagnitude < positionDelta)
        {
            _target.Die();
            Destroy(gameObject);
            return;
        }
        Vector3 position = Vector3.MoveTowards(currentPosition, _target.Position, positionDelta);
        Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.SetPositionAndRotation(position, rotation);
    }

    private void TargetOnDeathHandler(UnitBase unit)
    {
        Destroy(gameObject);
    }
}
