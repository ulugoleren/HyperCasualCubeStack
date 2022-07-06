using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HelperManager : MonoBehaviour {

	public static HelperManager Instance;
	[SerializeField] private List<CubeSpawnAreaController> cubeSpawnAreas;
	[SerializeField] private StorageController storageController;
	public Dictionary<CubeSpawnAreaController, bool> CubeSpawnAreaBusyness = new Dictionary<CubeSpawnAreaController, bool>();
	public Transform StorageHelperStop => storageController.helperStop;
	public Queue<HelperController> availableHelpers = new Queue<HelperController>();

	private void Awake()
	{
		Instance = this;
	}
	// Start is called before the first frame update
	void Start()
	{
		InitialArrangeDictionary();
	}

	// Update is called once per frame
	void Update() { }

	void InitialArrangeDictionary()
	{
		foreach (var cubeSpawnArea in cubeSpawnAreas)
		{
			CubeSpawnAreaBusyness.Add(cubeSpawnArea, false);
		}
	}

	public void TryArrangeTransportation()
	{
		if(availableHelpers.Count == 0)
			return;
		ArrangeTransportation(availableHelpers.Dequeue());
	}

	public void ArrangeTransportation(HelperController helperController)
	{
		int maxCubeCount = -1;
		CubeSpawnAreaController targetCubeSpawnArea = null;
		foreach (var cubeSpawnArea in cubeSpawnAreas)
		{
			if (cubeSpawnArea.CubesInArea.Count > maxCubeCount)
			{
				if(CubeSpawnAreaBusyness[cubeSpawnArea])
					continue;
				maxCubeCount = cubeSpawnArea.CubesInArea.Count;
				targetCubeSpawnArea = cubeSpawnArea;
			}
		}

		if (targetCubeSpawnArea != null)
		{
			CubeSpawnAreaBusyness[targetCubeSpawnArea] = true;
			helperController.MoveToSpawnArea(targetCubeSpawnArea);
		}
		else
		{
			availableHelpers.Enqueue(helperController);
		}
	}

	// public HelperController GetAvailableHelper()
	// {
	//     
	// }
}