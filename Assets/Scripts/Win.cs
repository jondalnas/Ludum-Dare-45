using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Win : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            GameObject.FindGameObjectWithTag("Win").GetComponent<Text>().enabled = true;
        }
    }
}
