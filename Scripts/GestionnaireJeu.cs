using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GestionnaireJeu : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabEnnemis;

    [SerializeField] private ComportementPersonnage joueur;
    public ComportementPersonnage Joueur => joueur;

    [SerializeField] private TMP_Text txtCompteurCreaturesLibres;
    [SerializeField] private TMP_Text txtCompteurCreaturesContenues;
    [SerializeField] private TMP_Text txtCompteurFin;

    [SerializeField] private GameObject ecranFin;
    [SerializeField] private RectTransform imageGlitchTransform;


    private int nbCreaturesLibres = 0;

    private int nbCreaturesContenues = 0;

    [SerializeField] private float tempsEntreVagues = 15f;
    int nbEnnemisParVague;

    private Coroutine ajouterEnnemis1, ajouterEnnemis2;

    public static event Action<int> OnSauvegarderArgent;
    public static event Action OnFinDuJeu; // servira a cacher l'icone du leurre dans GestionnaireLeurre.cs

    private int _tentativesSpawnMonstre = 5;

    void Start()
    {
        nbEnnemisParVague = ParametresJeu.Instance.EnnemisParVague;

        txtCompteurCreaturesLibres.text = "0";
        txtCompteurCreaturesContenues.text = "0";

        ajouterEnnemis1 = StartCoroutine(AjouterEnnemisPeriodiquement());
        ajouterEnnemis2 = StartCoroutine(AjouterVagueEnnemisPeriodiquement());
    }

    private void OnEnable()
    {
        joueur.OnCibleDetruite += FinDuJeu;
    }

    private void OnDisable()
    {
        joueur.OnCibleDetruite -= FinDuJeu;
    }

    private void FinDuJeu()
    {
        // On declenche l'evenement pour sauvegarder l'argent dans le jeu
        // on transfere aussi le nombre de monstre contenus
        OnSauvegarderArgent?.Invoke(nbCreaturesContenues);

        OnFinDuJeu?.Invoke();

        txtCompteurFin.text = nbCreaturesContenues + "";
        ecranFin.SetActive(true);
        StartCoroutine(AnimationFin());
        StartCoroutine(DelaiRetourMenu());

        StopCoroutine(ajouterEnnemis1);
        StopCoroutine(ajouterEnnemis2);
        foreach (var ennemi in FindObjectsByType<ComportementEnnemi>(FindObjectsSortMode.None))
            Destroy(ennemi);
    }

    private IEnumerator AnimationFin()
    {
        while (true)
        {
            imageGlitchTransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(-960f, 960f), 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator DelaiRetourMenu()
    {
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator AjouterEnnemisPeriodiquement()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            // les monstres qui peuvent sauter sont seulment ceux qui spawn singulierement
            ApparaitreEnnemi(peutSauter: true);
        }
    }

    // Wave attacks
    private IEnumerator AjouterVagueEnnemisPeriodiquement()
    {
        while (true)
        {
            yield return new WaitForSeconds(tempsEntreVagues);

            DeclencherVague();
        }
    }

    // Methode qui declenche une vague d'ennemi
    public void DeclencherVague()
    {
        // Selection de strategie de spawn
        IStrategieSpawn[] strategies = ParametresJeu.Instance.StrategiesSpawn;

        IStrategieSpawn strategieChoisie = strategies[UnityEngine.Random.Range(0, strategies.Length
            )];

        Vector3[] positions = strategieChoisie.GetPositions(ParametresJeu.Instance.EnnemisParVague, joueur.transform.position);

        foreach (Vector3 position in positions)
        {
            ApparaitreEnnemi(position);
        }
    }

    private void RafraichirCompteurCreaturesLibres()
    {
        txtCompteurCreaturesLibres.text = nbCreaturesLibres + "";
    }

    private void RafraichirCompteurCreaturesContenues()
    {
        txtCompteurCreaturesContenues.text = nbCreaturesContenues + "";
    }

    // utilisation de "optional arguments" pour faciliter
    private void ApparaitreEnnemi(bool peutSauter = false)
    {
        // On appel la methode de UtilitaireApparitionEnnemis pour creer un monstre avec la verification de position
        GameObject ennemi = UtilitaireApparitionPrefab.Instantier(GetEnnemiRandom(), _tentativesSpawnMonstre);

        if (ennemi == null) return; 

        ComportementEnnemi scriptEnnemi = ennemi.GetComponent<ComportementEnnemi>();

        InitialiserEnnemi(scriptEnnemi);

        // Ajout de probabilite 25% que le monstre saute
        if (peutSauter && UnityEngine.Random.Range(0, 4) == 1)
        {
            scriptEnnemi.EnnemiCommenceSaut();
        }

        nbCreaturesLibres++;
        RafraichirCompteurCreaturesLibres();
        RafraichirCompteurCreaturesContenues();
    }

    /*
     * Repetiton de code de facon volontaire
     * Principe de polymorphism -> overload
     * les 2 methodes n'ont pas les memes parametres
     */
    private void ApparaitreEnnemi(Vector3 position)
    {
        // On appel la methode de UtilitaireApparitionEnnemis qui spawn directement les ennemis, car on sait deja que les ennemis ne vont pas overlap
        // position deja controle
        GameObject ennemi = UtilitaireApparitionPrefab.ApparaitreSiLibre(GetEnnemiRandom(), position);

        if (ennemi == null) return;

        ComportementEnnemi scriptEnnemi = ennemi.GetComponent<ComportementEnnemi>();

        InitialiserEnnemi(scriptEnnemi);

        nbCreaturesLibres++;
        RafraichirCompteurCreaturesLibres();
        RafraichirCompteurCreaturesContenues();
    }

    private GameObject GetEnnemiRandom()
    {
        int randomEnnemiIndex = UnityEngine.Random.Range(0, prefabEnnemis.Length);
        return prefabEnnemis[randomEnnemiIndex];
    }

    private void InitialiserEnnemi(ComportementEnnemi scriptEnnemi)
    {
        scriptEnnemi.Initialiser(joueur);

        scriptEnnemi.OnMort += EnnemiMort;
    }

    void EnnemiMort()
    {
        nbCreaturesLibres--;
        RafraichirCompteurCreaturesLibres();

        nbCreaturesContenues++;
        RafraichirCompteurCreaturesContenues();
    }
}