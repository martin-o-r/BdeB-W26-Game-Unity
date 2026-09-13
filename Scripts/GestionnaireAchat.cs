using TMPro;
using UnityEngine;

/*
 * https://stackoverflow.com/questions/41073236/how-to-save-bool-to-playerprefs-unity
 *      -> PlayerPrefs ne supporte pas les bool, il faut les convertir en int
 *      -> sauvegarder un bool en int (0 ou 1)
 *      -> PlayerPrefs.SetInt("BouleFeuAchete", 1); pour true
 *      -> PlayerPrefs.SetInt("BouleFeuAchete", 0); pour false
 *      -> PlayerPrefs.GetInt("BouleFeuAchete", 0) == 1; pour lire la valeur (true ou false)
 */

public class GestionnaireAchat : MonoBehaviour
{
    private int _argent; 

    [SerializeField]
    private TextMeshProUGUI _affichageArgent;

    [SerializeField]
    private GameObject _bouleFeuBouton;

    [SerializeField]
    private GameObject _livrePossedeBouton;

    private int _prixBouleFeu = 150;
    private int _prixLivre = 100;

    // Ce script instantiate les boules de feu
    [SerializeField]
    private GestionBouleDeFeu _gestionnaireBouleFeu;

    // le LivrePossede est contenu dans ce GameObject
    [SerializeField]
    private GameObject _gestionnaireLivrePossede;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _argent = PlayerPrefs.GetInt("Argent", 0);

        _affichageArgent.text = _argent.ToString();

        // Attaque boule de feu
        bool bouleFeuAchete = PlayerPrefs.GetInt("BouleFeuAchete", 0) == 1;
        _bouleFeuBouton.SetActive(!bouleFeuAchete);
        _gestionnaireBouleFeu.enabled = bouleFeuAchete;

        // Attaque livre possede
        bool livrePossedeAchete = PlayerPrefs.GetInt("LivrePossedeAchete", 0) == 1;
        _livrePossedeBouton.SetActive(!livrePossedeAchete);
        _gestionnaireLivrePossede.SetActive(livrePossedeAchete);
    }

    private void OnEnable()
    {
        GestionnaireJeu.OnSauvegarderArgent += SauvegardeArgent;
    }

    private void OnDisable()
    {
        GestionnaireJeu.OnSauvegarderArgent -= SauvegardeArgent;
    }

    // Acheter l'attaque boule de feu
    public void AcheterAttaqueBouleFeu()
    {
        if (!FaireAchat(_prixBouleFeu)) return;

        DebloquerBouleFeu();
    }

    private void DebloquerBouleFeu()
    {
        _bouleFeuBouton.SetActive(false);
        _gestionnaireBouleFeu.enabled = true;

        PlayerPrefs.SetInt("BouleFeuAchete", 1);
        PlayerPrefs.Save();
    }


    // Acheter l'attaque livre possede
    public void AcheterAttaqueLivrePossede()
    {
        if (!FaireAchat(_prixLivre)) return;

        DebloquerLivrePossede();
    }

    private void DebloquerLivrePossede()
    {
        _livrePossedeBouton.SetActive(false);
        _gestionnaireLivrePossede.SetActive(true);

        PlayerPrefs.SetInt("LivrePossedeAchete", 1);
        PlayerPrefs.Save();
    }

    private bool FaireAchat(int prix)
    {
        // si pas assez, on ne fait rien
        if (_argent < prix) return false ;

        _argent -= prix;

        MettreAJourArgent();

        return true;
    }

    // A la fin de la partie, la methode est appelee pour sauvegarder l'argent
    private void SauvegardeArgent(int nbMonstresContenus)
    {
        _argent += nbMonstresContenus * ParametresJeu.Instance.ArgentTauxDifficulte;

        MettreAJourArgent();
    }

    private void MettreAJourArgent()
    {
        _affichageArgent.text = _argent.ToString();

        PlayerPrefs.SetInt("Argent", _argent);
        PlayerPrefs.Save();
    }


    // ==== methode pour les codes de triche ====

    public void AjouterArgent(int montant)
    {
        _argent += montant;

        MettreAJourArgent();
    }
    
    public void DebloquerToutesAttaques()
    {
        DebloquerBouleFeu();
        DebloquerLivrePossede();
    }

    // Code de triche supplementaire pour reinitialiser les achats (pour les tests)
    public void ReinitialiserAchats()
    {
        // Reinitialiser l'argent
        _argent = 0;
        MettreAJourArgent();

        // Reinitialiser les attaques achetees
        PlayerPrefs.SetInt("BouleFeuAchete", 0);
        _bouleFeuBouton.SetActive(true);
        _gestionnaireBouleFeu.enabled = false;

        PlayerPrefs.SetInt("LivrePossedeAchete", 0);
        _livrePossedeBouton.SetActive(true);
        _gestionnaireLivrePossede.SetActive(false);

        PlayerPrefs.Save();
    }
}
