using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeRotate : MonoBehaviour
{
    public const float ROTATION_SPEED = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right * (ROTATION_SPEED * Time.deltaTime));
    }
}
