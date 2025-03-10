using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walker001 : MonoBehaviour
{
    public GameObject Body;
    public GameObject LeftLegTarget;
    public GameObject RightLegTarget;

    public GameObject Terrain;

    public Camera cam;
    public NavMeshAgent agent;

    private float terrainHeight;
    private Vector3 pos;

    private float leftLegOffset = 0.8f;
    private float rightLegOffset = 0.8f;

    public Transform lookDir;

    private Color colFWD;

    private Vector3 bodyPos;
    private Vector3 bodyPosOld;
    private Vector3 bodyPosPrev;

    private Vector3 interpolLeft;
    private Vector3 interpolRight;

    private Vector3 direction;
    private Vector3 directionOld;
    private float distance_travelled_per_tick;

    private float testZ;

    Vector3 body;

    Vector3 oldFootPosLeft;
    Vector3 oldFootPosRight;
    Vector3 newFootPosLeft;
    Vector3 newFootPosRight;
    Vector3 footPosCentre;

    Vector3 footOffset;

    bool leftDown;
    bool rightDown;

    float legMoveSpeed;

    LineRenderer path;

    private float distThreshold;
    private float distance_travelled;

    private float angle;

    private float lerp;
    private float stepHeight;

    // Start is called before the first frame update
    void Start()
    {
        lerp = 1.0f;
        stepHeight = 0.5f;

        distThreshold = 4.0f;
        leftDown = rightDown = false;
        legMoveSpeed = 0.01f;

        pos = new Vector3(0, 0, 0);
        agent.updateRotation = true;

        colFWD = new Color(1.0f, 1.0f, 0.0f);

        bodyPos = Body.transform.position;
        bodyPosPrev = bodyPos;
        bodyPosOld = bodyPos;

        distance_travelled_per_tick = 0;
        distance_travelled = 0;

        interpolLeft = new Vector3();
        interpolRight = new Vector3();

        testZ = 0;

        footOffset = new Vector3(0.0f, 1.0f, 0.0f);

        oldFootPosLeft = newFootPosLeft = LeftLegTarget.transform.position;
        oldFootPosRight = newFootPosRight = RightLegTarget.transform.position;

        legMoveSpeed = 2.0f;

    }

    private void FixedUpdate()
    {
 
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
        //MoveLegs();


        if (agent.path.corners.Length >= 2)
        {
            path = Body.GetComponent<LineRenderer>();
            path.positionCount = agent.path.corners.Length;
            // Debug.Log("Position count: " + path.positionCount);

            for (int n = 0; n < agent.path.corners.Length; ++n)
            {
                path.SetPosition(n, agent.path.corners[n]);
            }
        }
    }

    private float HorizontalDistance(Vector3 v1, Vector3 v2)
    {
        float dist = 0.0f;

        Vector3 v1s = v1;
        Vector3 v2s = v2;

        v1s.z = 0.0f;
        v2s.z = 0.0f;

        dist = (v1s - v2s).magnitude;

        return dist;
    }

    private void MoveLegs()
    {




        bodyPosPrev = bodyPos;
        bodyPos = Body.GetComponent<Transform>().position;
        distance_travelled_per_tick = (bodyPos - bodyPosPrev).magnitude;

        direction = (bodyPos - bodyPosPrev).normalized;

        Debug.DrawRay(bodyPos, direction * 1000.0f, colFWD);



        body = Body.GetComponent<Transform>().position;

        distance_travelled += distance_travelled_per_tick;

        if (distance_travelled > distThreshold)
        {
            Debug.Log("Distance travelled: " + distance_travelled);

            direction = (bodyPos - bodyPosOld).normalized;
            angle = Vector3.Angle(direction, directionOld);

            //Debug.Log(angle);

            bodyPosOld = bodyPos;

            oldFootPosLeft = newFootPosLeft;
            oldFootPosRight = newFootPosRight;

            //newFootPosLeft = oldFootPosLeft + distance_travelled * direction;
            //newFootPosRight = oldFootPosRight + distance_travelled * direction;

            newFootPosLeft = bodyPos;
            newFootPosRight = bodyPos;

            newFootPosLeft.x -= 2.0f;
            newFootPosRight.x += 2.0f;

            pos = newFootPosLeft;
            terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);

            pos.y = terrainHeight;
            newFootPosLeft = pos + footOffset;

            pos = newFootPosRight;
            terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);

            pos.y = terrainHeight;
            newFootPosRight = pos + footOffset;

            footPosCentre = (newFootPosLeft + newFootPosRight) / 2;

            Quaternion quat = Body.GetComponent<Transform>().rotation;

            newFootPosLeft = quat * (newFootPosLeft - footPosCentre) + footPosCentre;
            newFootPosRight = quat * (newFootPosRight - footPosCentre) + footPosCentre;

            newFootPosLeft += Body.GetComponent<Transform>().up * (2 + legMoveSpeed);
            newFootPosRight += Body.GetComponent<Transform>().up * 3;

            lerp = 0.0f;
            distance_travelled = 0.0f;

            directionOld = direction;
        }
        else if (lerp < 1.0f)
        {
            Debug.Log(lerp);
            
            interpolLeft = Vector3.Lerp(oldFootPosLeft, newFootPosLeft, lerp);
            interpolRight = Vector3.Lerp(oldFootPosRight, newFootPosRight, lerp);

            lerp += Time.deltaTime * legMoveSpeed;
            //Debug.Log("Lerp: " + lerp);

            pos = interpolLeft;
            terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);

            pos.y = terrainHeight;
            pos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            LeftLegTarget.GetComponent<Transform>().position = pos + footOffset;

            // Debug.Log("interpolLeft: " + interpolLeft + "lerp: " + lerp);

            pos = interpolRight;
            terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);

            pos.y = terrainHeight;
            pos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            RightLegTarget.GetComponent<Transform>().position = pos + footOffset;

        }
        else
        {
            pos = newFootPosLeft;
            terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);
            pos.y = terrainHeight;
            LeftLegTarget.GetComponent<Transform>().position = pos + footOffset;

            RightLegTarget.GetComponent<Transform>().position = newFootPosRight;

            pos = newFootPosRight;
            terrainHeight = Terrain.GetComponent<Terrain>().SampleHeight(pos);
            pos.y = terrainHeight;
            RightLegTarget.GetComponent<Transform>().position = pos + footOffset;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(newFootPosLeft, 1);
        Gizmos.DrawSphere(newFootPosRight, 1);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(oldFootPosLeft, 1);
        Gizmos.DrawSphere(oldFootPosRight, 1);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(LeftLegTarget.GetComponent<Transform>().position, 0.5f);
        Gizmos.DrawSphere(RightLegTarget.GetComponent<Transform>().position, 0.5f);
    }
}
