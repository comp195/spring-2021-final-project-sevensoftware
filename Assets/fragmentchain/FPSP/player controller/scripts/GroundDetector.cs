using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    public PlayerController playerController;

    public float groundAngle;
    private float cosAngle;
    private float angleRad;
    public bool isGrounded;
    public float distToGround;

    void Start()
    {
        playerController = this.transform.parent.GetComponent<PlayerController>();
    }

    void Update()
    {
        isGrounded = false;
        //Angle Detection
        int layerMask = 1 << 9;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, 20.0f, layerMask))
        {
            if(hit.distance <= 0.25f)
            {
                isGrounded = true;
                GetAngle(hit.normal);
            }
            distToGround = hit.distance;
        }
        else
        {
            distToGround = 20.0f;
        }
    }

    private float GetAngle(Vector3 normal)
    {
        cosAngle = Vector3.Dot(normal, Vector3.down);
        angleRad = Mathf.Acos(cosAngle);
        groundAngle = (angleRad * 180 / Mathf.PI - 180) * -1;
        return groundAngle;
    }
}
