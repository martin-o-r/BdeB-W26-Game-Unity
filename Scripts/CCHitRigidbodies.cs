using UnityEngine;

public class CCHitRigidbodies : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForceAtPosition(-hit.normal * 3, hit.point);
        }
    }
}
