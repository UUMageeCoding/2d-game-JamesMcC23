using UnityEngine;
using UnityEngine.Events;

public class transition_trigger : MonoBehaviour
{
    
    [SerializeField] Collider2D parent_bounds;
    [SerializeField] GameObject player_character;


    /*void OnTriggerEnter(Collider other)
    {
        if (other == player_character)
        {
            game_manager_code.Instance.current_camera_bounds(parent_bounds);
            Debug.Log("trigger happened");


        }
        
    }*/
    void OnTriggerEnter2D(Collider2D collision)
    {
        ///game_manager_code.Instance.current_camera_bounds(parent_bounds);
        Debug.Log("trigger happened");
        
        Debug.Log(collision.gameObject.name);
    }

}


