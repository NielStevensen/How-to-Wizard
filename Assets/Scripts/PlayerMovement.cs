using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        controller = GetComponent<CharacterController>();
        controller.Move(new Vector3 (Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical")));
    }
}
