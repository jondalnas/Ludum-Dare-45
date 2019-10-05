using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D rb;
	private CameraController cc;
	private Transform foot;

	private Transform holding;
	private Vector2 holdingLocation;
	private BoxCollider2D holdingCollider;
	private float armLength;

	public float movementSpeed = 2;
	public float climbSpeed = 2;
	public float jumpSpeed = 5;

	public float distanceToGroundBias = 0.05f;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		cc = Camera.main.GetComponent<CameraController>();
		foot = transform.Find("Foot");

		//armLength = transform.Find("Player Arm Front").lossyScale.y;
	}
	
	void FixedUpdate() {
		//Don't run if player doesn't have a rigidbody
		if (!rb) return;

		//If player is climbing, then do climb mechanics, else do the normal mechanics
		if (holding) {
			rb.simulated = false;

			//If the player releases the hold button, then stop holding
			if (!Input.GetButton("Climb")) {
				holding = null;
				return;
			}

			//Move player so she holds on
			float climb = Input.GetAxis("Horizontal") * climbSpeed * Time.fixedDeltaTime;

			holdingLocation = holdingCollider.ClosestPoint(holdingLocation + Vector2.up * climb);
			transform.position = holdingLocation;
		} else {
			rb.simulated = true;
			//Calculate movement speed
			float move = Input.GetAxis("Horizontal") * movementSpeed;

			//Calculate player jump
			float vertical = 0f;
			if (Input.GetButtonDown("Jump")) {
				//Check if player is on ground
				RaycastHit2D hit = Physics2D.Raycast(foot.position, Vector2.down, distanceToGroundBias);

				if (hit) {
					vertical += jumpSpeed;
				}
			}

			//Update velocity
			rb.velocity = move * Vector2.right + (rb.velocity.y + vertical) * Vector2.up;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		//Check if cllision is climbable and that the player is climbing
		if (collision.gameObject.CompareTag("Climbable")) {
			if (Input.GetButton("Climb")) {
				holding = collision.transform;
				holdingCollider = (BoxCollider2D) collision.collider;
				holdingLocation = collision.contacts[0].point;
			}
		}
	}
}
