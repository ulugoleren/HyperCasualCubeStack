using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    public static CanvasController Instance;
    [SerializeField] private TextMeshProUGUI cubeCounterText;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateCubeCounter(int value)
    {
        cubeCounterText.text = value.ToString();
    }
}
