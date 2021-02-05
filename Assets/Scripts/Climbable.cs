using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbable : MonoBehaviour {
	void Start() {
		BoxCollider2D collider = GetComponent<BoxCollider2D>();

		//Create egde that the player can hold on to
		EdgeCollider2D[] edges = new EdgeCollider2D[4];
		for (int i = 0; i < edges.Length; i++) {
			edges[i] = gameObject.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
		}

		float minX = -collider.size.x / 2.0f + collider.offset.x;
		float minY = -collider.size.y / 2.0f + collider.offset.y;
		float maxX = collider.size.x / 2.0f + collider.offset.x;
		float maxY = collider.size.y / 2.0f + collider.offset.y;

		//Set edges positions
		edges[0].points = new Vector2[] { new Vector2(minX, maxY),
										  new Vector2(minX, minY) };

		edges[1].points = new Vector2[] { new Vector2(minX, minY),
										  new Vector2(maxX, minY) };

		edges[2].points = new Vector2[] { new Vector2(maxX, minY),
										  new Vector2(maxX, maxY) };

		edges[3].points = new Vector2[] { new Vector2(maxX, maxY),
										  new Vector2(minX, maxY) };

		foreach (EdgeCollider2D edge in edges) {
			edge.enabled = false;
		}
	}
}
