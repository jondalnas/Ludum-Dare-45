using UnityEngine;

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
				return;
			}

			//Move player so she holds on
			float climb = Input.GetAxis("Vertical") * (climbSpeed * Time.fixedDeltaTime / edgeLength);

			holdingLocation += climb;
            holdingLocation = Mathf.Clamp01(holdingLocation);

            if (holdingLocation >= 1) {
                holding = null;
                transform.position += new Vector3(0.125f, distArmToFoot);
                climbing = false;
                return;
            }

			transform.position = holdingPosition();
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

    void Update() {
        if (Input.GetButtonDown("Climb")) climbing = true;
        if (Input.GetButtonUp("Climb")) climbing = false;
    }

    Vector2 holdingPosition() {
        return ((holdingCollider.points[0] * holding.lossyScale) + (Vector2)holding.position) * holdingLocation + ((holdingCollider.points[1] * holding.lossyScale) + (Vector2)holding.position) * (1f - holdingLocation);
    }

	void OnCollisionEnter2D(Collision2D collision) {
		//Check if cllision is climbable and that the player is climbing
		if (collision.gameObject.CompareTag("Climbable")) {
			if (climbing) {
				BoxCollider2D col = (BoxCollider2D) collision.collider;

                EdgeCollider2D[] eddges = GetComponentsInChildren<EdgeCollider2D>();



                if (Physics2D.BoxCast(col.transform.position, col.size * 0.9f, col.transform.rotation.z, Vector2.left, 1f, ~LayerMask.NameToLayer("Player"))) Debug.Log("Hello");

                holding = collision.transform;

                holdingPoses[0] = (holdingCollider.points[0] * holding.lossyScale) + (Vector2)holding.position;
                holdingPoses[1] = (holdingCollider.points[1] * holding.lossyScale) + (Vector2)holding.position;

                edgeLength = (holdingPoses[0] - holdingPoses[1]).magnitude;

                holdingLocation = (holdingPoses[0] - collision.collider.ClosestPoint(arm.position)).magnitude / edgeLength;
            }
		}
	}
}
