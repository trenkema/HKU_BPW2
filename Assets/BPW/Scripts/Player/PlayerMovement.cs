using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;

    public LayerMask groundLayer;

    Vector3 velocity;

    bool isGrounded;

    public float x, z;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        //if (Input.GetButtonDown("Jump") && isGrounded == true)
        //{
        //    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        //}
    }

    void FixedUpdate()
    {
        velocity.y += gravity * Time.deltaTime;

        Vector3 move = transform.right * x + transform.forward * z;
        move = move * speed;
        move.y = velocity.y;
        controller.Move(move * Time.deltaTime);
    }
}
