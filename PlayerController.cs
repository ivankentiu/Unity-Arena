using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] LayerMask layerMask;
    private CharacterController characterController;
    private Vector3 currentLookTarget = Vector3.zero;
    private Animator anim;
    private BoxCollider[] swordColliders;
    private GameObject fireTrail;
    private ParticleSystem fireTrailParticles;
    private GameObject attachedArrow;
    private float timeBetweenAttacks = 1f;
    private float timer = 0f;
    private bool canMove;

    // Use this for initialization
    void Start () {
        fireTrail = GameObject.FindWithTag ("Fire") as GameObject;
        fireTrail.SetActive (false);
        characterController = GetComponent<CharacterController> ();
        anim = GetComponent<Animator> ();
        swordColliders = GetComponentsInChildren<BoxCollider> ();
        canMove = true;
    }

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;

        if (!GameManager.instance.GameOver) {
            Vector3 moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
            if (canMove) {
                characterController.SimpleMove (moveDirection * moveSpeed);
            }

            if (moveDirection != Vector3.zero) {
                anim.SetBool ("IsWalking", true);
            } else {
                anim.SetBool ("IsWalking", false);
            }

            if (Input.GetMouseButtonDown (0) && timer >= timeBetweenAttacks) {
                anim.Play ("DoubleChop");
                timer = 0f;
            }

            if (Input.GetMouseButtonDown (1) && timer >= timeBetweenAttacks) {
                anim.Play ("SpinAttack");
                canMove = false;
                characterController.detectCollisions = false;
                timer = 0f;
                foreach (Transform child in transform) {
                    if (child.tag == "Arrow") {
                        Destroy (child.gameObject);
                    }
                }
            }
        }

    }

    void FixedUpdate () {
        if (!GameManager.instance.GameOver) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

            Debug.DrawRay (ray.origin, ray.direction * 500, Color.blue);
            if (Physics.Raycast (ray, out hit, 500, layerMask, QueryTriggerInteraction.Ignore)) {
                if (hit.point != currentLookTarget) {
                    currentLookTarget = hit.point;
                }

                Vector3 targetPosition = new Vector3 (hit.point.x, transform.position.y, hit.point.z);
                Quaternion rotation = Quaternion.LookRotation (targetPosition - transform.position);

                if (canMove) {
                    transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * 10f);
                }
            }
        }

    }

    public void BeginAttack () {
        foreach (var weapon in swordColliders) {
            weapon.enabled = true;
        }
    }

    public void EndAttack () {
        foreach (var weapon in swordColliders) {
            weapon.enabled = false;
        }
    }

    public IEnumerator AfterJumpAttack () {
        foreach (var weapon in swordColliders) {
            weapon.enabled = false;
        }
        yield return new WaitForSeconds (0.7f);
        transform.position += transform.forward * 1;
        characterController.detectCollisions = true;
        canMove = true;
    }

    public void SpeedPowerUp () {
        StartCoroutine (fireTrailRoutine ());
    }

    IEnumerator fireTrailRoutine () {
        fireTrail.SetActive (true);
        moveSpeed = 10f;
        yield return new WaitForSeconds (10f);
        moveSpeed = 6f;
        fireTrailParticles = fireTrail.GetComponent<ParticleSystem> ();
        var emission = fireTrailParticles.emission;
        emission.enabled = false;
        yield return new WaitForSeconds (3f);
        emission.enabled = true;
        fireTrail.SetActive (false);
    }

}