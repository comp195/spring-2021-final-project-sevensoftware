using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [Header("Mouse Sensitivity")]
        [Range(0.0f, 20.0f)]
    public float mouseSensitivity;
    public float initialMouseSensitivity;
        [Tooltip("Divide mouse sensitivity by this when sliding")] [Range(1.0f, 4.0f)]
    public float slideMouseSensMod;

    [Header("Rotation Clamping")]
    private Vector3 currentCamRotation;

    [Header("Wallrun Tilting")]
        [Range(0.0f, 1.0f)]
    public float tiltSpeed;
    public float alignSpeed;
    private float wallrunCamTiltCounter;
    private float camTiltDefaultCounter;
    private float wallrunCamAlignCounter;

    [Header("Vaulting")]
    private float vaultCamLerpCounter;

    [Header("Headbob")]
    public float headbobVerticalStrength;
    public float headbobHorizontalStrength;
    public float headbobSpeed;
    private float headbobTimer;
    private Vector3 initalCamLocalPos;
    private float camLocalPosReturnTimer;

    [Header("GFX")]
    public float speedLinesMaxSpeed;

    [Header("References")]
    private PlayerController controller;
    private Camera cam;
    private PlayerMovement movement;
    private ParticleSystem speedLines;



    void Start()
    {
        //References
        cam = Camera.main;
        controller = GetComponent<PlayerController>();
        movement = GetComponent<PlayerMovement>();
        speedLines = GameObject.Find("SpeedLines").GetComponent<ParticleSystem>();

        //Initial Values
        initialMouseSensitivity = mouseSensitivity;
        initalCamLocalPos = cam.transform.localPosition;
    }
    void Update()
    {
        if (!controller.isPaused)
        {
            CamRotationUnwrap();

            switch (controller.status)
            {
                case Status.walking:
                    mouseSensitivity = initialMouseSensitivity;
                    mouseSensToTimescale();
                    DefaultCameraUpdate();
                    Headbob();
                    break;

                case Status.wallrunning:
                    mouseSensitivity = initialMouseSensitivity;
                    ReturnToNeutralLocalPos();
                    mouseSensToTimescale();
                    WallrunCameraUpdate();
                    break;

                case Status.sliding:
                    mouseSensitivity = initialMouseSensitivity / slideMouseSensMod;
                    ReturnToNeutralLocalPos();
                    mouseSensToTimescale();
                    DefaultCameraUpdate();
                    break;

                default:
                    mouseSensitivity = initialMouseSensitivity;
                    ReturnToNeutralLocalPos();
                    mouseSensToTimescale();
                    DefaultCameraUpdate();
                    break;
            }

            SpeedLines();
        }
    }
    void DefaultCameraUpdate()//Moves the FPP camera according to player input
    {
        transform.Rotate(0f, (Input.GetAxis("Mouse X") * mouseSensitivity) * 100 * Time.deltaTime, 0f, Space.World); // Player body is rotated, cam inherits rotation

        //Camera Pitch Clamping system, might need a rework
        if(currentCamRotation.x >= -85 && currentCamRotation.x <= 70) 
        {
            cam.transform.Rotate(-(Input.GetAxis("Mouse Y") * mouseSensitivity) * 100 * Time.deltaTime, 0f, 0f);
        }
        else if (currentCamRotation.x >= -65)
        {
            if (Input.GetAxis("Mouse Y") > 0)
            {
                cam.transform.Rotate(-(Input.GetAxis("Mouse Y") * mouseSensitivity) * 100 * Time.deltaTime, 0f, 0f);
            }
        }
        else if (currentCamRotation.x <= 85)
        {
            if (Input.GetAxis("Mouse Y") < 0)
            {
                cam.transform.Rotate(-(Input.GetAxis("Mouse Y") * mouseSensitivity) * 100 * Time.deltaTime, 0f, 0f);
            }
        }

        return;
    }
    void WallrunCameraUpdate()
    {  
        cam.transform.Rotate(0f, (Input.GetAxis("Mouse X") * mouseSensitivity) * 100 * Time.deltaTime, 0f, Space.World); // Only cam is rotated, not the player body

        //Camera Pitch Clamping system, might need a rework
        if (currentCamRotation.x >= -85 && currentCamRotation.x <= 70)
        {
            cam.transform.Rotate(-(Input.GetAxis("Mouse Y") * mouseSensitivity) * 100 * Time.deltaTime, 0f, 0f);
        }
        else if (currentCamRotation.x >= -65)
        {
            if (Input.GetAxis("Mouse Y") > 0)
            {
                cam.transform.Rotate(-(Input.GetAxis("Mouse Y") * mouseSensitivity) * 100 * Time.deltaTime, 0f, 0f);
            }
        }
        else if (currentCamRotation.x <= 85)
        {
            if (Input.GetAxis("Mouse Y") < 0)
            {
                cam.transform.Rotate(-(Input.GetAxis("Mouse Y") * mouseSensitivity) * 100 * Time.deltaTime, 0f, 0f);
            }
        }

        return;
    }
    void mouseSensToTimescale()
    {
        mouseSensitivity = initialMouseSensitivity / Time.timeScale;
    }
    public IEnumerator WallrunCamTilt(Quaternion currRot, bool rightSide)
    {
        while (wallrunCamTiltCounter <= tiltSpeed)
        {
            cam.transform.localRotation = Quaternion.Slerp(
                currRot,                                            // From here
                Quaternion.Euler(new Vector3(                       //
                    currRot.eulerAngles.x,                          ///
                    currRot.eulerAngles.y,                          /// To here
                    rightSide ? 10.0f : -10.0f)),                   ///
                (wallrunCamTiltCounter / tiltSpeed));               // At this progress
            wallrunCamTiltCounter += Time.deltaTime;

            yield return null;
        }
        wallrunCamTiltCounter = 0;
        yield return null;
    }
    public IEnumerator WallrunCamAlign(Quaternion currRot, bool rightSide)
    {
        cam.transform.parent = null;
        while (wallrunCamAlignCounter <= alignSpeed)
        {
            cam.transform.position = transform.position;
            cam.transform.localRotation = Quaternion.Slerp(
                currRot,                                            // From here
                Quaternion.Euler(new Vector3(                       //
                    currRot.eulerAngles.x,                          ///
                    0,                                              /// To here
                    currRot.eulerAngles.z)),                        ///
                (wallrunCamTiltCounter / tiltSpeed));               // At this progress
            wallrunCamAlignCounter += Time.deltaTime;

            yield return null;
        }
        cam.transform.parent = this.transform;
        wallrunCamAlignCounter = 0;
        yield return null;
    }
    public IEnumerator CamTiltDefault(Quaternion currRot)
    {
        while (camTiltDefaultCounter <= tiltSpeed)
        {
            cam.transform.localRotation = Quaternion.Slerp(
                currRot,                                            // From here
                Quaternion.Euler(new Vector3(                       //
                    currRot.eulerAngles.x,                          ///
                    0.0f,                                           /// To here
                    0.0f)),                                         ///
                (camTiltDefaultCounter / tiltSpeed));               // At this progress
            camTiltDefaultCounter += Time.deltaTime;

            yield return null;
        }
        cam.transform.localRotation = Quaternion.Euler(new Vector3(currRot.eulerAngles.x, 0.0f, 0.0f));
        camTiltDefaultCounter = 0;
        yield return null;
    }
    public IEnumerator VaultCamLerp(Vector3 currPos, Vector3 targetPos, float lerpSpeed, Vector3 playerTargetPos)
    {
        while (vaultCamLerpCounter <= lerpSpeed)
        {
            cam.transform.position = Vector3.Slerp(currPos, targetPos, (vaultCamLerpCounter / lerpSpeed));
            vaultCamLerpCounter += Time.deltaTime;

            yield return null;
        }
        cam.transform.position = targetPos; // Assure target is reached
        vaultCamLerpCounter = 0;
        cam.transform.parent = null;
        movement.Vault(playerTargetPos);
        cam.transform.parent = this.transform;
        yield return null;
    }
    void Headbob()
    {
        cam.transform.localPosition = new Vector3(initalCamLocalPos.x + Mathf.Sin(headbobTimer / 2) * headbobHorizontalStrength, initalCamLocalPos.y + Mathf.Sin(headbobTimer) * headbobVerticalStrength, initalCamLocalPos.z);

        if (Mathf.Cos(headbobTimer) <= 0)
        {
        headbobTimer += Time.deltaTime * headbobSpeed;
        }
        else
        {
            headbobTimer += Time.deltaTime * headbobSpeed / 1.5f;
        }
    }
    void ReturnToNeutralLocalPos()
    {
        if (cam.transform.localPosition != initalCamLocalPos)
        {
            cam.transform.localPosition = Vector3.Slerp(cam.transform.localPosition, initalCamLocalPos, camLocalPosReturnTimer);
            camLocalPosReturnTimer += Time.deltaTime * 0.2f;
        }
        else
        {
            camLocalPosReturnTimer = 0.0f;
        }
    }
    void CamRotationUnwrap()
    {
        currentCamRotation = cam.transform.localRotation.eulerAngles;
        if (currentCamRotation.x > 180) currentCamRotation.x = currentCamRotation.x - 360;
    }
    void SpeedLines()
    {
        float lineAlpha = controller.horizontalSpeed / speedLinesMaxSpeed;
        ParticleSystem.MainModule psmain = speedLines.main;
        psmain.startColor = new Color(1, 1, 1, lineAlpha);
    }
}