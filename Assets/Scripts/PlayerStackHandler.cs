using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerStackHandler : MonoBehaviour {

	public static PlayerStackHandler Instance;
	[SerializeField] private List<StackController> cubeStacks;
	[SerializeField] private AnimationCurve moveCurve;
	[SerializeField] private AnimationCurve heightCurveUp;
	[SerializeField] private AnimationCurve heightCurveDown;
	[SerializeField] private float moveDuration;
	public float MoveDuration => moveDuration;
	[SerializeField] private float depositCooldown;
	private float depositTimePassed = 0f;
	private int cubeCount;
	private int CubeCount
	{ get => cubeCount;
	  set {

		  cubeCount = value;
		  CanvasController.Instance.UpdateCubeCounter(value);

	  } }

	public bool IsPlayerFull => cubeStacks[^1].IsFull;
	public ZoneController CurrentZone { get; set; }
	public CubeSpawnAreaController CurrentSpawnArea { get; set; }
	public StorageController CurrentStorage { get; set; }


	private void Awake()
	{
		Instance = this;
	}

	// Update is called once per frame
	void Update()
	{
		DepositProcess();
		WithdrawProcess();
		StorageWithdrawProcess();
	}

	private void OnTriggerEnter(Collider other)
	{
		// if (other.TryGetComponent<CubeController>(out var cubeController))
		// {
		// 	CollectCube(cubeController);
		// }

		if (other.TryGetComponent<ZoneController>(out var zoneController))
		{
			if (CurrentZone != null)
				return;

			CurrentZone = zoneController;
			depositTimePassed = 0f;
		}
		else if (other.TryGetComponent<CubeSpawnAreaController>(out var cubeSpawnAreaController))
		{
			if (CurrentSpawnArea != null)
				return;

			CurrentSpawnArea = cubeSpawnAreaController;
			depositTimePassed = 0f;
		}
		else if (other.TryGetComponent<StorageController>(out var storageController))
		{
			if (CurrentStorage != null)
				return;

			CurrentStorage = storageController;
			depositTimePassed = 0f;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent<ZoneController>(out var zoneController))
		{
			if (CurrentZone == null)
				return;

			CurrentZone = null;
			depositTimePassed = 0f;
			depositCooldown = .2f;
		}
		else if (other.TryGetComponent<CubeSpawnAreaController>(out var cubeSpawnAreaController))
		{
			if (CurrentSpawnArea == null)
				return;

			CurrentSpawnArea = null;
			depositTimePassed = 0f;
			depositCooldown = .2f;
		}
		else if (other.TryGetComponent<StorageController>(out var storageController))
		{
			if (CurrentStorage == null)
				return;

			CurrentStorage = null;
			depositTimePassed = 0f;
			depositCooldown = .2f;
		}
	}

	public void CollectCube(CubeController cubeController)
	{
		if (cubeStacks[^1].IsFull)
		{
			Debug.Log("Tüm stackler dolu");
			return;
		}

		for (int i = 0; i < cubeStacks.Count; i++)
		{

			if (cubeStacks[i].IsFull)
			{
				continue;
			}

			cubeStacks[i].AddToStack(cubeController);
			CubeCount++;
			return;
		}


	}

	void DepositProcess()
	{
		if (CurrentZone == null)
			return;
		if (CurrentZone.IsPurchased || !CurrentZone.IsOnSale)
			return;

		depositTimePassed += Time.deltaTime;

		if (depositTimePassed < depositCooldown)
			return;

		var targetCube = GetAvailableCube();
		if (targetCube == null)
			return;

		depositTimePassed = 0f;
		targetCube.transform.SetParent(CurrentZone.transform);
		StartCoroutine(ParabolicMovement(targetCube.transform, Vector3.zero, moveDuration, moveCurve));
		targetCube.transform.DOLocalRotate(Vector3.zero, moveDuration);
		depositCooldown *= .9f;
		CubeCount--;
		//depositCooldown = Mathf.Clamp(depositCooldown, .05f, .2f);

		StartCoroutine(DepositOnFinish(targetCube, CurrentZone, moveDuration));
	}

	IEnumerator DepositOnFinish(CubeController cubeController, ZoneController zoneController, float delay)
	{
		zoneController.EvaluatePrice();
		yield return new WaitForSeconds(delay);
		cubeController.MyReset();
	}

	void WithdrawProcess()
	{
		if (CurrentSpawnArea == null)
			return;
		if (!CurrentSpawnArea.IsAvailable)
			return;
		if (IsPlayerFull)
			return;

		depositTimePassed += Time.deltaTime;

		if (depositTimePassed < depositCooldown)
			return;

		depositTimePassed = 0f;

		var targetCube = CurrentSpawnArea.GetAvailableCube();
		if (targetCube == null)
			return;

		CollectCube(targetCube);
		depositCooldown *= .9f;
		//depositCooldown = Mathf.Clamp(depositCooldown, .05f, .2f);
	}

	void StorageWithdrawProcess()
	{
		if (CurrentStorage == null)
			return;
		if (CurrentStorage.IsBusy)
			return;
		if (IsPlayerFull)
			return;

		depositTimePassed += Time.deltaTime;

		if (depositTimePassed < depositCooldown)
			return;

		depositTimePassed = 0f;

		var targetCube = CurrentStorage.GetAvailableCube();
		if (targetCube == null)
			return;

		CollectCube(targetCube);
		depositCooldown *= .9f;
		//depositCooldown = Mathf.Clamp(depositCooldown, .05f, .2f);
	}

	CubeController GetAvailableCube()
	{
		if (cubeStacks[0].CubeStack.Count == 0)
		{
			return null;
		}

		for (int i = cubeStacks.Count - 1; i >= 0; i--)
		{
			if (cubeStacks[i].CubeStack.Count == 0)
			{
				continue;
			}

			var result = cubeStacks[i].CubeStack[^1];
			cubeStacks[i].CubeStack.RemoveAt(cubeStacks[i].CubeStack.Count - 1);
			return result;
		}

		return null;
	}



	public void ParabolicMovement(Transform moveTarget, Vector3 targetPos)
	{
		StartCoroutine(ParabolicMovement(moveTarget, targetPos, moveDuration, moveCurve));
	}

	IEnumerator ParabolicMovement(Transform moveTarget, Vector3 targetPos, float duration, AnimationCurve _moveCurve)
	{
		var init = moveTarget.localPosition;

		var passed = 0f;

		while (passed < duration)
		{
			passed += Time.deltaTime;
			var normalized = passed / duration;
			var moveEvaluated = _moveCurve.Evaluate(normalized);
			var heightEvaluated = init.y < targetPos.y ? heightCurveUp.Evaluate(normalized) : heightCurveDown.Evaluate(normalized);

			var currentPos = Vector3.Lerp(init, targetPos, moveEvaluated);
			var currentHeight = Mathf.LerpUnclamped(init.y, targetPos.y, heightEvaluated);
			currentPos.y = currentHeight;
			moveTarget.localPosition = currentPos;


			yield return null;
		}
	}
}