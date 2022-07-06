using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

    public static ZoneManager Instance;
    [SerializeField] private List<ZoneController> zoneControllers;
    [SerializeField] private List<ZoneSpawnAreaUnlockPair> zoneSpawnAreaUnlockPairList;
    private int _availableIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        zoneControllers[_availableIndex].Arrange();
    }

    public void IndexIncrement()
    {
        _availableIndex++;
        if(_availableIndex >= zoneControllers.Count)
            return;
        zoneControllers[_availableIndex].Arrange();
    }

    public void UnlockSpawnArea(ZoneController zoneController)
    {
        var zoneSpawnAreaUnlockPair = zoneSpawnAreaUnlockPairList.Find(x => x.zoneController == zoneController);
        if(zoneSpawnAreaUnlockPair == null)
            return;
        zoneSpawnAreaUnlockPair.cubeSpawnAreaController.IsAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class ZoneSpawnAreaUnlockPair {
    public ZoneController zoneController;
    public CubeSpawnAreaController cubeSpawnAreaController;
}
