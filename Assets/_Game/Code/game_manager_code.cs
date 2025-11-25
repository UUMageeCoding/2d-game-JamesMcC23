using Unity.Cinemachine;
using UnityEngine;

public class game_manager_code : MonoBehaviour
{


  [SerializeField] CinemachineConfiner2D main_cinemachine_camera;
  Collider2D camera_bound_var;

  private static game_manager_code _instance;

  

  public static game_manager_code Instance

  {

    get { return _instance; }

  }

  

  void Awake()

  {

    // Ensure only one instance exists

    if (_instance == null)

    {

      _instance = this;

      DontDestroyOnLoad(gameObject);

    }

    else

    {

      Destroy(gameObject);

    }

  } 

public void current_camera_bounds(Collider2D camera_bound)
    {
        camera_bound_var = camera_bound;
        
    }


    void Update()
    {
//        main_cinemachine_camera.BoundingShape2D = camera_bound_var;
    }

    



}
