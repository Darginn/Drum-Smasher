using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScreenResolution : MonoBehaviour
{
    public bool MaintainWidth = true;

    [Range(-1,1)]
    public int AdaptPosition;

    private float _defaultWidth;
    private float _defaultHeight;

    private Vector3 _cameraPos;
    
    void Start()
    {
        _cameraPos = Camera.main.transform.position;

        _defaultHeight = Camera.main.orthographicSize;
        _defaultWidth = Camera.main.orthographicSize * (16f/9f);
    }

    
    void Update()
    {
        if(MaintainWidth)
        {
            Camera.main.orthographicSize = _defaultWidth / Camera.main.aspect;

            Camera.main.transform.position = new Vector3(_cameraPos.x,AdaptPosition*(_defaultHeight - Camera.main.orthographicSize),_cameraPos.z);
        }
        else
        {
            Camera.main.transform.position = new Vector3(AdaptPosition * (_defaultWidth - Camera.main.orthographicSize * Camera.main.aspect), _cameraPos.y, _cameraPos.z);
        }
    }
}
