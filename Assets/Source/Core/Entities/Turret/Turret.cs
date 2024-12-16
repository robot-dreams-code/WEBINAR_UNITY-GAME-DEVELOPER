using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private TurretTargeter _targeter;
    [SerializeField] private Transform _jointTransform;
    [SerializeField] private Transform _muzzleTransform;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _cooldown;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _recoilName;

    private UnitBase _target;
    private bool _hasTarget;
    private bool _hasCooldown;
    
    private YieldInstruction _cooldownWait;
    
    private Shot _shot;
    
    private void Start()
    {
        _targeter.onFirstTargetEnter += OnFirstTargetEnterHandler;
        
        _cooldownWait = new WaitForSeconds(_cooldown);
    }

    private void OnFirstTargetEnterHandler()
    {
        _targeter.onFirstTargetEnter -= OnFirstTargetEnterHandler;
        _targeter.onLastTargetExit += OnLastTargetExitHandler;
        _target = _targeter.GetTarget();
        _target.onDetargeted += OnTargetExitHandler;
        _hasTarget = true;
    }

    private void OnLastTargetExitHandler()
    {
        _targeter.onLastTargetExit -= OnLastTargetExitHandler;
        _targeter.onFirstTargetEnter += OnFirstTargetEnterHandler;
        _target = null;
        _hasTarget = false;
    }

    private void OnTargetExitHandler()
    {
        _target.onDetargeted -= OnTargetExitHandler;
        _target = null;
        _target = _targeter.GetTarget();
        if (_target == null)
        {
            _hasTarget = false;
            return;
        }
        _target.onDetargeted += OnTargetExitHandler;
    }

    private void Update()
    {
        if (!_hasTarget)
            return;
        Quaternion targetRotation = Quaternion.LookRotation((_target.Position - _jointTransform.position).normalized);
        Quaternion currentRotation = _jointTransform.rotation;
        float angle = Quaternion.Angle(targetRotation, currentRotation);
        float maxAngle = _rotationSpeed * Time.deltaTime;

        Quaternion rotation;
        
        if (angle > maxAngle)
        {
            rotation = Quaternion.RotateTowards(currentRotation, targetRotation, maxAngle);
        }
        else
        {
            rotation = Quaternion.RotateTowards(currentRotation, targetRotation, angle);

            if (!_hasCooldown)
            {
                _shot = new Shot { target = _target };
                _hasCooldown = true;
                StartCoroutine(Cooldown(_cooldownWait));
                _animator.Play(_recoilName);
            }
        }

        _jointTransform.rotation = rotation;
    }

    private void Shoot()
    {
        if (_shot == null)
            return;
        Instantiate(_projectilePrefab, _muzzleTransform.position, _muzzleTransform.rotation).Begin(_shot.target);
        _shot = null;
    }
    
    private IEnumerator Cooldown(YieldInstruction cooldown)
    {
        yield return cooldown;
        _hasCooldown = false;
    }
}
