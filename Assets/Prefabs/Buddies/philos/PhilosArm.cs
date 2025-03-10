using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnector : MonoBehaviour
{
    public GameObject Shoulder;
    public GameObject Hand;
    public GameObject Arm;
    LineRenderer line;

    int arm_num_segments;

    Vector3 linePos;
    Vector3 linePosOrig;

    Vector3 direction;
    Vector3 handShoulder;
    float distance;

    float randomDisplacement;
    float widthMultiplier;

    float randomThicknessMin;
    float randomThicknessMax;
    float randomThickness;

    AnimationCurve curve;
    Gradient gradient;

    float timePos;

    Color baseColor;
    Color fringeColor;
    Color col;

    public Light light;

    // Start is called before the first frame update
    void Start()
    {


        arm_num_segments = 8;
        line = Arm.GetComponent<LineRenderer>();
        line.positionCount = arm_num_segments;

        randomDisplacement = 0.030f;
        randomThicknessMax = 0.099f;
        randomThicknessMin = 0.003f;

        baseColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        fringeColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
        col = new Color(1.0f, 1.0f, 1.0f, 1.0f);


    }


    private void FixedUpdate()
    {
        line.startWidth = randomThicknessMin;
        line.endWidth = randomThicknessMin;
        
        curve = new AnimationCurve();
        gradient = new Gradient();

        var colors = new GradientColorKey[arm_num_segments];


        for (int i = 0; i < arm_num_segments; i++)
        {
            timePos = (float)i / (float)(arm_num_segments - 1);

            handShoulder = Hand.transform.position - Shoulder.transform.position;

            direction = handShoulder.normalized;

            distance = handShoulder.magnitude;

            linePos = Shoulder.transform.position + direction * distance * (timePos);

            if (i > 0 || i < arm_num_segments - 1)
            {


                linePosOrig = linePos;

                linePos.x += Random.Range(-randomDisplacement, randomDisplacement);
                linePos.y += Random.Range(-randomDisplacement, randomDisplacement);
                linePos.z += Random.Range(-randomDisplacement, randomDisplacement);
                randomThickness = Random.Range(randomThicknessMin, randomThicknessMax);

                col.r = col.g = 1.0f / (1.0f - 10 * (linePos - linePosOrig).magnitude);

                if (i == 4)
                {
                    light.GetComponent<Transform>().position = linePos;
                }
            }
            else
            {
                randomThickness = randomThicknessMin;
            }

            line.SetPosition(i, linePos);

            colors[i] = new GradientColorKey(col, timePos);
            

            curve.AddKey(timePos, randomThickness);

        }

        gradient.colorKeys = colors;

        line.widthCurve = curve;
        //line.colorGradient = gradient;

        // Update is called once per frame
        void Update()
    {
            // line.SetPosition(0, Shoulder.transform.position);
            // line.SetPosition(arm_num_segments - 1, Hand.transform.position);
        }
    }
}
