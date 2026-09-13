using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class GestionnaireAmeliorations : MonoBehaviour
{
    [SerializeField]
    private ComportementPersonnage scriptJoueur;

    [SerializeField]
    private ComportementCamera comportementCamera;

    [SerializeField]
    private GameObject panel;

    [SerializeField] 
    private TMP_Text textAmelioration1;
    
    [SerializeField] 
    private TMP_Text textAmelioration2;
    
    [SerializeField] 
    private TMP_Text textAmelioration3;

    [SerializeField] 
    private int vieGagne = 4;
    
    [SerializeField] 
    private int rayonGagne = 1;
    
    [SerializeField] 
    private int vitesseAttaqueGagne = 1;

    [SerializeField]
    private float _courseGagne = 1.25f;

    [SerializeField]
    private int _dommageGagne = 1;

    // contient toutes les ameliorations possibles
    private List<StrategieAmelioration> _ameliorationsPossible;

    // array qui va contenir les ameliorations a afficher
    private List<StrategieAmelioration> _ameliorationsAAfficher = new List<StrategieAmelioration>();

    void Start()
    {
        panel.SetActive(false); // commencer avec le panel des ameliorations ferme

        RemplirListeAmeliorationsPossibles();
    }

    private void OnEnable()
    {
        scriptJoueur.OnAmeliorationAtteinte += OuvrirMenu;
    }

    private void OnDisable()
    {
        scriptJoueur.OnAmeliorationAtteinte -= OuvrirMenu;
    }

    // Le menu s'ouvre lorsque le joueur attein le seuil de points amasses
    private void OuvrirMenu()
    {
        Selectionner3AmeliorationsHasard();

        AfficherNomAmeliorations();

        Time.timeScale = 0;
        comportementCamera.ArreterBrassage();
        panel.SetActive(true);
    }

    private void FermerMenu()
    {
        Time.timeScale = 1;
        panel.SetActive(false);

        // s'assurer de "vider" les _ameliorationsAAfficher pour la prochaine fois que le menu s'ouvre
        _ameliorationsAAfficher.Clear();
    }

    private void RemplirListeAmeliorationsPossibles()
    {
        // Dans le futur, si no doit rajouter des ameliorations, il suffira de les ajouter dans cette liste
        _ameliorationsPossible = new List<StrategieAmelioration>
        {
            new AmeliorationVie(vieGagne),
            new AmeliorationRayon(rayonGagne),
            new AmeliorationDommage(vitesseAttaqueGagne),
            new AmeliorationCourse(_courseGagne),
            new AmeliorationDommageAttaques(_dommageGagne)
        };
    }

    private void Selectionner3AmeliorationsHasard()
    {
        while (_ameliorationsAAfficher.Count < 3)
        {
            int indexRandom = UnityEngine.Random.Range(0, _ameliorationsPossible.Count);

            if (!_ameliorationsAAfficher.Contains(_ameliorationsPossible[indexRandom]))
            {
                _ameliorationsAAfficher.Add(_ameliorationsPossible[indexRandom]);
            }
        }
    }

    private void AfficherNomAmeliorations()
    {
        textAmelioration1.text = _ameliorationsAAfficher[0].NomAmelioration;
        textAmelioration2.text = _ameliorationsAAfficher[1].NomAmelioration;
        textAmelioration3.text = _ameliorationsAAfficher[2].NomAmelioration;
    }

    // ===== Les methodes sont associees aux boutons du menu =====

    public void Amelioration1()
    {
        _ameliorationsAAfficher[0].Ameliorer(scriptJoueur);
        FermerMenu();
    }

    public void Amelioration2()
    {
        _ameliorationsAAfficher[1].Ameliorer(scriptJoueur);
        FermerMenu();
    }
    public void Amelioration3()
    {
        _ameliorationsAAfficher[2].Ameliorer(scriptJoueur);
        FermerMenu();
    }
}
