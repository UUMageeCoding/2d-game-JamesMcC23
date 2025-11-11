using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class bat_swing : MonoBehaviour
{
    [SerializeField] private Transform attack_transform;
    [SerializeField] private float attack_range = 1.5f;
    [SerializeField] private LayerMask attackable;
    private RaycastHit2D[] hits;

    [SerializeField] private float attack_cooldown = 0.5f;
    [SerializeField] private Object bat_pivot;
    private float attack_timer;

    [SerializeField] private Rigidbody2D knockback_target;
    [SerializeField] private GameObject attack_hitbox;
    [SerializeField] private float strength = 16;


    private void Start()
    {
        attack_timer = attack_cooldown;

    }




    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && attack_timer >= attack_cooldown)
        {
            attack_timer = 0f;
            attack();

        }

        attack_timer += Time.deltaTime;
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
                UnityEngine.Vector2 direction = (transform.position - attack_hitbox.transform.position).normalized;
                knockback_target.AddForce(direction * strength, ForceMode2D.Impulse);
                Debug.Log(direction);

                Debug.Log("Hit has been hitted");
            }
        }

    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attack_transform.position, attack_range);
    }

}
