using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawnAreaController : MonoBehaviour {

	[SerializeField] private bool isAvailableAtStart;
	[SerializeField, Range(0, 5)] private float spawnCooldown;
	[SerializeField] private int maxCubeCount;
	public Transform helperStop;
	private float timePassed = 0f;
	private List<Vector3> meshVerticeList;
	private List<CubeController> _cubesInArea = new List<CubeController>();
	public bool IsAvailable { get; set; }
	public bool IsBusy { get; set; }

	public List<CubeController> CubesInArea => _cubesInArea;
	public bool IsFull => _cubesInArea.Count >= maxCubeCount;

	public bool IsAvailableAtStart => isAvailableAtStart;
	// Start is called before the first frame update
	void Start()
	{
		IsAvailable = isAvailableAtStart;
		meshVerticeList = new List<Vector3>(GetComponent<MeshFilter>().sharedMesh.vertices);
		InitialSpawn();
	}

	[ContextMenu("GetXCorners")]
	public Vector2 GetXCornerPositions()
	{
		var result = new Vector2();
		result.x = transform.TransformPoint(meshVerticeList[10]).x;
		result.y = transform.TransformPoint(meshVerticeList[0]).x;
		
		return result;
	}

	[ContextMenu("GetZCorners")]
	public Vector2 GetZCornerPositions()
	{
		var result = new Vector2();
		result.x = transform.TransformPoint(meshVerticeList[110]).z;
		result.y = transform.TransformPoint(meshVerticeList[0]).z;
		
		return result;
	}

	// Update is called once per frame
	void Update()
	{
		SpawnOvertime();
	}

	void SpawnOvertime()
	{
		timePassed += Time.deltaTime;

		if (IsFull || !IsAvailable)
			return;

		if (!(timePassed > spawnCooldown))
			return;
		timePassed = 0f;
		if (CubePoolManager.Instance.CubePool.Count == 0)
			CubePoolManager.Instance.DynamicSpawn();

		var xCornersPositions = GetXCornerPositions();
		var zCornersPositions = GetZCornerPositions();
		var spawnedObj = CubePoolManager.Instance.Spawn(xCornersPositions, zCornersPositions, this);
		_cubesInArea.Add(spawnedObj);
	}

	void InitialSpawn()
	{
		if (!isAvailableAtStart)
			return;

		for (int i = 0; i < maxCubeCount; i++)
		{
			if (CubePoolManager.Instance.CubePool.Count == 0)
				CubePoolManager.Instance.DynamicSpawn();

			var xCornersPositions = GetXCornerPositions();
			var zCornersPositions = GetZCornerPositions();
			var spawnedObj = CubePoolManager.Instance.Spawn(xCornersPositions, zCornersPositions, this);
			_cubesInArea.Add(spawnedObj);
		}
	}

	public CubeController GetAvailableCube()
	{
		return _cubesInArea.Count == 0 ? null : _cubesInArea[^1];

	}
}