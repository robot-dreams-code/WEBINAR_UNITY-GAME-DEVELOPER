using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitBase : MonoBehaviour
{
    public event Action onDetargeted;
    public event Action<UnitBase> onDeath;
    
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;
    [SerializeField] private float _speed;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _movementAnimation;
    [SerializeField] private string _deathAnimation;
    [SerializeField] private Transform _unitRoot;
    
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _position;
    private Vector3 _rootPosition;

    private float _duration;
    private float _time;
    private float _reciprocal;
    private float _currentSpeed;
    
    public Vector3 Position => _position;
    
    public void Begin(Vector3 startPosition, Vector3 endPosition)
    {
        _rootPosition = _startPosition = _position = startPosition;
        _endPosition = endPosition;
        _duration = (_endPosition - _startPosition).magnitude / _speed;
        _reciprocal = 1f / _duration;
        _animator.Play(_movementAnimation, 0, Random.Range(0f, 1f));
        _unitRoot.SetParent(null);
    }

    public void OnTargeted()
    {
        
    }

    public void OnDetargeted()
    {
        onDetargeted?.Invoke();
    }

    public void Die()
    {
        enabled = false;
        _collider.enabled = false;
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _animator.CrossFade(_deathAnimation, 0.1f);
        
        onDeath?.Invoke(this);
    }
    
    private void FixedUpdate()
    {
        if (_time > _duration)
        {
            Destroy();
            return;
        }

        Vector3 prevPosition = _position;
        _position = Vector3.Lerp(_startPosition, _endPosition, _time * _reciprocal);
        _rigidbody.position = _position;
        _currentSpeed = (_position - prevPosition).magnitude / Time.fixedDeltaTime;
        _time += Time.fixedDeltaTime;
    }

    private void Update()
    {
        _rootPosition = Vector3.MoveTowards(_rootPosition, _position, _currentSpeed * Time.deltaTime);
        _unitRoot.position = _rootPosition;
    }

    public void Destroy()
    {
        Destroy(gameObject);
        Destroy(_unitRoot.gameObject);
    }
}
