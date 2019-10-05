using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public Transform player;
	public Vector3 offset = new Vector3(0, 0, -10);

	void Setup() {
		if (!player) player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void Update() {
		//If there is no player, then don't update the camera
		if (!player) return;

		//Set position of camera to the players posisiton plus an offset
		transform.position = player.position + offset;
	}
}
