using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheRock : MonoBehaviour
{
    public Terrain terrain;
    
    float dist;

    public Vector3 windDirection;

    Vector3 agh;

    Rigidbody rb;

    Vector3 boink;


    // Start is called before the first frame update
    void Start()
    {
        windDirection = new Vector3(-1000.0f, 0.0f, 0.0f);

        rb = GetComponent<Rigidbody>();

        rb.AddForce(windDirection);

        boink = new Vector3(0.0f, 1000.0f, 0.0f);
    }

    void FixedUpdate()
    {
        Vector3 pos = rb.GetComponent<Transform>().position;
        float terrainHeight;

        terrainHeight = terrain.SampleHeight(pos);

        dist =  (pos.y - terrainHeight);

        if(dist < 0)
        {
            Debug.Log("boink");
            pos.y = terrainHeight;
            rb.GetComponent<Transform>().position = pos;
            rb.velocity = Vector3.zero;
            rb.AddForce(boink);
            rb.AddForce(windDirection);
            
        }
    }
}
