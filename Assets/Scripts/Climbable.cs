using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbable : MonoBehaviour {
	private BoxCollider2D collider;

	void Start() {
		collider = GetComponent<BoxCollider2D>();
	}

	//Returns where the pos is on the collider
	private float posToLocalScale(Vector2 pos) {
		collider.bounds.ClosestPoint(pos);

		return 0;
	}
}
