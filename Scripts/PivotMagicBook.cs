using UnityEngine;

public class PivotMagicBook : MonoBehaviour
{
    [SerializeField]
    private GameObject _joueur;

    [SerializeField]
    private float _vitesseRotation = 80f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = new Vector3(_joueur.transform.position.x, 0.75f, _joueur.transform.position.z);
    }

    private void OnEnable()
    {
        ScriptAttaque.OnAmeliorerAttaque += AccelererRotation;
    }

    private void OnDisable()
    {
        ScriptAttaque.OnAmeliorerAttaque -= AccelererRotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, _vitesseRotation * Time.deltaTime, 0f);
    }

    void LateUpdate()
    {
        transform.position = new Vector3(_joueur.transform.position.x, 0.75f, _joueur.transform.position.z);
    }

    // A la suite d'une amelioration sur la vitesse d'attaque, le livre va tourner plus vite autout du joueur
    private void AccelererRotation(float niveauAttaque)
    {
        float nouvelleVitesseRotation = _vitesseRotation * niveauAttaque/2;

        _vitesseRotation = Mathf.Max(_vitesseRotation, nouvelleVitesseRotation);
    }
}
