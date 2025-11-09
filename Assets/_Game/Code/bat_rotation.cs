using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class bat_rotation : MonoBehaviour
{
    [SerializeField] private Camera main_camera;
    [SerializeField] float angle;

    private UnityEngine.Vector2 mouse_position;

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(Input.mousePosition);

        mouse_position = Input.mousePosition;

        UnityEngine.Vector3 world_position = main_camera.ScreenToWorldPoint(new UnityEngine.Vector3(mouse_position.x, mouse_position.y, main_camera.nearClipPlane + 30));

        UnityEngine.Vector3 rotation_direction = (world_position - transform.position).normalized;
        rotation_direction.z = 0;

        float angle = Mathf.Atan2(rotation_direction.y, rotation_direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new UnityEngine.Vector3(0, 0, angle));


    }
    
}
