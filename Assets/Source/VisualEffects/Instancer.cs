using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class Instancer : MonoBehaviour
{
    private const string DRAW_DATA_PROPERTY = "_DrawData";
    private const string WIND_POSITION_PROPERTY = "_WindPosition";
    private const string INSTANCE_COUNT_PROPERTY = "_InstanceCount";
    private const string WIND_DIRECTION_PROPERTY = "_WindDirection";
    
    public struct DrawData
    {
        public float4x4 objectToWorld;
        public float4 color;
    }
    
    private static float4 ConvertColor(Color color)
    {
        return new float4(color.r, color.g, color.b, color.a);
    }
    
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    [SerializeField] private Vector2Int _numInstances;
    [SerializeField] private float _offset;
    [SerializeField] private float _rotation;
    [SerializeField] private Vector3 _scale;
    [SerializeField] private Vector3 _posOffset;
    [SerializeField] private Vector3 _rotOffset;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private Vector2 _heightRange;
    [SerializeField] private float _heightFactor;
    [SerializeField] private Vector2 _windSpeed;
    [SerializeField] private Vector3 _windDirection;
    [SerializeField] private ShadowCastingMode _shadowCastingMode;
    
    private MaterialPropertyBlock _materialPropertyBlock;

    private Vector2 _windPosition;
    
    private ComputeBuffer _drawBuffer;
    private ComputeBuffer _dataBuffer;

    private int _drawDataId;
    private int _windPositionId;
    private int _windDirectionId;
    private int _instanceCountId;
    
    private DrawData[] _drawData;
    
    private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
    
    private void OnEnable()
    {
        _drawDataId = Shader.PropertyToID(DRAW_DATA_PROPERTY);
        _windPositionId = Shader.PropertyToID(WIND_POSITION_PROPERTY);
        _windDirectionId = Shader.PropertyToID(WIND_DIRECTION_PROPERTY);
        _instanceCountId = Shader.PropertyToID(INSTANCE_COUNT_PROPERTY);
        
        ResetInstances();
    }

    private void OnDisable()
    {
        _drawBuffer?.Release();
        _dataBuffer?.Release();
    }

    private void Update()
    {
        _windPosition += _windSpeed * Time.deltaTime;
        
        _materialPropertyBlock.SetVector(_windPositionId, _windPosition);
        _materialPropertyBlock.SetVector(_windDirectionId, _windDirection);
        //_materialPropertyBlock.SetVectorArray("_BaseColor", _colors);
        
        Graphics.DrawMeshInstancedIndirect(_mesh, 0, _material, new Bounds(Vector3.zero, Vector3.one * 1000f),
            _drawBuffer, 0, _materialPropertyBlock, _shadowCastingMode);
    }

    [ContextMenu("Reset instances")]
    private void ResetInstances()
    {
        Vector2Int semirowCount = _numInstances - Vector2Int.one;
        Vector2Int instanceCount = new Vector2Int(_numInstances.x + semirowCount.x, _numInstances.y + semirowCount.y);
        
        uint bufferLength = (uint)(instanceCount.x * instanceCount.y);
        uint indexCount = _mesh.GetIndexCount(0);
        
        _drawBuffer?.Release();
        _drawBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        _args[0] = indexCount;
        _args[1] = bufferLength;
        _drawBuffer.SetData(_args);
        
        _dataBuffer?.Release();
        _dataBuffer = new ComputeBuffer((int)bufferLength, Marshal.SizeOf<DrawData>());
        
        _drawData = new DrawData[bufferLength];
        
        _materialPropertyBlock = new MaterialPropertyBlock();
        float xOffset = ((_numInstances.x / 2f) - 0.5f) * _offset;
        float zOffset = ((_numInstances.y / 2f) - 0.5f) * _offset;
        Quaternion rotOffset = Quaternion.Euler(_rotOffset);
        for (int i = 0; i < _numInstances.x; ++i)
        {
            for (int j = 0; j < _numInstances.y; ++j)
            {
                Vector3 scale = new Vector3(_scale.x, UnityEngine.Random.Range(_heightRange.x, _heightRange.y),
                    _scale.z);
                Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * rotOffset;
                Vector3 position = new Vector3(-xOffset + (i * _offset), _heightFactor * scale.y, -zOffset + (j * _offset)) + _posOffset;
                _drawData[i * _numInstances.y + j].objectToWorld = Matrix4x4.TRS(position, rotation, scale);
                _drawData[i * _numInstances.y + j].color = ConvertColor(_gradient.Evaluate(UnityEngine.Random.value));
            }
        }

        float xSemirowOffset = ((semirowCount.x / 2f) - 0.5f) * _offset;
        float zSemirowOffset = ((semirowCount.y / 2f) - 0.5f) * _offset;
        int indexOffset = _numInstances.x * _numInstances.y;
        for (int i = 0; i < semirowCount.x; ++i)
        {
            for (int j = 0; j < semirowCount.y; ++j)
            {
                int index = i * semirowCount.y + j + indexOffset;
                
                Vector3 scale = new Vector3(_scale.x, UnityEngine.Random.Range(_heightRange.x, _heightRange.y),
                    _scale.z);
                Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * rotOffset;
                Vector3 position = new Vector3(-xSemirowOffset + (i * _offset), _heightFactor * scale.y, -zSemirowOffset + (j * _offset)) + _posOffset;
                
                _drawData[index].objectToWorld = Matrix4x4.TRS(position, rotation, scale);
                _drawData[index].color = ConvertColor(_gradient.Evaluate(UnityEngine.Random.value));
            }
        }
        
        _dataBuffer.SetData(_drawData);
        _materialPropertyBlock.SetBuffer(_drawDataId, _dataBuffer);
        _materialPropertyBlock.SetFloat(_instanceCountId, bufferLength);
    }
}
