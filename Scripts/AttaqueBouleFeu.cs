using UnityEngine;

/*
 * https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Object.Destroy.html
 *      -> Destroy(gameObject), mais aussi Destroy(gameObject, delayTime)
 *      -> on peut simplemen utiliser le 2e overload pour partir le decompte de temps avant sa destruction
 */

public class AttaqueBouleFeu : MonoBehaviour
{
    [SerializeField]
    private int _dommageBouleFeu = 5;

    private Vector3 _deplacement;
    private Vector3 _direction;

    [SerializeField]
    private float _vitesse = 3f;

    private GameObject _joueur;

    private void Awake()
    {
        _joueur = GameObject.FindWithTag("Player");
    }

    private void OnEnable()
    {
        AmeliorationDommageAttaques.OnAmeliorerDommageAttaques += AmeliorerDommage;
    }

    private void OnDisable()
    {
        AmeliorationDommageAttaques.OnAmeliorerDommageAttaques -= AmeliorerDommage;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        Destroy(gameObject, 5f);

        Vector3 joueur = new Vector3(_joueur.transform.position.x, 0.75f, _joueur.transform.position.z);

        _direction = (transform.position - joueur).normalized;
        
        _deplacement = _direction * _vitesse;
    }

    void Update()
    {
        transform.position += _deplacement * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monstre"))
        {
            ComportementEnnemi ennemi = other.gameObject.GetComponent<ComportementEnnemi>();

            if (ennemi != null) ennemi.PerdreVie(_dommageBouleFeu);
        }
    }

    // Amelioration du dommage fait par les boules de feu
    private void AmeliorerDommage(int amelioration)
    {
        _dommageBouleFeu += amelioration;
    }
}
