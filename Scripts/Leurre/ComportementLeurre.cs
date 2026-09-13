using System;
using System.Collections;
using UnityEngine;

/*
 * "Bug" rencontre : 
 *      -> lorsque le leurre apparait, mais que certains monstres sont trop loin, ceux-ci ne se dirigent pas vers le leurre
 *      -> alors le leurre reste sur la scene tout le long du jeu
 * Idee de solution : 
 *      -> un destroy avec un compte a rebour pour detruite le leure apres un certain temps, s'il n'est pas detruit avant par un monstre
 *      
 * https://learn.unity.com/tutorial/invoke-2d
 * https://docs.unity3d.com/6000.4/Documentation/ScriptReference/MonoBehaviour.Invoke.html
 *      -> Invoke(nameof(nomMethode), float)
 *      -> nomMethode doit etre une methode de la classe qui n'a pas de parametres et qui retourne un type void
 *      -> la methode appele va s'activer apres le delais indique dans le Invoke()
 *      -> dans notre cas :
 *          - des qu'un leurre est cree, donc des que ce script-ci est active, on lance Involke() pour detruire le leurre apres un certain temps
 *          - verifier si le leurre n'est pas deja detruit avec CancelInvoke()
 */

public class ComportementLeurre : MonoBehaviour, ICible
{
    public Vector3 Position => transform.position;

    public event Action OnCibleDetruite;

    private float _tempsAvantAutoDetruire = 10f;

    void Start()
    {
        // on appel la methode qui s'activera apres un certain delais
        Invoke(nameof(AutoDetruire), _tempsAvantAutoDetruire);
    }

    public void SeFaireAttaquer(int dommage)
    {
        // on annule le invoke advenant que le leurre a ete detruit prealablement par un monstre
        CancelInvoke(nameof(AutoDetruire));

        OnCibleDetruite?.Invoke();
        Destroy(gameObject);
    }

    private void AutoDetruire()
    {
        OnCibleDetruite?.Invoke();
        Destroy(gameObject);
    }
}
