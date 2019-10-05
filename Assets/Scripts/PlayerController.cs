using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	Rigidbody2D rb;
	CameraController cc;
	public float movementSpeed = 2;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		cc = Camera.main.GetComponent<CameraController>();

	}
	
	void FixedUpdate() {
		if (!rb) return;

		float move = Input.GetAxis("Horizontal") * movementSpeed;

		rb.velocity = move * Vector2.right + rb.velocity.y * Vector2.up;

		cc.updateCamera(rb.velocity);
	}
}
