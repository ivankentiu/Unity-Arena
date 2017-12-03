using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUp : MonoBehaviour
{
    private GameObject player;
    private PlayerHealth PlayerHealth;

    // Use this for initialization
    void Start()
    {
        player = GameManager.instance.Player;
        PlayerHealth = player.GetComponent<PlayerHealth>();
		GameManager.instance.RegisterPowerUp();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            PlayerHealth.PowerUpHealth();
            Destroy(gameObject);
        }
    }
}
