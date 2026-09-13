using UnityEngine;
using UnityEngine.InputSystem;


public class CodesTriche : MonoBehaviour
{
    private GestionnaireJeu gestionnaireJeu;

    [SerializeField]
    private GestionnaireAchat _gestionnaireAchat;

    [SerializeField] 
    private GameObject experiencePrefab;

    [SerializeField]
    private GameObject _remplirViePrefab;

    [SerializeField]
    private GameObject _terroriserPrefab;

    void Start()
    {
        gestionnaireJeu = GetComponent<GestionnaireJeu>();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            gestionnaireJeu.Joueur.RedonnerMaxVies();
        }

        // 2 => faire apparaitre un orb d'experience
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            InstantierOrbTest(experiencePrefab);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            gestionnaireJeu.Joueur.GagnerExperience(5);
        }

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            gestionnaireJeu.Joueur.GagnerExperience(100);
        }

        // 5 => declenche une vague d'ennemi directement
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            gestionnaireJeu.DeclencherVague();
        }

        // 6 => faire apparaitre un objet qui rempli la vie du joueur
        if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            InstantierOrbTest(_remplirViePrefab);
        }

        // 7 => faire apparaitre un objet qui terrorise les monstres
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            InstantierOrbTest(_terroriserPrefab);
        }

        // 8 => faire gagner 50 $ au joueur
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            _gestionnaireAchat.AjouterArgent(50);
        }

        // 9 => devrait debloquer les 2 attaques : livre et boules de feu
        if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            _gestionnaireAchat.DebloquerToutesAttaques();
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            gestionnaireJeu.Joueur.PerdreVies(1);
        }
        
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            gestionnaireJeu.Joueur.PerdreVies(999);
        }

        // X => reinitialiser les achats : 0 argent, les attaques bloquees
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            _gestionnaireAchat.ReinitialiserAchats();
        }

        if (Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            foreach (var ennemi in FindObjectsByType<ComportementEnnemi>(FindObjectsSortMode.None))
            {
                Destroy(ennemi.gameObject);
            }
        }
    }

    // Methode qui instantie un type d'orb a un endroit au hasard autour du joueur
    private void InstantierOrbTest(GameObject orb)
    {
        Vector3 positionJoueur = gestionnaireJeu.Joueur.transform.position;

        Vector3 spawn = positionJoueur + new Vector3(Random.value * 5 - 2.5f, 0, Random.value * 5 - 2.5f);

        Instantiate(orb, spawn, Quaternion.identity);
    }
}