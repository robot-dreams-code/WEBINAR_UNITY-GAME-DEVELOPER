using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _targetFrameRate = 60;
    
    private void Awake()
    {
        Application.targetFrameRate = _targetFrameRate;
    }
}
