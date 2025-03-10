using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject partPrefab, parentObject;

    [SerializeField]
    [Range(1, 1000)]
    int length = 1;

    [SerializeField]
    float partDistance = 0.21f;

    [SerializeField]
    bool reset, spawn, snapfirst, snapLast;

    public GameObject shoulderRight;
    public GameObject handRight;
    public GameObject shoulderConnectorRight;
    public GameObject handConnectorRight;

    private void Start()
    {
        //spawn = true;
        Spawn();
        shoulderConnectorRight.GetComponent<Transform>().position = shoulderRight.GetComponent<Transform>().position;

        handConnectorRight.GetComponent<Transform>().position = handRight.GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawn)
        {
            Spawn();
            spawn = false;
        }



        if(reset)
        {
            foreach (GameObject tmp in GameObject.FindGameObjectsWithTag("Rope"))
            {
                Destroy(tmp);
            }

            reset = false;
        }

        if(spawn)
        {
            Spawn();
            spawn = false;
        }
    }

    public void Spawn()
    {
        shoulderRight = GameObject.Find("ShoulderRight");
        handRight = GameObject.Find("HandRight");

        int count = (int)(length / partDistance);

        for(int x = 0; x < count; x++)
        {
            GameObject tmp;

            tmp = Instantiate(partPrefab, new Vector3(transform.position.x, transform.position.y + (partDistance + (x + 1)), transform.position.z), Quaternion.identity, parentObject.transform);

            if (x == 0) shoulderConnectorRight = tmp;
            else if(x == count - 1) { handConnectorRight = tmp; }

            tmp.transform.eulerAngles = new Vector3(180, 0, 0);

            tmp.name = parentObject.transform.childCount.ToString();

            if(x == 0)
            {
                Destroy(tmp.GetComponent<CharacterJoint>());
            }
            else
            {
                tmp.GetComponent<CharacterJoint>().connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }
    }
}
