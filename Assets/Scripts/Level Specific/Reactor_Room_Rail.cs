using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor_Room_Rail : MonoBehaviour
{
    public Transform target = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Range z : -4,75 - 4,75
        //Range x : 12,75 - -43,6
        if (target != null) {
            float N = 0f;
            float lerpZ = Mathf.InverseLerp(-4.75f, 4.75f, target.position.z);
            if (target.position.x > -3) lerpZ *= -1f;

            float lerpX = Mathf.InverseLerp(12.75f, -43.6f, target.position.x);

            lerpZ *= (4.75f * 2f);
            if (target.position.x > -3) lerpZ += (4.75f * 2f);
            lerpX *= (12.75f + 43.6f);
            N = Mathf.InverseLerp(0f, (4.75f * 4f) + (12.75f + 43.6f), lerpZ + lerpX);

            GetComponent<BezierSolution.BezierWalkerManual>().NormalizedT = N;
        }
    }
}
