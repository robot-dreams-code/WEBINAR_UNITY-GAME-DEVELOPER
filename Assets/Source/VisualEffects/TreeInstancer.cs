using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[ExecuteAlways]
public class TreeInstancer : MonoBehaviour
{
    [SerializeField] private Mesh _treeMesh;
    [SerializeField] private Material _trunkMaterial;
    [SerializeField] private Material _crownMaterial;
    [SerializeField] private Transform[] _pointAnchors;
    [SerializeField] private int[] _instanceCounts;
    [SerializeField] private int _rowCount;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private float _positionVariance;
    [SerializeField] private Vector2 _heightRange;
    [SerializeField] private float _rowOffset;
    [SerializeField] private ShadowCastingMode _shadowCastingMode;
    
    private Matrix4x4[] _matrices;
    private MaterialPropertyBlock _propertyBlock;
    private Vector3[] _points;

    private void OnEnable()
    {
        ResetInstances();
    }

    private void Update()
    {
        Graphics.DrawMeshInstanced(_treeMesh, 0, _trunkMaterial, _matrices, _matrices.Length, _propertyBlock, _shadowCastingMode);
        Graphics.DrawMeshInstanced(_treeMesh, 1, _crownMaterial, _matrices, _matrices.Length, _propertyBlock, _shadowCastingMode);
    }
    
    [ContextMenu("Reset instances")]
    private void ResetInstances()
    {
        _points = new Vector3[_pointAnchors.Length];
        for (int i = 0; i < _pointAnchors.Length; ++i)
            _points[i] = _pointAnchors[i].position;
        
        int instanceCount = 0;
        for (int i = 0; i < _instanceCounts.Length; ++i)
            instanceCount += _instanceCounts[i];
        instanceCount *= _rowCount;
        
        _matrices = new Matrix4x4[instanceCount];
        
        _propertyBlock = new MaterialPropertyBlock();

        int instanceOffset = 0;
        
        for (int i = 0; i < _instanceCounts.Length; ++i)
        {
            int currentInstanceCount = _instanceCounts[i];
            Vector3 startPoint = _points[i * 2];
            Vector3 endPoint = _points[i * 2 + 1];
            Vector3 direction = endPoint - startPoint;
            float directionOffset = direction.magnitude / (currentInstanceCount - 1);
            direction.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            float rowOffset = -_rowOffset * ((_rowCount - 1) * 0.5f);
            
            for (int j = 0; j < currentInstanceCount; ++j)
            {
                for (int k = 0; k < _rowCount; ++k)
                {
                    Vector3 scale = Vector3.one * Random.Range(_heightRange.x, _heightRange.y);
                    Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    Vector3 position = _positionOffset +
                                       startPoint + direction * directionOffset * j +
                                       right * (rowOffset + k * _rowOffset);
                    Vector2 variance = Random.insideUnitCircle * _positionVariance;
                    position.x += variance.x;
                    position.z += variance.y;
                    _matrices[instanceOffset] = Matrix4x4.TRS(position, rotation, scale);
                    instanceOffset++;
                }
            }
            
        }
    }
}