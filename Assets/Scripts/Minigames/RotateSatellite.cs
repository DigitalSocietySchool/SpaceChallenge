﻿using UnityEngine;
using System.Collections;
using System;

public class RotateSatellite : MonoBehaviour 
{
    private float baseAngle = 0.0f;
    Quaternion newRotation;
    public LayerMask mask;
    [SerializeField]
    Controller2 controller;
    float rotation;
    public Material lineMaterial;
    [SerializeField]
    AudioClip laserhit;

    void Start()
    {
        GetComponent<LineRenderer>().SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
        //GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Additive"));
        GetComponent<LineRenderer>().material = lineMaterial;
        GetComponent<LineRenderer>().SetColors(Color.red, Color.red);

        var direction = transform.forward;
        direction = new Vector3(0, direction.z, 0);
        var endPoint = transform.position + direction * 13;
        endPoint = new Vector3(endPoint.x, endPoint.y, 0);
        GetComponent<LineRenderer>().SetPosition(1, endPoint);
    }

    void Update()
    {
        if(controller.resources[1] <= 50)
        {
            rotation = 0.4f + (controller.resources[1] / 200);
        }
        else
        {
            rotation = 1;
        }
        if (newRotation != null)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotation);
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * 50, Mathf.Infinity, mask);
        switch(hit.collider.name)
        {
            case "Earth":
                {
                    if (!GameObject.Find("SFXController").GetComponent<AudioSource>().isPlaying)
                    {
                        
                        GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(laserhit);
                    }
                    GameObject.Find("SFXController").GetComponent<AudioSource>().pitch = Mathf.Lerp(1, 2, 0.1f);
                    controller.charging = true;
                    controller.overcharging = false;
                    controller.theOneToCharge = 0;
                    break;
                }
            case "CriticalE":
                {
                    if (!GameObject.Find("SFXController").GetComponent<AudioSource>().isPlaying)
                    {
                        
                        GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(laserhit);
                    }
                    GameObject.Find("SFXController").GetComponent<AudioSource>().pitch = Mathf.Lerp(2, 1, 0.1f);
                    controller.charging = true;
                    controller.overcharging = true;
                    controller.theOneToCharge = 0;
                    break;
                }
            case "Asteroid":
                {
                    if (!GameObject.Find("SFXController").GetComponent<AudioSource>().isPlaying)
                    {
                        
                        GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(laserhit);
                    }
                    GameObject.Find("SFXController").GetComponent<AudioSource>().pitch = Mathf.Lerp(1, 2, 0.1f);
                    controller.charging = true;
                    controller.overcharging = false;
                    controller.theOneToCharge = 1;
                    break;
                }
            case "CriticalA":
                {
                    if (!GameObject.Find("SFXController").GetComponent<AudioSource>().isPlaying)
                    {
                        
                        GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(laserhit);
                    }
                    GameObject.Find("SFXController").GetComponent<AudioSource>().pitch = Mathf.Lerp(2, 1, 0.1f);
                    controller.overcharging = true;
                    controller.charging = true;
                    controller.theOneToCharge = 1;
                    break;
                }
            case "Sun":
                {
                    if (!GameObject.Find("SFXController").GetComponent<AudioSource>().isPlaying)
                    {
                        
                        GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(laserhit);
                    }
                    GameObject.Find("SFXController").GetComponent<AudioSource>().pitch = Mathf.Lerp(1, 2, 0.1f);
                    controller.charging = true;
                    controller.overcharging = false;
                    controller.theOneToCharge = 2;
                    break;
                }
            case "CriticalS":
                {
                    if (!GameObject.Find("SFXController").GetComponent<AudioSource>().isPlaying)
                    {
                        
                        GameObject.Find("SFXController").GetComponent<AudioSource>().PlayOneShot(laserhit);
                    }
                    
                    GameObject.Find("SFXController").GetComponent<AudioSource>().pitch = Mathf.Lerp(2, 1, 0.1f);
                    controller.overcharging = true;
                    controller.charging = true;
                    controller.theOneToCharge = 2;
                    break;
                }
            default:
                {
                    GameObject.Find("SFXController").GetComponent<AudioSource>().Stop();
                    controller.charging = false;
                    controller.overcharging = false;
                    controller.theOneToCharge = -1;
                    break;
                }
        }
    }

    void OnMouseDown()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos = Input.mousePosition - pos;
        baseAngle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        baseAngle -= Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
    }

    void OnMouseDrag()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        pos = Input.mousePosition - pos;
        float ang = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg - baseAngle;
        newRotation = Quaternion.AngleAxis(ang, Vector3.forward);
    }
}
