using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class revised_player_controller : MonoBehaviour
{
    #region "Variables"

        #region "Movement and ground check"

            [Header("Movement Settings")]
            [SerializeField] private float max_player_velocity = 15f;
            [SerializeField] private float acceleration = 1f;

            [SerializeField] private float decceleration = 1f;

            [SerializeField] private float jump_force = 12f;
            private UnityEngine.Vector2 current_player_velocity = new UnityEngine.Vector2(0,0);

            [Header("Dash")]

            [SerializeField] private float dash_speed = 20f;
            [SerializeField] private float dash_time = 0.5f;
            private TrailRenderer trail_renderer;
            private Vector2 dashing_direction;
            private bool is_dashing;
            private bool can_dash = true;
            private float default_direction = 1.00f;


            [Header("Ground Check")]
            [SerializeField] private Transform groundCheck;
            [SerializeField] private float groundCheckRadius = 0.2f;
            [SerializeField] private LayerMask groundLayer;
            private bool is_grounded;
            
        #endregion

        #region "Bat swing and knockback"

            [Header("Bat swing")]
            [SerializeField] private Transform attack_transform;
            [SerializeField] private LayerMask attackable;
            private RaycastHit2D[] hits;

            [SerializeField] private float attack_cooldown = 0.5f;
            [SerializeField] private GameObject bat_pivot;
            private float attack_timer;
            private UnityEngine.Vector2 hit_force;

            [SerializeField] private Rigidbody2D knockback_target;
            [SerializeField] private GameObject attack_hitbox;
            [SerializeField] private float strength = 16;

            [SerializeField] private float attack_range_x, attack_range_y;
            [SerializeField] private float hit_divider = 2;
            

        #endregion

        #region "checkpoint and respawn"
        private UnityEngine.Vector3 respawn_point;                  //nice
        [SerializeField] private float respawn_time = 0.1f;         

        #endregion

        #region "Misc"

            private Rigidbody2D rb;
           private SpriteRenderer sprite;
           private Animator juan_animator;

        #endregion
    
    #endregion


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trail_renderer = GetComponent<TrailRenderer>();
        respawn_point = this.transform.position;
        sprite = GetComponent<SpriteRenderer>();
        juan_animator = GetComponent<Animator>();

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

        Gizmos.DrawWireCube(attack_transform.position,new UnityEngine.Vector2(attack_range_x,attack_range_y) );
    }

    void Update()
    {

        // Check if grounded
        is_grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);


        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump_force);
            //Debug.Log("THE SPACE KEY IS BEING PRESSED");
        }

        

        #region "Dash"

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


           // Debug.Log(dashing_direction);

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

        #endregion

        //bat code
        if (Input.GetMouseButton(0) && attack_timer >= attack_cooldown)
        {
            attack_timer = 0f;
            attack();
            //Debug.Log("Hit direction is " + hit_direction);

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
            sprite.flipX = false;
            juan_animator.SetBool("walking", true);

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
            sprite.flipX = true;
            juan_animator.SetBool("walking", true);


            if (current_player_velocity.x > -max_player_velocity)
            {
                current_player_velocity.x += acceleration * -1;
                default_direction = -1.00f;
                
                
                //Debug.Log("Acceleration IS happening TO THE LEFT");

            }

        }
        else
        {
            juan_animator.SetBool("walking", false);

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
        hits = Physics2D.BoxCastAll(attack_transform.position, new UnityEngine.Vector2(attack_range_x,attack_range_y), bat_pivot.transform.rotation.z, transform.forward, attackable);


        for (int i = 0; i < hits.Length; i++)
        {
            attackable_object attackable_Object = hits[i].collider.gameObject.GetComponent<attackable_object>();

            if (attackable_Object != null)
            {

                UnityEngine.Vector2 hit_direction = (transform.position - attack_hitbox.transform.position).normalized;
                hit_force = hit_direction * (((Math.Abs(rb.linearVelocityX) + Math.Abs(rb.linearVelocityY)) / hit_divider) + strength);
               
                rb.linearVelocityY = hit_force.y;
                current_player_velocity.x += hit_force.x;
                can_dash = true;

                //Debug.Log("Hit has been hitted");
            }
        }

    }

    #region "Respawn, death and checkpoints"
    void OnTriggerEnter2D(Collider2D colision_box)
    {
        if(colision_box.tag == "checkpoint")
        {
            respawn_point = this.transform.position;

            Debug.Log("this IS a checkpoint...");
            Debug.Log(respawn_point);
        }

        if(colision_box.tag == "harmfull")
        {
            
            
            StartCoroutine(DEATH());
        }

        if(colision_box.tag == "bounds")
        {
            
            
            StartCoroutine(DEATH());
        }
        
    }

    private IEnumerator DEATH()
    {
        yield return new WaitForSeconds(respawn_time);
        transform.position = respawn_point;
        current_player_velocity = new UnityEngine.Vector2(0,0);
        Debug.Log("times up");

    }

    
    #endregion







}
