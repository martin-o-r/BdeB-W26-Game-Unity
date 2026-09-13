using System.Collections;
using UnityEngine;

/*
 * Attention !!!!
 * Divison des taches : 
 *      1 - doit gerer la creation des boules de feu
 *      2 - doit gerer ce que la boule de fait!!
 *      
 * Ce script doit se concenter sur la gestion ds boules!!
 *      -> quand instantiate
 *      -> les ameliorations des objet instantiated
 *      
 * La vitesse d'attaque est prise directement du script SciptAttaque.cs a l'aide d'un public static qui contient la vitesse d'attaque.
 * Alors des que l'utilisateur ameliore la vitesse d'attaque, elle sera directement refletee dans la vitesse d'attaque par boules de feu aussi
 * 
 * https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Random-onUnitSphere.html
 *      -> Random.onUnitSphere
 *      -> retourne un point random dans une sphere qui est centre a (0,0,0)
 *      -> on peut garder que les positions x et z puis .normalized avec la position du joueur
 *      
 * https://stackoverflow.com/questions/49678042/get-random-point-on-a-unit-circle-circle-at-0-0
 * https://docs.unity3d.com/ScriptReference/Random-insideUnitCircle.html
 * https://discussions.unity.com/t/help-with-random-insideunitcircle/872941
 *      -> UnityEngine.Random.insideUnitCircle();
 *          - retourne un Vector2 de la position d'un point P random a l'interieur d'un cercle de rayon 1
 *          - ce cercle est centre a l'origine O(0,0,0)
 *          - la norme du vecteur OP peut varier
 *      -> UnityEngine.Random.insideUnitCircle().normalized;
 *          - permet d'avoir une norme = 1 du centre de l'origine vers le point (vecteur unitaire)
 *          - position + direction * radius 
 *              > position == point de reference
 *              > direction == vers ou pointe vecteur unitaire
 *              > radius == scalaire qui defini la norme de direction (aka offset du joueur)
 *              
 * https://discussions.unity.com/t/how-quaternion-lookrotation-works/812156/5
 *      -> Quaternion.LookRotation() --> changement de plan, la direction se fera dans le script AttaqueBouleFeu.cs
 *          - permet de definir la rotation selon le vecteur mis en parametre
 *          - dans notre cas, on chercher un vecteur oppose au joueur
 *          - on doit faire reference a la position de spawn de la boule de feu
 *          - vecteur AP = OP - OA selon Chasles generalise
 *          - soit A = spawnPosition et P = joueur
 *          - spawnPosition - joueur == vecteur oppose a la position du joueur
 */

/// <summary>
/// Script qui gere l'instantiation des boules de feu. La rapidite de l'attaque est partage avec la rapidite de l'attribut tempsEntreAttaque du script ScriptAttaque.cs.
/// </summary>
/// <remarks>
/// La gestion du dommage et du deplacement par les boules de feu sera fait par un autre script : AttaqueBouleFeu.cs
/// </remarks>

public class GestionBouleDeFeu : MonoBehaviour
{
    [SerializeField]
    private GameObject _bouleFeuPrefab;

    [SerializeField]
    private GameObject _joueur;

    [SerializeField]
    private float _grosseurAmelioration = 0.2f;

    private float _grosseurBoule = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    { 
        StartCoroutine(AttaqueBouleFeu()); 
    }

    private void OnEnable()
    {
        ScriptAttaque.OnAmeliorerRayon += FaireGrossirBoule;
        StartCoroutine(AttaqueBouleFeu());
    }

    private void OnDisable()
    {
        ScriptAttaque.OnAmeliorerRayon -= FaireGrossirBoule;
        StopAllCoroutines(); // arrete la creation de boules de feu lorsque le script est desactive
    }

    // Methode qui s'occupe seulement d'instancier les boules de feu
    private IEnumerator AttaqueBouleFeu()
    {
        while (true)
        {
            // delais qui represente la vitesse d'attaque
            yield return new WaitForSeconds(ScriptAttaque.TempsEntreAttaque);

            Vector3 spawnPosition = TrouverPositionBouleFeu();

            GameObject bouleFeu = Instantiate(_bouleFeuPrefab, spawnPosition, Quaternion.identity);

            bouleFeu.transform.localScale = new Vector3(_grosseurBoule, _grosseurBoule, _grosseurBoule);
        }
    }

    private Vector3 TrouverPositionBouleFeu()
    {
        // trouver une position aux alentours d'Olivia afin de spawn la boule de feu
        Vector2 randomCercle = Random.insideUnitCircle.normalized;

        // dans plan R2 -> x par y
        // dans plan R3 -> x par y sur hauteur y
        // donc plan R2 -> R3 | y -> z
        Vector3 spawnDirection = new Vector3(randomCercle.x, 0f, randomCercle.y);

        // decalement par la position du joueur
        Vector3 spawnPosition = _joueur.transform.position + spawnDirection * (ScriptAttaque.SphereRayonInitiale / 2);
        spawnPosition.y = 0.75f;

        return spawnPosition;
    }

    // Invoked a partir de ScriptAttaque.cs lorsque l'utilisateur choisit d'ameliorer le rayon
    private void FaireGrossirBoule(float sphereRadius)
    {
        _grosseurBoule += _grosseurAmelioration;
    }
}
