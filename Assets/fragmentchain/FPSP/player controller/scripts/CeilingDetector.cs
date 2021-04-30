using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingDetector : MonoBehaviour
{
    public PlayerController playerController;

    public float distToCeiling;
    private float groundAngle;
    private float cosAngle;
    private float angleRad;
    public float angle;
    public bool canStand;
    public int triggerCount = 0;

    void Start()
    {
        playerController = this.transform.parent.GetComponent<PlayerController>();
    }

    void Update()
    {
        //Angle Detection
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.up, out hit, 20.0f, layerMask))
        {
            groundAngle = GetAngle(hit.normal);
            distToCeiling = hit.distance;
        }
        else
        {
            distToCeiling = 20.0f;
        }
        //Can stand check
        if (triggerCount <= 0) { canStand = true; }
        else { canStand = false; }
    }

    private float GetAngle(Vector3 normal)
    {
        cosAngle = Vector3.Dot(normal, Vector3.up);
        angleRad = Mathf.Acos(cosAngle);
        angle = (angleRad * 180 / Mathf.PI - 180) * -1;
        return angle;
    }
    void OnTriggerEnter(Collider other)
    {
        triggerCount++;
    }
    void OnTriggerExit(Collider other)
    {
        triggerCount--;
    }
}
