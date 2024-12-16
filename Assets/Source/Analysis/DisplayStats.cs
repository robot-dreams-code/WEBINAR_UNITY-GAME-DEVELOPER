using System;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStats : MonoBehaviour
{
    const string k_FpsFormatString = "{0:F1}";
    const string k_MsFormatString = "{0:F2}ms";
    const float k_RefreshRate = 1f / 5f;
    
    private struct FrameTimeSample
    {
        public float FramesPerSecond;
        public float FullFrameTime;
        public float MainThreadCPUFrameTime;
        public float MainThreadCPUPresentWaitTime;
        public float RenderThreadCPUFrameTime;
        public float GPUFrameTime;

        public FrameTimeSample(float initValue)
        {
            FramesPerSecond = initValue;
            FullFrameTime = initValue;
            MainThreadCPUFrameTime = initValue;
            MainThreadCPUPresentWaitTime = initValue;
            RenderThreadCPUFrameTime = initValue;
            GPUFrameTime = initValue;
        }
    }

    [SerializeField] private Text _graphicsAPIValue;
    [SerializeField] private Text _frameRateValue;
    [SerializeField] private Text _frameTimeValue;
    [SerializeField] private Text _cpuMainThreadFrameValue;
    [SerializeField] private Text _cpuRenderThreadFrameValue;
    [SerializeField] private Text _cpuPresentWaitValue;
    [SerializeField] private Text _gpuFrameValue;
    
    private FrameTiming[] _frameTimings = new FrameTiming[1];
    private FrameTimeSample _sample;
    private float _time;

    private void Start()
    {
        _graphicsAPIValue.text = SystemInfo.graphicsDeviceType.ToString();
    }

    private void Update()
    {
        if (_time < k_RefreshRate)
        {
            _time += Time.deltaTime;
            return;
        }

        _time = 0f;
        
        _frameTimings[0] = default;
        _sample = default;
        
        FrameTimingManager.CaptureFrameTimings();
        FrameTimingManager.GetLatestTimings(1, _frameTimings);
        FrameTiming frameTiming = _frameTimings[0];
        
        _sample.FullFrameTime = (float)frameTiming.cpuFrameTime;
        float pureFps = _sample.FullFrameTime > 0f ? 1000f / _sample.FullFrameTime : 0f;
        _sample.FramesPerSecond = pureFps > Application.targetFrameRate ? Application.targetFrameRate : pureFps;
        _sample.MainThreadCPUFrameTime = (float)frameTiming.cpuMainThreadFrameTime;
        _sample.MainThreadCPUPresentWaitTime = (float)frameTiming.cpuMainThreadPresentWaitTime;
        _sample.RenderThreadCPUFrameTime = (float)frameTiming.cpuRenderThreadFrameTime;
        _sample.GPUFrameTime = (float)frameTiming.gpuFrameTime;
        
        _frameRateValue.text = string.Format(k_FpsFormatString, _sample.FramesPerSecond);
        _frameTimeValue.text = string.Format(k_MsFormatString, _sample.FullFrameTime);
        _cpuMainThreadFrameValue.text = string.Format(k_MsFormatString, _sample.MainThreadCPUFrameTime);
        _cpuRenderThreadFrameValue.text = string.Format(k_MsFormatString, _sample.RenderThreadCPUFrameTime);
        _cpuPresentWaitValue.text = string.Format(k_MsFormatString, _sample.MainThreadCPUPresentWaitTime);
        _gpuFrameValue.text = string.Format(k_MsFormatString, _sample.GPUFrameTime);
    }
    
    
}
