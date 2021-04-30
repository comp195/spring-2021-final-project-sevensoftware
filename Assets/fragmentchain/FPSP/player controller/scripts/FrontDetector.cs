using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDetector : MonoBehaviour
{
    
    public float obstacleMaxDistance;
    public float obstacleMinAngle;
    public float distanceToObstacle;
    public int precision = 6;
    private float obstacleMaxHeight = 3f;
    private float subdivision;
    private int rayCount;
    public bool[] obstacleZone;
    public float obstacleHeightInWorld;
    public float obstacleHeightFromPlayer;

    public bool obstacleDetected;

    public float heightCheckDistFromPlayer;
    private bool heightChecked;

    public Vector3 pointOnObstacle;

    private Vector3 rayPosition;

    private float cosAngle;
    private float angleRad;
    private float angle;

    public float angleToPlayer;
    public float wallAngle;

    public Transform playerLowPoint;

    void Start()
    {
        subdivision = obstacleMaxHeight / precision;
        obstacleZone = new bool[precision + 1];
        playerLowPoint = gameObject.transform.GetChild(0);
    }

    void Update()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;

        //reset values each update
        rayPosition = this.transform.position;
        rayPosition.y -= subdivision;
        obstacleHeightInWorld = 0;
        obstacleHeightFromPlayer = 0;
        rayCount = 0;
        distanceToObstacle = 0;
        Populate<bool>(obstacleZone, false);
        pointOnObstacle = Vector3.zero;

        obstacleDetected = false;
        heightChecked = false;

        for (float i = 0; i <= obstacleMaxHeight; i += subdivision)
        {
            rayPosition.y += subdivision;
            Debug.DrawRay(rayPosition, transform.TransformDirection(Vector3.forward));
            if (Physics.Raycast(rayPosition, transform.TransformDirection(Vector3.forward), out hit, obstacleMaxDistance, layerMask))
            {
                obstacleDetected = true;
                if (GetAngleToPlane(hit.normal, Vector3.down) > obstacleMinAngle)
                {
                    obstacleZone[rayCount] = true;
                    distanceToObstacle = hit.distance / this.transform.localScale.y;
                    angleToPlayer = GetAngleToPlane(hit.normal, transform.TransformDirection(Vector3.forward));
                    wallAngle = GetWallAngle(hit.normal);
                }
            }
            else if (Physics.Raycast(rayPosition + transform.forward * (distanceToObstacle + 0.08f), transform.TransformDirection(Vector3.down), out hit, 5, layerMask) && !heightChecked && obstacleDetected)
            {
                Debug.DrawRay(rayPosition + transform.forward * (distanceToObstacle + 0.08f), transform.TransformDirection(Vector3.down));
                obstacleHeightInWorld = rayPosition.y - hit.distance;
                obstacleHeightFromPlayer = rayPosition.y - hit.distance - playerLowPoint.transform.position.y;
                heightChecked = true;
                pointOnObstacle = hit.point;
            }
            rayCount++;
        }
        if (obstacleZone[precision] && obstacleHeightInWorld <= 0)
        {
            obstacleHeightInWorld = obstacleMaxHeight;
            obstacleHeightFromPlayer = obstacleMaxHeight;
        }
    }
    public void Populate<T>(T[] arr, T value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = value;
        }
    }
    private float GetAngleToPlane(Vector3 planeNormal, Vector3 approachingRay)
    {
        cosAngle = Mathf.Abs(Vector3.Dot(planeNormal, approachingRay));
        angleRad = Mathf.Acos(cosAngle);
        angle = (angleRad * 180 / Mathf.PI);
        return angle;
    }
    private float GetWallAngle( Vector3 planeNormal)
    {
        cosAngle = Mathf.Abs(Vector3.Dot(planeNormal, Vector3.up));
        angleRad = Mathf.Acos(cosAngle);
        angle = (angleRad * 180 / Mathf.PI);
        return angle;
    }
    private void DetermineVaultLanding()
    {

    }
}
