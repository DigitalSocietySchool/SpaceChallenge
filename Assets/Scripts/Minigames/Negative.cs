﻿using UnityEngine;
using System.Collections;
using System;

public class Negative : MonoBehaviour 
{
    [SerializeField]
    Sprite[] allSprites;
    public float speedMultiplier = 1;

	void Start()
    {
        int numberToLoad = Convert.ToInt32(Mathf.Floor(UnityEngine.Random.Range(0, allSprites.Length)));
        GetComponent<SpriteRenderer>().sprite = allSprites[numberToLoad];

    }
	
	void Update () 
	{
        transform.position -= new Vector3(0.13f * speedMultiplier, 0, 0);
        if(transform.position.x < - 14)
        {
            Destroy(gameObject);
        }

        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<Player>().Hit();
            
            Destroy(gameObject);
        }
    }
}
