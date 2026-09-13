using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.AI;

/*
 * https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Random-value.html
 *      -> UnityEngine.Random.value retourne un float compris [0.0f, 1.0f]
 *      -> les 2 bornes sont incluses
 *      
 * https://medium.com/@donovanlett85/unity-game-development-how-to-give-items-a-number-based-chance-of-spawning-in-e16f266fcf49
 *      -> weighted selection algorithm
 *      -> une structure de donnes va contenir une chance X et une action a accomplir (tuple)
 *      -> on determine un seuil Random.value : [0.0f, 1.0f]
 *      -> on passe a travers la structure de donnees et on additionne les chances de chaque evenement
 *      -> on verifie si on a atteint le seuil
 *      -> si oui, l'action associe a la chance est activee, sinon on continue
 *      -> logique :
 *          - total des chance == 1 (100%)
 *          - determiner un seuil au hasard Random.value
 *          - on additionne les chance de chaque evenements puis on compare au seuil
 * https://discussions.unity.com/t/fast-weighted-random-selection-tutorial-code/950094/2
 *      -> utiliser une list de tuple
 *      -> on peut choisir une list comme structure de donnees qui va contenir le weighted selection algorithm
 * https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples
 *      -> un tuple en C# est declare par () nomTuple = (Var1: x, Var2: y, etc) <- field names
 *      -> cas d'utilisation dans une list : List<(float nomVar1, Action action1)>
 *      -> application directe du weighted selection algorithm
 */

public class ComportementEnnemi : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animateur;
    [HideInInspector] public ComportementPersonnage scriptJoueur;

    public float distancePoursuiteToAttaque = 5f;
    public float distanceAttaqueToPoursuite = 2f;
    public float tempsEntreAttaque = 2f;
    public int dommages = 2;
    public int experiences = 5;
    public int vie = 10;

    private EtatEnnemi etatCourant;
    public EtatPoursuite etatPoursuite;
    public EtatFuite etatFuite;
    public EtatAttaque etatAttaque;
    // ajout de l'etatSaut en suivantle format precedent
    public EtatSaut etatSaut;

    [SerializeField] 
    private GameObject _experiencePrefab;
    [SerializeField]
    private GameObject _remplirVieOrbPrefab;
    [SerializeField]
    private GameObject _terroriserEnnemiPrefab;

    private List<(float chance, GameObject orb)> _orbsPossible;

    private AnimationViePerdue animationViePerdue;
    
    public event Action OnMort;
    
    [SerializeField] 
    public bool EstPeureux;

    // Sert a determiner la cible actuelle des monstres
    // Pointe vers Olivia ou le leurre
    private ICible _cibleCourante;
    public ICible CibleCourante => _cibleCourante;

    // passer la lumiere d'atterrissage a l'etatSaut
    [SerializeField]
    private Light _lumierreAtterissagePrefab;
    public Light LumiereAtterrissagePrefab => _lumierreAtterissagePrefab;

    private bool _commenceParSaut = false;

    public void Initialiser(ComportementPersonnage _scriptJoueur)
    {
        scriptJoueur = _scriptJoueur;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animateur = GetComponent<Animator>();

        etatPoursuite = new EtatPoursuite(this);
        etatFuite = new EtatFuite(this);
        etatAttaque = new EtatAttaque(this);
        etatSaut = new EtatSaut(this);

        // Pour commencer la scene de jeu, les monsters se dirigent vers Olivia par defaut
        ChoisirCible(scriptJoueur);

        if (_commenceParSaut)
        {
            etatCourant = etatSaut;
            etatCourant.Entrer();
            // pas besoin de modifier _commenceParSaut, car le monstre est independant, ca n'affecte pas les futurs changements d'etat
        }
        else
        {
            etatCourant = etatPoursuite;
            etatCourant.Entrer(); // debut de l'etat du monstre, il faut qu'il connaisse sa cible
        }

        animationViePerdue = GetComponent<AnimationViePerdue>();

        RemplirOrbPossibles();
    }

    void Update()
    {
        etatCourant.Executer(Time.deltaTime);
    }

    public void ChangerEtat(EtatEnnemi nouvelEtat)
    {
        if (etatCourant == nouvelEtat) return;

        etatCourant.Sortir();
        etatCourant = nouvelEtat;
        etatCourant.Entrer();
    }

    public void PerdreVie(int dommage)
    {
        vie -= dommage;
        if (vie <= 0) Mourir();
        animationViePerdue.Demarrer();
    }

    private void Mourir()
    {
        InstantierOrb();

        // Detruit le monstre
        Destroy(gameObject); 

        // Rafraichit compteur monstres libres/tues dans GestionnaireJeu.cs
        OnMort?.Invoke(); 
    }

    private void RemplirOrbPossibles()
    {
        _orbsPossible = new List<(float, GameObject)>
        {
            (0.85f, _experiencePrefab),
            (0.10f, _remplirVieOrbPrefab),
            (0.05f, _terroriserEnnemiPrefab)
        };
    }

    private void InstantierOrb()
    {
        Vector3 positionOrb = new Vector3(transform.position.x, 0.5f, transform.position.z);

        GameObject orb = Instantiate(ChoixOrbAuHasard(), positionOrb, Quaternion.identity);

        // verification pour les objet experienec seulement
        if (orb.TryGetComponent<ObjetExperience>(out ObjetExperience experience))
        {
            experience.experienceGagne = experiences;
        }
    }

    private GameObject ChoixOrbAuHasard()
    {
        float chance = UnityEngine.Random.value;

        float sommeChance = 0;
        GameObject orb = null;

        foreach ((float, GameObject) evenement in _orbsPossible)
        {
            sommeChance += evenement.Item1;

            if (chance < sommeChance)
            {
                orb = evenement.Item2;
                break;
            }
        }

        return orb;
    }

    public bool APeur()
    {
        return EstPeureux && vie == scriptJoueur.Dommages;
    }

    /*
     * Methode qui gere le changement de cible entre Olivia et le leurre
     * ComportementEnnemi garde une reference vers la _cibleCourante
     * Lorsque la ccible courante est detruite, elle invoke OnCibleDetruite,
     * ce qui declenche automatiquement CiblerOliviar()
     */
    public void ChoisirCible(ICible nouvelleCible) // nouvelleCible == leurre
    {
        if (_cibleCourante != null) // _cibleCourante == Olivia
        {
            // se desabonner de l'ancienne cible
            _cibleCourante.OnCibleDetruite -= CiblerOlivia;
        }

        // les monstrent se dirigent vers la nouvelle cible : Olivia ou le leurre
        _cibleCourante = nouvelleCible;

        // subscribe a la nouvelle cible
        _cibleCourante.OnCibleDetruite += CiblerOlivia;
    }

    private void CiblerOlivia()
    {
        ChoisirCible(scriptJoueur);
    }

    // Determine si le monstre commence par le saut ou etatPoursuite
    public void EnnemiCommenceSaut()
    {
        _commenceParSaut = true;
    }
}