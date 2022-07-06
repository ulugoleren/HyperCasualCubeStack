using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasMovementDisplay : MonoBehaviour {

	[SerializeField] private Image circleOutline;
	[SerializeField] private Image circle;
	[SerializeField] private float threshold;
	// Start is called before the first frame update
	void Start()
	{
        
	}

	// Update is called once per frame
	void LateUpdate()
	{
		if (Input.GetMouseButtonDown(0))
		{
			circleOutline.enabled = true;
			circleOutline.transform.position = Input.mousePosition;
			circle.enabled = true;
			circle.transform.position = Input.mousePosition;
		}
		else if (Input.GetMouseButton(0))
		{
			var currentMousePosition = Input.mousePosition;
			if (Vector3.Distance(circleOutline.transform.position, currentMousePosition) < threshold)
				circle.transform.position = Input.mousePosition;
			else
			{
				var direction = currentMousePosition - circleOutline.transform.position;
				direction = direction.normalized * threshold;
				circle.transform.position = circleOutline.transform.position + direction;
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			circleOutline.enabled = false;
			circle.enabled = false;
		}
	}
}