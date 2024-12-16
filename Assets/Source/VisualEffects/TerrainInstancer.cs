using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class TerrainInstancer : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private Vector2Int _numInstances;
    [SerializeField] private float _offset;
    [SerializeField] private Vector3 _scale;
    [SerializeField] private Vector3 _posOffset;
    [SerializeField] private Vector3 _rotOffset;
    [SerializeField] private ShadowCastingMode _shadowCastingMode;
    
    private MaterialPropertyBlock _materialPropertyBlock;
    private Matrix4x4[] _matrices;
    
    private void OnEnable()
    {
        ResetInstances();
    }

    private void Update()
    {
        Graphics.DrawMeshInstanced(_mesh, 0, _material, _matrices, _matrices.Length, _materialPropertyBlock, _shadowCastingMode);
    }

    [ContextMenu("Reset instances")]
    private void ResetInstances()
    {
        Vector2Int instanceCount = _numInstances;
        
        uint bufferLength = (uint)(instanceCount.x * instanceCount.y);
        
        _matrices = new Matrix4x4[bufferLength];
        
        _materialPropertyBlock = new MaterialPropertyBlock();
        float xOffset = ((_numInstances.x / 2f) - 0.5f) * _offset;
        float zOffset = ((_numInstances.y / 2f) - 0.5f) * _offset;
        Quaternion rotOffset = Quaternion.Euler(_rotOffset);
        for (int i = 0; i < _numInstances.x; ++i)
        {
            for (int j = 0; j < _numInstances.y; ++j)
            {
                Vector3 scale = _scale;
                Quaternion rotation = rotOffset;
                Vector3 position = new Vector3(-xOffset + (i * _offset), 0f, -zOffset + (j * _offset)) + _posOffset;
                _matrices[i * _numInstances.y + j] = Matrix4x4.TRS(position, rotation, scale);
            }
        }
    }
}