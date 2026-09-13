using System;
using UnityEngine;

public class OrbRemplirVie : MonoBehaviour
{
    public static event Action OnOrbVieAttrape;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnOrbVieAttrape?.Invoke();

            Destroy(gameObject);
        }
    }
}
