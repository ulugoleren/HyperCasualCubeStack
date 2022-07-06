using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageController : MonoBehaviour {
    public static StorageController Instance;
    [SerializeField] private List<StackController> cubeStacks;
    public Transform helperStop;
    public bool IsFull => cubeStacks[^1].IsFull;
    public bool IsBusy { get; set; }
    public bool IsPlayerInStorage => PlayerStackHandler.Instance.CurrentStorage != null;
    public bool IsPlayerWithdraw => IsPlayerInStorage && !IsBusy && !PlayerStackHandler.Instance.IsPlayerFull && cubeStacks[0].CubeStack.Count > 0;
    private bool previousIsPlayerWithdraw = false;
    
    public Queue<HelperController> availableToDepositHelpers = new Queue<HelperController>();

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
            return;
        }


    }
    
    public CubeController GetAvailableCube()
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

    public void TryAvailableToDepositHelpers()
    {
        Debug.Log("deneyelim hemen");
        IsBusy = false;
        
        if(IsPlayerInStorage && cubeStacks[0].CubeStack.Count != 0 && !PlayerStackHandler.Instance.IsPlayerFull)
            return;
        
        if(availableToDepositHelpers.Count == 0)
            return;
        
        availableToDepositHelpers.Dequeue().StartDepositing();
    }

    public void CheckIfPlayerWithdrawChanges()
    {
        if(previousIsPlayerWithdraw == IsPlayerWithdraw)
            return;
        
        Debug.Log("değişti" + IsPlayerWithdraw);
        previousIsPlayerWithdraw = IsPlayerWithdraw;
        if(!IsPlayerWithdraw)
            TryAvailableToDepositHelpers();
        
            
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfPlayerWithdrawChanges();
    }
}
