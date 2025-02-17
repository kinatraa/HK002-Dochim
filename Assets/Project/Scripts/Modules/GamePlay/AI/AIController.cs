using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private DiamondClick _diamondClick;
    
    

    void Start()
    {
        _diamondClick = GetComponent<DiamondClick>();
    }
}
