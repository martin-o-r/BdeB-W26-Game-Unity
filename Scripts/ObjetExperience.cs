using UnityEngine;

public class ObjetExperience : MonoBehaviour
{
    [SerializeField]
    private Material[] materials;

    [HideInInspector] 
    public int experienceGagne = 5;

    private void Start()
    {
        GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ComportementPersonnage personnage = other.gameObject.GetComponent<ComportementPersonnage>();
            personnage.GagnerVies(1);
            personnage.GagnerExperience(experienceGagne);
            Destroy(gameObject);
        }
    }
}
