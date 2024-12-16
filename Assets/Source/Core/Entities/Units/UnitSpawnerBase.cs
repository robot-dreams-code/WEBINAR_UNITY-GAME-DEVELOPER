using UnityEngine;

public class UnitSpawnerBase : MonoBehaviour
{
    [SerializeField] private UnitBase _unitPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _spawnInterval;

    private float _time = 0f;

    private void Awake()
    {
        enabled = false;
    }

    public void Begin()
    {
        enabled = true;
    }

    private void Update()
    {
        if (_time < _spawnInterval)
        {
            _time += Time.deltaTime;
            return;
        }
        
        Instantiate(_unitPrefab, _spawnPoint.position, _spawnPoint.rotation).Begin(_spawnPoint.position, _endPoint.position);
        _time = 0f;
    }
}
