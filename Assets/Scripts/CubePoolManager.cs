using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubePoolManager : MonoBehaviour {

    public static CubePoolManager Instance;
    [SerializeField] private List<CubeSpawnAreaController> cubeSpawnAreaControllers;
    [SerializeField] private CubeController cube;
    [SerializeField, Range(0,100)] private int initialSpawnCount;
    [SerializeField] private float spawnHeight;
    private Queue<CubeController> _cubePool = new Queue<CubeController>();
    public Queue<CubeController> CubePool => _cubePool;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        // SpawnOvertime();
    }

    void InitialSpawn()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            var spawnedObj = Instantiate(cube,transform);
            _cubePool.Enqueue(spawnedObj);
            spawnedObj.gameObject.SetActive(false);
        }
    }

    // void SpawnOvertime()
    // {
    //     timePassed += Time.deltaTime;
    //
    //     if (!(timePassed > spawnCooldown))
    //         return;
    //     timePassed = 0f;
    //     if (_cubePool.Count == 0)
    //     {
    //         var spawnedObj = Instantiate(cube,transform);
    //         _cubePool.Enqueue(spawnedObj);
    //     }
    //     Spawn();
    // }
    
    public CubeController Spawn(Vector2 spawnRangeX, Vector2 spawnRangeZ, CubeSpawnAreaController cubeSpawnAreaController)
    {
        var spawnedObj = _cubePool.Dequeue();
        spawnedObj.gameObject.SetActive(true);
        spawnedObj.transform.SetParent(transform);
        spawnedObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        spawnedObj.SetCubeSpawnArea(cubeSpawnAreaController);

        var randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        var randomZ = Random.Range(spawnRangeZ.x, spawnRangeZ.y);
        spawnedObj.transform.position = new Vector3(randomX, spawnHeight, randomZ);

        return spawnedObj;
    }

    public void DynamicSpawn()
    {
        var spawnedObj = Instantiate(cube,transform);
        _cubePool.Enqueue(spawnedObj);
    }
    
    public CubeController SpawnOnBusiness()
    {
        if (_cubePool.Count == 0)
        {
            var _spawnedObj = Instantiate(cube,transform);
            _cubePool.Enqueue(_spawnedObj);
        }
        var spawnedObj = _cubePool.Dequeue();
        spawnedObj.gameObject.SetActive(true);
        return spawnedObj;
    }

    public void ReleaseCube(CubeController moneyController)
    {
        _cubePool.Enqueue(moneyController);
    }
}
