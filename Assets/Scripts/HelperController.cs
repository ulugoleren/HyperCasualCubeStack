using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HelperController : MonoBehaviour {

    [SerializeField] private StackController cubeStack;
    public bool IsAvailable { get; set; }
    public CubeSpawnAreaController CurrentSpawnArea { get; set; }
    [SerializeField] private float depositCooldown = .2f;
    private float depositTimePassed = 0f;
    private bool depositProcess = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WithdrawProcess();
        DepositProcess();
    }

    private void OnEnable()
    {
        cubeStack.OnFull += OnFull;
    }

    private void OnDisable()
    {
        cubeStack.OnFull -= OnFull;
    }

    public void OnFull()
    {
        HelperManager.Instance.CubeSpawnAreaBusyness[CurrentSpawnArea] = false;
        HelperManager.Instance.TryArrangeTransportation();
        CurrentSpawnArea = null;
        MoveToStorage();
    }

    public void MoveToStorage()
    {
        var targetPosition = HelperManager.Instance.StorageHelperStop.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DORotateQuaternion(lookRotation, 1f));
        mySequence.Append(transform.DOMove(targetPosition, 5f));
        mySequence.OnComplete(OnArrivedStorage);
    }
    
    public void OnArrivedStorage()
    {
        if (StorageController.Instance.IsBusy || StorageController.Instance.IsPlayerWithdraw || StorageController.Instance.IsFull)
        {
            StorageController.Instance.availableToDepositHelpers.Enqueue(this);
            Debug.Log("queue count: " + StorageController.Instance.availableToDepositHelpers.Count);
            return;
        }

        StartDepositing();
    }

    public void StartDepositing()
    {
        Debug.Log(gameObject.name + " ben meşgul ettim");
        StorageController.Instance.IsBusy = true;
        depositProcess = true;
        depositCooldown = .2f;
    }

    public void MoveToSpawnArea(CubeSpawnAreaController cubeSpawnAreaController)
    {
        Quaternion lookRotation = Quaternion.LookRotation(cubeSpawnAreaController.helperStop.position - transform.position);

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DORotateQuaternion(lookRotation, 1f));
        mySequence.Append(transform.DOMove(cubeSpawnAreaController.helperStop.position, 5f));
        mySequence.OnComplete(() =>
        {
            OnArrivedSpawnArea(cubeSpawnAreaController);
        });
    }
    
    public void OnArrivedSpawnArea(CubeSpawnAreaController cubeSpawnAreaController)
    {
        CurrentSpawnArea = cubeSpawnAreaController;
        depositCooldown = .2f;
    }
    
    void WithdrawProcess()
    {
        if (CurrentSpawnArea == null)
            return;
        if (!CurrentSpawnArea.IsAvailable)
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
    
    void DepositProcess()
    {
        if(!depositProcess)
            return;
        if (StorageController.Instance.IsFull)
        {
            depositProcess = false;
            StorageController.Instance.IsBusy = false;
            if (cubeStack.CubeStack.Count != 0)
            {
                StorageController.Instance.availableToDepositHelpers.Enqueue(this);
                return;
            }
            
            HelperManager.Instance.ArrangeTransportation(this);
            return;
        }

        depositTimePassed += Time.deltaTime;

        if (depositTimePassed < depositCooldown)
            return;

        if (cubeStack.CubeStack.Count == 0)
        {
            depositProcess = false;
            HelperManager.Instance.ArrangeTransportation(this);
            StorageController.Instance.TryAvailableToDepositHelpers();
            return;
        }
        
        var targetCube = cubeStack.CubeStack[^1];
        cubeStack.CubeStack.RemoveAt(cubeStack.CubeStack.Count - 1);
        if (targetCube == null)
            return;

        depositTimePassed = 0f;
        
        depositCooldown *= .9f;
        StorageController.Instance.CollectCube(targetCube);
        
    }
	
    IEnumerator DepositOnFinish(CubeController cubeController, ZoneController zoneController, float delay)
    {
        zoneController.EvaluatePrice();
        yield return new WaitForSeconds(delay);
        cubeController.MyReset();
    }
    
    public void CollectCube(CubeController cubeController)
    {
        if (cubeStack.IsFull)
        {
            Debug.Log("Tüm stackler dolu");
            return;
        }

        cubeStack.AddToStack(cubeController);
    }

    

    public void ArrangeTransportation()
    {
        
    }
}
