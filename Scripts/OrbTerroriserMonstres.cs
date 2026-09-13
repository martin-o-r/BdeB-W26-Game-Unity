using UnityEngine;

public class OrbTerroriserMonstres : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TerroriserTousLesMonstres();

            Destroy(gameObject);
        }
    }

    private void TerroriserTousLesMonstres()
    {
        GameObject[] monstres = GameObject.FindGameObjectsWithTag("Monstre");

        foreach (GameObject monstre in monstres)
        {
            // Apres 5 secondes de fuite, les monstre vont "s'echaper"
            Destroy(monstre, 5f);

            ComportementEnnemi comportement = monstre.GetComponent<ComportementEnnemi>();

            comportement.ChangerEtat(comportement.etatFuite);
        }
    }
}
