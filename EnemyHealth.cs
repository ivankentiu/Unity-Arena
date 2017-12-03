using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int startingHealth = 20;
    [SerializeField]
    private float timeSinceLastHit = 0.5f;
    [SerializeField]
    private float dissapearSpeed = 2f;

    private Animator anim;
    private AudioSource audio;
    private float timer = 0f;
    private NavMeshAgent nav;
    private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
    private bool dissapearEnemy = false;
    private int currentHealth;
    private ParticleSystem blood;
    private bool isAlive;

    public bool IsAlive
    {
        get { return isAlive; }
    }
    // Use this for initialization
    void Start()
    {
        GameManager.instance.RegisterEnemy(this);
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        blood = GetComponentInChildren<ParticleSystem>();
        isAlive = true;
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (dissapearEnemy) {
            transform.Translate(-Vector3.up * dissapearSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (timer >= timeSinceLastHit && !GameManager.instance.GameOver)
        {
            if (other.tag == "PlayerWeapon")
            {
                TakeHit(); 
                blood.Play();
                timer = 0f;
            }
        }
    }

    void TakeHit()
    {
        if (currentHealth > 0)
        {
            audio.PlayOneShot(audio.clip);
            anim.Play("Hurt");
            currentHealth -= 10;      
        }

        if (currentHealth <= 0)
        {
            isAlive = false;
            KillEnemy();
        }
    }

    void KillEnemy()
    {
        GameManager.instance.KilledEnemy(this);
        capsuleCollider.enabled = false;
        nav.enabled = false;
        anim.SetTrigger("EnemyDie");
        rigidBody.isKinematic = true;

        StartCoroutine(RemoveEnemy());
    }

    IEnumerator RemoveEnemy()
    {
        // wait 4 seconds after enemy dies
        yield return new WaitForSeconds(4f);
        // start to sink the enemy
        dissapearEnemy = true;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
