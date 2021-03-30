using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterNav : MonoBehaviour
{

    public GameObject target;
    public Rigidbody rb;
    public const int MOVEMENT_SPEED = 5;

    public GameObject spotlight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vectorToTarget = (target.transform.position - transform.position).normalized;
        Vector3 vectorToTargetNoY = new Vector3(vectorToTarget.x, 0, vectorToTarget.z);
        transform.position += vectorToTargetNoY * MOVEMENT_SPEED * Time.deltaTime;
        Quaternion heliLookRotation = Quaternion.LookRotation(vectorToTargetNoY);
        transform.rotation = Quaternion.Slerp(transform.rotation, heliLookRotation, Time.deltaTime);

        Quaternion spotlightRotation = Quaternion.LookRotation(vectorToTarget);
        spotlight.transform.rotation = spotlightRotation;
    }
}
