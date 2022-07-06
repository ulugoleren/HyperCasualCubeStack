using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class StackController : MonoBehaviour {
	
	[SerializeField] private float intervalValue = .2f;
	[SerializeField, Range(0,30)] private int maxStackCount;
	
	private List<CubeController> _cubeStack = new List<CubeController>();
	public List<CubeController> CubeStack => _cubeStack;
	public bool IsFull => _cubeStack.Count >= maxStackCount;
	public UnityAction OnFull;

	public Vector3 StackPivot => Vector3.zero;
	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update() { }

	// public void SpawnMoney()
	// {
	// 	if (IsFull)
	// 	{
	// 		Debug.Log("stack dolu");
	// 		return;
	// 	}
	//
	// 	var spawnedObj = MoneyPoolManager.Instance.SpawnOnBusiness();
	// 	spawnedObj.transform.SetParent(transform);
	// 	spawnedObj.GetComponent<BoxCollider>().enabled = false;
	// 	spawnedObj.transform.localPosition = _cubeStack.Count == 0 ? Vector3.zero : Vector3.up * intervalValue * _cubeStack.Count;
	// 	spawnedObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
	// 	_cubeStack.Add(spawnedObj);
	// }

	public void AddToStack(CubeController cubeController)
	{
		cubeController.transform.SetParent(transform);
		var targetPos = _cubeStack.Count == 0 ? StackPivot : StackPivot + Vector3.up * intervalValue * _cubeStack.Count;
		PlayerStackHandler.Instance.ParabolicMovement(cubeController.transform, targetPos);
		var moveDuration = PlayerStackHandler.Instance.MoveDuration;
		cubeController.transform.DOLocalRotate(Vector3.zero, moveDuration);
		cubeController.Collected();

		_cubeStack.Add(cubeController);
		if(IsFull)
			OnFull?.Invoke();
	}
}