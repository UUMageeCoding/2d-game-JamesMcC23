using System.Collections;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Events;

public class knockback : MonoBehaviour
{
    [SerializeField] private Rigidbody2D knockback_target;
    [SerializeField] private float strength = 16, delay = 0.15f;

    public UnityEvent OnBegin, OnDone;

    public void PlayFeedback(GameObject sender)
    {
        StopAllCoroutines();
        OnBegin?.Invoke();
        UnityEngine.Vector2 direction = (transform.position - sender.transform.position).normalized;
        knockback_target.AddForce(direction * strength, ForceMode2D.Impulse);
        StartCoroutine(Reset());
        
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(delay);
        knockback_target.linearVelocity = UnityEngine.Vector3.zero;
        OnDone?.Invoke();
    }


}
