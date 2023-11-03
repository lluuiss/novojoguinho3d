using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coins : MonoBehaviour
{
    private BoxCollider col;

    private AudioSource sound;
    // Start is called before the first frame update

    private void Awake()
    {
        col = GetComponent<BoxCollider>();
        sound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Collider.gameObject.CompareTag(Player))
            sound.Play();
            col.enabled = false;
            GameController.gameController.SetCoins(1);
            Destroy(this.gameObject, 0.1f);
        
        }
    }
