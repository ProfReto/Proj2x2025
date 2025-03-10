using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Walker001Foot : MonoBehaviour
{
    public GameObject Body;
    public GameObject Terrain;
    public GameObject target;

    //float distance_travelled_per_tick;
    //float distance_travelled;

    Vector3 bodyPos;

    Vector3 interpol;

    Vector3 oldFootPos;
    Vector3 newFootPos;


    Vector3 pos;

    Vector3 footOffset;

    public float xOffset;

    float angle;
    float distThreshold;
    float terrainHeight;
    float legMoveSpeed;

    float stepLength;

    float lerp;
    float stepHeight;

    public float init_offset;

    // Start is called before the first frame update
    void Start()
    {
        //distance_travelled = distance_travelled_per_tick = 0;
        distThreshold = 4.5f;

        stepHeight = 0.5f;


        footOffset = new Vector3(0.0f, 1.0f, 0.0f);

        legMoveSpeed = 2.0f;

        newFootPos = bodyPos;
        newFootPos.x += xOffset;
        terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);
        newFootPos.y = terrainHeight;

        newFootPos.z += init_offset;

        oldFootPos = newFootPos = newFootPos + footOffset;

        stepLength = 1.0f;

        lerp = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        bodyPos = Body.GetComponent<Transform>().position;

        target.GetComponent<Transform>().position = oldFootPos;

        newFootPos = bodyPos;
        newFootPos += Body.GetComponent<Transform>().right * xOffset;
        terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);
        newFootPos.y = terrainHeight;
        newFootPos += Body.GetComponent<Transform>().forward * stepLength + footOffset;
                
        if ((newFootPos - oldFootPos).magnitude > distThreshold && lerp >= 1.0f)
        {
            lerp = 0.0f;
        }
        else if (lerp < 1.0f)
        {
            interpol = Vector3.Lerp(oldFootPos, newFootPos, lerp);

            lerp += Time.deltaTime * legMoveSpeed;

            pos = interpol;
            terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);

            pos.y = terrainHeight;
            pos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            target.GetComponent<Transform>().position = pos + footOffset;

            if (lerp >= 1.0f) 
            {
                oldFootPos = newFootPos;
            }
        }

        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(newFootPos, 1);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(oldFootPos, 1);

    }
}

