using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField] private Transform player;
	private Vector3 _followOffset;
	// Start is called before the first frame update
	void Start()
	{
		_followOffset = transform.position - player.position;
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = Vector3.Lerp(transform.position, player.position + _followOffset, Time.deltaTime);
	}
}