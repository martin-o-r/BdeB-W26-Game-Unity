using UnityEngine;
using UnityEngine.UIElements;

/*
 * https://docs.unity3d.com/ScriptReference/Transform-position.html
 * https://docs.unity3d.com/ScriptReference/Transform-localPosition.html
 *      -> transform position vs localPosition :
 *          - positon fait reference au "World Space"
 *          - localPosition fait reference a l'objet parent
 *      -> dans notre cas, le livre fait reference au point pivot seulement
 * 
 * Conseil de l'enseignant :
 *      -> preferable de placer le livre possede a l'interieur d'Olivia
 *      -> finalement, j'ai creer un EmptyObject qui suit la meme positioin qu'Olivia
 *      -> ce EmptyObject a comme enfant le livre possede, qui tourne en orbit
 *      -> le EmptyObject n'est pas affecte par les changements de deplacements d'Olivia
 *      
 * Bug rencontre :
 *      - null reference desfois a la fin de la partie au niveau du OnTriggerEnter()
 *      - RaceCondition : parfois, le monstre est detruit, mais pas son component (difference de quelques frames)
 *      - ajout d'une verification pour s'assurer que le component existe encore avant de faire perdre des vies au monstre
 */

/// <summary>
/// Script qui gere le livre possede
/// </summary>
/// <remarks>
/// Le script PivotMagicBook.cs gere la vitesse de rotation du livre ainsi que les ameliorations.
/// L'Objet MagicBook a du etre enleve de la hierarchie d'Olivia pour que la rotation autour d'Olivia ne soit pas affecte par les changement de directions d'Olivia 
/// </remarks>

public class GestionLivrePossede : MonoBehaviour
{
    private float _grossissement = 0.25f;

    [SerializeField]
    private int _dommageLivrePossede = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float _distancePivot = ScriptAttaque.SphereRayonInitiale;

        transform.localPosition = new Vector3(_distancePivot + transform.localPosition.x, transform.localPosition.y, 0f);
    }

    private void OnEnable()
    {
        ScriptAttaque.OnAmeliorerRayon += AgrandirRayonLivre;

        AmeliorationDommageAttaques.OnAmeliorerDommageAttaques += AmeliorerDommage;
    }

    private void OnDisable()
    {
        ScriptAttaque.OnAmeliorerRayon -= AgrandirRayonLivre;

        AmeliorationDommageAttaques.OnAmeliorerDommageAttaques -= AmeliorerDommage;
    }

    // A la suite d'une amelioration du rayon d'attaque, le rayon livre-joueur va augmenter et le livre va grossir
    private void AgrandirRayonLivre(float nouveauRayon)
    {
        transform.localPosition = new Vector3(nouveauRayon, transform.localPosition.y, 0f);

        GrossirLivre();
    }

    private void GrossirLivre()
    {
        // multiplication d'un vecteur par un scalaire
        // scalaire k, vecteur Vector3.one = (1, 1, 1) => k(1,1,1) == (k,k,k)
        // puis somme de 2 vecteurs de meme format
        transform.localScale += Vector3.one * _grossissement;
    }

    // Amelioration du dommage fait par le livre possede
    private void AmeliorerDommage(int amelioration)
    {
        _dommageLivrePossede += amelioration;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monstre"))
        {
            ComportementEnnemi ennemi = other.gameObject.GetComponent<ComportementEnnemi>();

            if (ennemi != null) ennemi.PerdreVie(_dommageLivrePossede);
        }
    }
}
