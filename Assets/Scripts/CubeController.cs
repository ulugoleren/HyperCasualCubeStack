using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {

    // private BoxCollider _boxCollider;
    // public BoxCollider BoxCollider => _boxCollider;
    
    private CubeSpawnAreaController _cubeSpawnAreaController;

    public void SetCubeSpawnArea(CubeSpawnAreaController cubeSpawnAreaController) => _cubeSpawnAreaController = cubeSpawnAreaController;
    public void Collected()
    {
        if(_cubeSpawnAreaController == null)
            return;
        _cubeSpawnAreaController.CubesInArea.Remove(this);
        _cubeSpawnAreaController = null;
    }

    public void MyReset()
    {
        gameObject.SetActive(false);
        CubePoolManager.Instance.ReleaseCube(this);
    }

}
