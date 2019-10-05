using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public Transform player;
	public Vector3 offset = new Vector3(0, 0, -10);

	
	public void updateCamera(Vector2 playerVel) {
		if (!player) return;

		transform.position = player.position + offset + (Vector3) playerVel * Time.fixedDeltaTime;
	}
}
