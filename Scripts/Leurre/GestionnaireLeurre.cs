using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestionnaireLeurre : MonoBehaviour
{
    [SerializeField]
    private GameObject _leurrePrefab;

    [SerializeField]
    private RawImage _leurreIcone;

    [SerializeField]
    private float _distanceCiblerLeurre = 5f;

    [SerializeField]
    private float _leurreCooldown = 7f;

    private bool _isLeurreDisponible = true;

   
    void Start()
    {
        // l'icone leurre commence activee
        _leurreIcone.enabled = true;
    }

    private void OnEnable()
    {
        GestionnaireJeu.OnFinDuJeu += DesactiverLeurreIcone;
    }

    private void OnDisable()
    {
        GestionnaireJeu.OnFinDuJeu -= DesactiverLeurreIcone;
    }

    public void ActiverLeurre()
    {
        if (!_isLeurreDisponible) return;

        GameObject leurrePrefab = Instantiate(_leurrePrefab, transform.position, Quaternion.identity);

        ComportementLeurre leurre = leurrePrefab.GetComponent<ComportementLeurre>();

        MonstresCiblerLeurre(leurrePrefab, leurre);

        StartCoroutine(LeurreCooldown());
    }

    private void MonstresCiblerLeurre(GameObject leurrePrefab, ComportementLeurre leurre)
    {
        GameObject[] monstres = GameObject.FindGameObjectsWithTag("Monstre");

        // on passe a travers tous les monstres pour determiner leur distance, si un monstre est assez proche du leurre, alors il cible le leurre
        foreach (GameObject monstre in monstres)
        {
            if (Vector3.Distance(monstre.transform.position, leurrePrefab.transform.position) < _distanceCiblerLeurre)
            {
                monstre.GetComponent<ComportementEnnemi>().ChoisirCible(leurre);
            }
        }
    }

    // le cooldown du leurre est fait avec une coroutine
    private IEnumerator LeurreCooldown()
    {
        _isLeurreDisponible = false;
        _leurreIcone.enabled = false;

        yield return new WaitForSeconds(_leurreCooldown);

        _isLeurreDisponible = true;
        _leurreIcone.enabled = true;
    }

    private void DesactiverLeurreIcone()
    {
        _leurreIcone.enabled = false;
    }
}
