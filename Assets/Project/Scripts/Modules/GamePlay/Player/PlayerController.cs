using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    
    private DiamondClick _diamondClick;
    
    private Camera _camera;
    void Start()
    {
        _tilemap = GamePlayManager.Instance.Tilemap;
        _camera = Camera.main;
        _diamondClick = GetComponent<DiamondClick>();
    }

    void Update()
    {
        if (_diamondClick.CanClick())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPosition = _tilemap.WorldToCell(worldPoint);
                
                _diamondClick.SelectTile(gridPosition);
            }
        }
    }
}
