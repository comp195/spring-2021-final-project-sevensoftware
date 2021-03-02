using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private void Awake() {
        // controls.Player.Jump.performed += _ => Jump();
    }

    void Jump() {
        Debug.Log("jump");
    }
    
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     float mouseX = Input.GetAxis("Mouse X");
    // }
}
