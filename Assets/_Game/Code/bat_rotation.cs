using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class bat_rotation : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Input.mousePosition);



    }
}
