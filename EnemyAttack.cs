using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    [SerializeField] float range = 3f;
    [SerializeField] float timeBetweenAttacks = 1f;
     
    private Animator anim;
    private GameObject player;
    private bool playerInRange;
    private BoxCollider[] weaponColliders;
    private EnemyHealth enemyHealth;

    // Use this for initialization
    void Start () {
        weaponColliders = GetComponentsInChildren<BoxCollider> ();
        player = GameManager.instance.Player;
        anim = GetComponent<Animator> ();
        enemyHealth = GetComponent<EnemyHealth> ();
        StartCoroutine (Attack ());
    }

    // Update is called once per frame
    void Update () {
        if (Vector3.Distance (transform.position, player.transform.position) < range && enemyHealth.IsAlive) {
            playerInRange = true;
            RotateTowards (player.transform);
        } else {
            playerInRange = false;
        }
        anim.SetBool ("PlayerInRange", playerInRange);
    }

    IEnumerator Attack () {
        if (playerInRange && !GameManager.instance.GameOver) {
            if (this.gameObject.tag == "Soldier") {
                int randomAttack = Random.Range (1, 50);

                if (randomAttack > 10) {
                    anim.Play ("Attack");
                } else {
                    anim.Play ("FrenzyAttack");
                }

            } else {
                anim.Play ("Attack");
            }

            yield return new WaitForSeconds (timeBetweenAttacks);
        }
        yield return null;
        StartCoroutine (Attack ());
    }

    private void RotateTowards (Transform player) {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation (direction);
        transform.rotation = Quaternion.Slerp (transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    public void EnemyBeginAttack () {
        foreach (var weapon in weaponColliders) {
            weapon.enabled = true;
        }
    }

    public void EnemyEndAttack () {
        foreach (var weapon in weaponColliders) {
            weapon.enabled = false;
        }
    }

}