using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class revised_player_controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float max_player_velocity = 15f;
    [SerializeField] private float acceleration = 1f;

    [SerializeField] private float decceleration = 1f;

    [SerializeField] private float jump_force = 12f;

    [SerializeField] private float dash_speed = 20f;


    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool is_grounded;
    private float move_input;
    private float current_player_velocity = 1f;

    private bool can_dash;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Set to Dynamic with gravity
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

     // Visualise ground check in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    /* misc functions */

    void Update()
    {
        // Get horizontal input
        move_input = Input.GetAxisRaw("Horizontal");

        // Check if grounded
        is_grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // update dash
        can_dash = is_grounded;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_force);
            //Debug.Log("THE SPACE KEY IS BEING PRESSED");
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && can_dash)
        {
            current_player_velocity += (dash_speed * move_input);
            Debug.Log("Dash is dashing");
        }
    }

    /*movement function*/

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D))
        {

            if (current_player_velocity < max_player_velocity)
            {
                current_player_velocity += acceleration;
                //Debug.Log("Acceleration IS happening TO THE RIGHT");
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            // Apply horizontal movement to the left


            if (current_player_velocity > -max_player_velocity)
            {
                current_player_velocity += acceleration * -1;
                //Debug.Log("Acceleration IS happening TO THE LEFT");
            }

        }
        else
        {
            if (is_grounded == true && Math.Abs(current_player_velocity) > 0)
            {
                current_player_velocity *= decceleration;
                //Debug.Log("Deceleration IS happening");
            }

        }

        rb.linearVelocity = new Vector2(current_player_velocity, rb.linearVelocity.y);

    }
    


}
