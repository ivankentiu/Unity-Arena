using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
    private Rigidbody body;

    private void Start () {
        body = GetComponent<Rigidbody> ();
    }
    
    void OnCollisionEnter (Collision other) {
        if (other.gameObject.CompareTag ("Player")) {
            transform.SetParent (other.transform);
            body.isKinematic = true;
            Destroy (gameObject, 3f);
        } else {
            Destroy (gameObject);
        }

    }

}