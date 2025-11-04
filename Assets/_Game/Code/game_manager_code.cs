using UnityEngine;

public class game_manager_code : MonoBehaviour
{


  // Singleton instance

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


}
