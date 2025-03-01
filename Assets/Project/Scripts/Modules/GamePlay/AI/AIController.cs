using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private DiamondClick _diamondClick;

    [SerializeField] private GameObject _aiObject;
    private IAIBehavior _ai;

    private BoundsInt _bounds;

    void Awake()
    {
        _diamondClick = GetComponent<DiamondClick>();
        
        _bounds = GamePlayManager.Instance.BoardBounds;
    }

    void OnEnable()
    {
        _ai = _aiObject.GetComponent<IAIBehavior>();
        _ai.SetDiamondClick(_diamondClick);
    }

    public void PlayTurn()
    {
        StartCoroutine(_ai.SelectTile());
    }
}
