using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

/*
 * https://gamedevbeginner.com/events-and-delegates-in-unity/
 *      -> le publisher (observer pattern) doit avoir des events static et public afin qu'un autre script puisse y acceder sans utiliser de coupling
 *      -> utilisation de static event pour pouvoir acceder ainsi : ScriptAttaque.OnAmeliorer += / -=
 */

public class ScriptAttaque : MonoBehaviour
{
    private HashSet<ComportementEnnemi> ennemiSet = new();

    private float tempsEntreAttaque;
    public static float TempsEntreAttaque { get; private set; } // Pour attaque par boule de feu

    public float rayon = 2f;
    public int dommages = 5;
    public int niveauAttaque = 1;

    private SphereCollider sphereCollider;
    public static float SphereRayonInitiale {  get; private set; }
    private VisualEffect vfxManager;

    public static event Action<float> OnAmeliorerRayon;
    public static event Action<float> OnAmeliorerAttaque;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        SphereRayonInitiale = sphereCollider.radius;

        vfxManager = GetComponent<VisualEffect>();

        RecalculTempsAttaque();
        TempsEntreAttaque = tempsEntreAttaque;

        StartCoroutine(Attaque());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ComportementEnnemi ennemi))
        {
            ennemiSet.Add(ennemi);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ComportementEnnemi ennemi))
        {
            ennemiSet.Remove(ennemi);
        }
    }

    private IEnumerator Attaque()
    {
        while (true)
        {
            if (ennemiSet.Count == 0)
            {
                yield return new WaitForSeconds(tempsEntreAttaque);
                continue;
            }

            // Choisi un ennemi aleatoire
            int indexAleatoire = UnityEngine.Random.Range(0, ennemiSet.Count);
            ComportementEnnemi[] ennemiTable = ennemiSet.ToArray();
            ComportementEnnemi ennemiRandom = ennemiTable[indexAleatoire];

            // Si existe pas, on le retire et on recommence
            if (ennemiRandom == null)
            {
                ennemiSet.Remove(ennemiRandom);
                continue;
            }

            // Sinon on attaque l'ennemi
            ennemiRandom.PerdreVie(dommages);

            yield return new WaitForSeconds(tempsEntreAttaque);
        }
    }

    public void AmeliorerRayon(int rayonGagne)
    {
        rayon += rayonGagne;

        sphereCollider.radius = rayon;
        vfxManager.SetFloat("RayonAttaque", rayon / 2);

        OnAmeliorerRayon?.Invoke(sphereCollider.radius);
    }

    public void AmeliorerRapidite(int amelioration)
    {
        niveauAttaque += amelioration;
        RecalculTempsAttaque();
        TempsEntreAttaque = tempsEntreAttaque;

        OnAmeliorerAttaque?.Invoke(niveauAttaque);
    }

    // Amelioration de base pour les attaques, appele depuis ComportementPersonnage.AmeliorerDommageAttaqueDeBase()
    public void AmeliorerDommageDeBase(int amelioration)
    {
        dommages += amelioration;
    }

    private void RecalculTempsAttaque()
    {
        tempsEntreAttaque = 10.0f / Mathf.Sqrt(100.0f * (niveauAttaque));
    }
}