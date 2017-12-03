using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerAttack : MonoBehaviour
{
    [SerializeField]
    private float range = 10f;
    [SerializeField]
    private float timeBetweenAttacks = 1f;
    [SerializeField]
    Transform fireLocation;

    private Animator anim;
    private GameObject player;
    private bool playerInRange;
    private EnemyHealth enemyHealth;
    private GameObject arrow;

    // Use this for initialization
    void Start()
    {
        arrow = GameManager.instance.Arrow;
        player = GameManager.instance.Player;
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        StartCoroutine(Attack());
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < range && enemyHealth.IsAlive)
        {
            playerInRange = true;
            RotateTowards(player.transform);
        }
        else
        {
            playerInRange = false;
        }
        anim.SetBool("PlayerInRange", playerInRange);
    }

    IEnumerator Attack()
    {
        if (playerInRange && !GameManager.instance.GameOver)
        {
            anim.Play("Attack");
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
        yield return null;
        StartCoroutine(Attack());
    }

    private void RotateTowards(Transform player)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    public void FireArrow()
    {
        GameObject newArrow = Instantiate(arrow) as GameObject;
        newArrow.transform.position = fireLocation.position;
        newArrow.transform.rotation = transform.rotation;
        newArrow.GetComponent<Rigidbody>().velocity = transform.forward * 25;
    }
}
