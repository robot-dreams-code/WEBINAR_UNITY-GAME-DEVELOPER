using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;

public class XRVignetteController : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private LocomotionProvider[] _locomotionProviders;
    [SerializeField] private Vector2 _vignetteRange;
    [SerializeField] private float _speed;

    private Vignette _vignette;
    private float _targetIntensity;
    private float _currentIntensity;
    
    private void Start()
    {
        _volume.profile.TryGet(out _vignette);
        _vignette.intensity.value = _currentIntensity = _vignetteRange.x;
    }
    
    private void Update()
    {
        CheckProviders();
        
        _vignette.intensity.value = _currentIntensity = Mathf.MoveTowards(_currentIntensity, _targetIntensity, _speed * Time.deltaTime);
    }

    private void CheckProviders()
    {
        for (int i = 0; i < _locomotionProviders.Length; ++i)
        {
            LocomotionProvider locomotionProvider = _locomotionProviders[i];
            if (locomotionProvider.locomotionPhase is LocomotionPhase.Idle or LocomotionPhase.Done)
                continue;
            TryBeginVignette();
            return;
        }
        TryEndVignette();
    }

    private void TryBeginVignette()
    {
        _targetIntensity = _vignetteRange.y;
    }
    
    private void TryEndVignette()
    {
        _targetIntensity = _vignetteRange.x;
    }
}
