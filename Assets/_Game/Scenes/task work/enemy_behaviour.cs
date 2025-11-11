using UnityEngine;

public class enemy_behaviour : MonoBehaviour
{
 public Transform playerPosition;
    Vector2 directionToPlayer;
    int EnemySpeed = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { }

    // Update is called once per frame
    void Update()
    {
        // get direction to player
        if (playerPosition != null)
        {
            directionToPlayer = (playerPosition.position - transform.position).normalized;
        }

        if (playerPosition.position - transform.position != Vector3.zero) MoveToPlayer(EnemySpeed);

    }

    public void MoveToPlayer(float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, playerPosition.position, speed * Time.deltaTime);
    }
}
