﻿using UnityEngine;

public class PlayerController : MonoBehaviour {
	private Rigidbody2D rb;
	private CameraController cc;
	private Transform foot;

	private Transform holding;
	private float holdingLocation;
    private EdgeCollider2D holdingCollider;
    private Vector2[] holdingPoses = new Vector2[2];
    private float armLength;
    private Transform arm;
    private float edgeLength;
    private float distArmToFoot;
    private bool climbing;
    private Vector2 holdingDir;
    private Vector2[] holdingDirs;
    EdgeCollider2D[] edges;
    private int currentEdge;

    private HingeJoint2D[] arms;
    private float armDist;
    public float changeArmDistance = 0.25f;
    private int currentArm = -1;

    public float movementSpeed = 2;
	public float climbSpeed = 2;
	public float jumpSpeed = 5;

	public float distanceToGroundBias = 0.05f;

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		cc = Camera.main.GetComponent<CameraController>();
		foot = transform.Find("Foot");

        arm = transform.Find("Sprites").Find("Player Torso").Find("Player Arm Front");
        armLength = arm.lossyScale.y;
        distArmToFoot = (arm.position - foot.position).magnitude;

        arms = new HingeJoint2D[] { transform.Find("Sprites").Find("Player Torso").Find("Player Arm Front").GetComponents<HingeJoint2D>()[1] , transform.Find("Sprites").Find("Player Torso").Find("Player Arm Back").GetComponents<HingeJoint2D>()[1] };
    }
	
	void FixedUpdate() {
		//Don't run if player doesn't have a rigidbody
		if (!rb) return;

		//If player is climbing, then do climb mechanics, else do the normal mechanics
		if (holding) {
			rb.simulated = false;

			//If the player releases the hold button, then stop holding
			if (!climbing) {
				holding = null;
                transform.position += (Vector3) (-holdingDir * (transform.lossyScale * (-holdingDir * GetComponent<BoxCollider2D>().size - GetComponent<BoxCollider2D>().offset)).magnitude * 1/2f);
                deRagdoll();
                return;
			}

			//Move player so she holds on
			float climb = Input.GetAxis("Vertical") * (climbSpeed * Time.fixedDeltaTime / edgeLength);

			holdingLocation += climb;
            holdingLocation = Mathf.Clamp01(holdingLocation);

            if (holdingLocation >= 1) {
                holdingLocation = 0;
                currentEdge--;
                if (currentEdge < 0) currentEdge = 3;

                holdingCollider = edges[currentEdge];
                holdingDir = holdingDirs[currentEdge];

                holdingPoses[0] = (holdingCollider.points[0] * holding.lossyScale) + (Vector2)holding.position;
                holdingPoses[1] = (holdingCollider.points[1] * holding.lossyScale) + (Vector2)holding.position;

                edgeLength = (holdingPoses[0] - holdingPoses[1]).magnitude;
            } else if (holdingLocation <= 0) {
                holdingLocation = 1;
                currentEdge++;
                if (currentEdge > 3) currentEdge = 0;

                holdingCollider = edges[currentEdge];
                holdingDir = holdingDirs[currentEdge];

                holdingPoses[0] = (holdingCollider.points[0] * holding.lossyScale) + (Vector2)holding.position;
                holdingPoses[1] = (holdingCollider.points[1] * holding.lossyScale) + (Vector2)holding.position;

                edgeLength = (holdingPoses[0] - holdingPoses[1]).magnitude;
            }

			Vector2 newPos = holdingPosition();

            //Check if character is going to end up inside something, if not, then move
            //Debug.Log(Physics2D.OverlapBoxAll(newPos, transform.GetComponent<BoxCollider2D>().size * transform.lossyScale, transform.rotation.z, ~LayerMask.GetMask("Player"))[0].name + ", " + Physics2D.OverlapBoxAll(newPos, transform.GetComponent<BoxCollider2D>().size * transform.lossyScale, transform.rotation.z, ~LayerMask.GetMask("Player"))[1].name);
            if (Physics2D.OverlapBoxAll(newPos, transform.GetComponent<BoxCollider2D>().size * transform.lossyScale, transform.rotation.z, ~LayerMask.GetMask("Player")).Length < 2) {
                transform.position = newPos;

                Debug.Log(currentArm);
                if (currentArm == -1) {
                    arms[0].connectedAnchor = newPos;
                    arms[1].connectedAnchor = newPos;

                    currentArm = 0;
                }

                arms[currentArm].connectedAnchor += (newPos - arms[currentArm].connectedAnchor) * 0.25f;
                armDist += Mathf.Abs(climb) * edgeLength;
                if (armDist > changeArmDistance) {
                    armDist = 0;
                    currentArm++;
                    if (currentArm >= arms.Length) currentArm = 0;
                }
            } else {
                holdingLocation -= climb;
            }

		} else {
			rb.simulated = true;
			//Calculate movement speed
			float move = Input.GetAxis("Horizontal") * movementSpeed;

			//Calculate player jump
			float vertical = 0f;
			if (Input.GetButtonDown("Jump")) {
				//Check if player is on ground
				RaycastHit2D hit = Physics2D.Raycast(foot.position + Vector3.down * 0.1f, Vector2.down, distanceToGroundBias);
                
				if (hit) {
					vertical += jumpSpeed;
				}
			}

			//Update velocity
			rb.velocity = move * Vector2.right + (rb.velocity.y + vertical) * Vector2.up;
		}
	}

    void Update() {
        if (Input.GetButtonDown("Climb")) climbing = true;
        if (Input.GetButtonUp("Climb")) climbing = false;

        Debug.DrawLine(transform.position, transform.position - transform.right, Color.red);
    }

    Vector2 holdingPosition() {
        return (holdingPoses[0]) * holdingLocation + (holdingPoses[1]) * (1f - holdingLocation);
    }

    private void ragdoll() {
        Transform torso = transform.Find("Sprites").Find("Player Torso");

        foreach (Rigidbody2D r in torso.GetComponentsInChildren<Rigidbody2D>()) {
            r.simulated = true;
        }

        foreach (BoxCollider2D c in torso.GetComponentsInChildren<BoxCollider2D>()) {
            c.enabled = true;
        }

        torso.GetComponent<Rigidbody2D>().simulated = true;
        torso.GetComponent<BoxCollider2D>().enabled = true;

        currentArm = -1;
    }

    private void deRagdoll() {
        Transform torso = transform.Find("Sprites").Find("Player Torso");

        foreach (Rigidbody2D r in torso.GetComponentsInChildren<Rigidbody2D>()) {
            r.simulated = false;
        }

        foreach (BoxCollider2D c in torso.GetComponentsInChildren<BoxCollider2D>()) {
            c.enabled = false;
        }

        torso.GetComponent<Rigidbody2D>().simulated = false;
        torso.GetComponent<BoxCollider2D>().enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        //Check if cllision is climbable and that the player is climbing
        if (collision.gameObject.CompareTag("Climbable")) {
			if (climbing) {
                holding = collision.transform;

                BoxCollider2D col = (BoxCollider2D) collision.collider;

                edges = collision.transform.GetComponents<EdgeCollider2D>();

                holdingDirs = new Vector2[] { col.transform.right, col.transform.up, -col.transform.right, -col.transform.up };

                if (Physics2D.BoxCast(col.transform.position, col.size, col.transform.rotation.z, -col.transform.right, 1f, 1 << LayerMask.NameToLayer("Player")).transform) {
                    holdingDir = col.transform.right;
                    currentEdge = 0;
                } else if (Physics2D.BoxCast(col.transform.position, col.size, col.transform.rotation.z, col.transform.right, 1f, 1 << LayerMask.NameToLayer("Player")).transform) {
                    holdingDir = -col.transform.right;
                    currentEdge = 2;
                } else if (Physics2D.BoxCast(col.transform.position, col.size, col.transform.rotation.z, -col.transform.up, 1f, 1 << LayerMask.NameToLayer("Player")).transform) {
                    holdingDir = col.transform.up;
                    currentEdge = 1;
                } else if (Physics2D.BoxCast(col.transform.position, col.size, col.transform.rotation.z, col.transform.up, 1f, 1 << LayerMask.NameToLayer("Player")).transform) {
                    holdingDir = -col.transform.up;
                    currentEdge = 3;
                }

                holdingCollider = edges[currentEdge];

                holdingPoses[0] = (holdingCollider.points[0] * holding.lossyScale) + (Vector2)holding.position;
                holdingPoses[1] = (holdingCollider.points[1] * holding.lossyScale) + (Vector2)holding.position;

                edgeLength = (holdingPoses[0] - holdingPoses[1]).magnitude;

                holdingLocation = (holdingPoses[1] - collision.collider.ClosestPoint(arm.position)).magnitude / edgeLength;

                ragdoll();
            }
		}
	}
}
