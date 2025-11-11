using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class revised_player_controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float max_player_velocity = 15f;
    [SerializeField] private float acceleration = 1f;

    [SerializeField] private float decceleration = 1f;

    [SerializeField] private float jump_force = 12f;

    [SerializeField] private float dash_speed = 20f;
    [SerializeField] private float dash_time = 0.5f;


    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Bat swing")]
    [SerializeField] private Transform attack_transform;
    [SerializeField] private float attack_range = 1.5f;
    [SerializeField] private LayerMask attackable;
    private RaycastHit2D[] hits;

    [SerializeField] private float attack_cooldown = 0.5f;
    [SerializeField] private UnityEngine.Object bat_pivot;
    private float attack_timer;
    private UnityEngine.Vector2 hit_force;

    [SerializeField] private Rigidbody2D knockback_target;
    [SerializeField] private GameObject attack_hitbox;
    [SerializeField] private float strength = 16;

    private TrailRenderer trail_renderer;

    private Rigidbody2D rb;
    private bool is_grounded;
    private float move_input;
    private UnityEngine.Vector2 current_player_velocity = new UnityEngine.Vector2(0,0);

    private Vector2 dashing_direction;
    private bool is_dashing;
    private bool can_dash = true;
    private float default_direction = 1.00f;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail_renderer = GetComponent<TrailRenderer>();

        // Set to Dynamic with gravity
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        attack_timer = attack_cooldown;
    }

     // Visualise ground check in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        Gizmos.DrawWireSphere(attack_transform.position, attack_range);
    }

    /* misc functions */

    void Update()
    {
        // Get horizontal input
        move_input = Input.GetAxisRaw("Horizontal");

        // Check if grounded
        is_grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // update dash
        //can_dash = is_grounded;

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_force);
            //Debug.Log("THE SPACE KEY IS BEING PRESSED");
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && can_dash /*&& is_grounded == false*/)
        {
            is_dashing = true;
            can_dash = false;
            trail_renderer.emitting = true;
            if (Input.GetAxisRaw("Horizontal") == 0.00f && Input.GetAxisRaw("Vertical") == 0.00f)
            {
                dashing_direction = new Vector2(default_direction, Input.GetAxisRaw("Vertical"));
            }
            else
            {
                dashing_direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            }


            Debug.Log(dashing_direction);

            StartCoroutine(stop_dashing());
            
            




        }
        

        if (is_dashing)
        {
            rb.linearVelocity = dashing_direction.normalized * dash_speed;
            return;
        }
        if (is_grounded)
        {
            can_dash = true;
        }

        //bat code
        if (Input.GetMouseButton(0) && attack_timer >= attack_cooldown)
        {
            attack_timer = 0f;
            //attack();

            UnityEngine.Vector2 hit_direction = (transform.position - attack_hitbox.transform.position).normalized;
            rb.linearVelocity = hit_direction * (((Math.Abs(rb.linearVelocityX) + Math.Abs(rb.linearVelocityY)) / 2) + strength);

            Debug.Log("linear Velocity is " + rb.linearVelocity);
            Debug.Log("Hit direction is " + hit_direction);

        }
        else
        {
            hit_force = new UnityEngine.Vector2(0,0);
        }

        attack_timer += Time.deltaTime;

    
    }
    
    //stop dashing
    private IEnumerator stop_dashing()
    {
        yield return new WaitForSeconds(dash_time);
        trail_renderer.emitting = false;
        is_dashing = false;
        if(dashing_direction == new UnityEngine.Vector2(0,1))
        {
            current_player_velocity = new UnityEngine.Vector2(0,0);
        }
        
        
        
    }

    /*movement function*/

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D))
        {

            if (current_player_velocity.x < max_player_velocity)
            {
                current_player_velocity.x += acceleration;
                default_direction = 1.00f;
            
                //Debug.Log("Acceleration IS happening TO THE RIGHT");

            }

            
        }
        else if (Input.GetKey(KeyCode.A))
        {
            // Apply horizontal movement to the left


            if (current_player_velocity.x > -max_player_velocity)
            {
                current_player_velocity.x += acceleration * -1;
                default_direction = -1.00f;
                
                
                //Debug.Log("Acceleration IS happening TO THE LEFT");

            }

        }
        else
        {
            if (is_grounded == true && Math.Abs(current_player_velocity.x) > 0)
            {
                current_player_velocity.x *= decceleration;
                //Debug.Log("Deceleration IS happening");
            }

        }

        //cause of my suffering

        if (is_dashing == false)
        {
            
           rb.linearVelocityX = current_player_velocity.x;
        }






    }
    private void attack()
    {
        hits = Physics2D.CircleCastAll(attack_transform.position, attack_range, transform.right, 0f, attackable);


        for (int i = 0; i < hits.Length; i++)
        {
            attackable_object attackable_Object = hits[i].collider.gameObject.GetComponent<attackable_object>();

            if (attackable_Object != null)
            {
                //player knockback code go here :)


                UnityEngine.Vector2 hit_direction = (transform.position - attack_hitbox.transform.position).normalized;
                //knockback_target.AddForce(hit_direction * strength, ForceMode2D.Impulse);
                rb.linearVelocity = hit_direction * (((Math.Abs(rb.linearVelocityX) + Math.Abs(rb.linearVelocityY))/2) + strength);
                Debug.Log(rb.linearVelocity);

                Debug.Log("Hit has been hitted");
            }
        }

    }


    


}
