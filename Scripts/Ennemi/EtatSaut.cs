using UnityEngine;

/*
 * https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Mathf.Clamp01.html
 *      -> Mathf.Clamp01(float value) retourne un float compris entre 0 et 1
 *      -> pour le pourcentage de completion du saut
 *      -> necessaire pour utiliser Lerp
 *      
 * https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Mathf.Lerp.html
 *      -> Linear interpolation
 *      -> retounrne une valeur entre a et b en fonction de t (valeur interpolation)
 *      -> 
 * https://docs.unity3d.com/ScriptReference/Vector3.Lerp.html
 *      -> meme que Lerp, mais le calcul utilise des Vector3
 *      -> dans notre cas, on a besoin que la variation se fasse que sur y
 *      -> il faut suivre la parabole du saut
 * https://koshurai.medium.com/understanding-linear-interpolation-a-simple-guide-fe817d89cbc7
 *      -> linear interpolation == Lerp
 *      -> est un calcul d'approximation d'une valeur entre 2 points
 *      -> se calcul sur une droite ou sur une courbe
 *      -> on assume que delta est lineaire entre 2 points
 *      
 * https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
 *      -> exemples d'utilisation de Lerp + explications
 *      
 * https://docs.unity3d.com/ScriptReference/Object.Instantiate.html
 *      -> on ne peut pas utiliser Instantiate() dans un script normal
 *      -> il faut appeler la methode static au complet : Object.Instantiate()
 *      
 * https://docs.unity3d.com/ScriptReference/Physics.SphereCastAll.html
 *      -> retourne un RaycastHits[]
 *      -> tous les colliders "hit" dans le balayage des colliders
 *      -> ajouter decalage lors atterrissage
 *      -> on veut balayer les collider plus haut que y=0
 *      -> deplace une sphere a travers la scene et permet de detecter les collider a travers ce deplacement
 *      -> 3e parametre == direction
 *          - direction dans laquelle la sphere se deplace
 *      -> 4e parametre == maxDistance
 *          - distance que la sphere parcours a partir de son point de depart
 */

public class EtatSaut : EtatEnnemi
{
    private float _dureeSaut = 3f;
    private float _hauteurSaut = 3f;
    private float _rayonAtterrissage = 5f;
    private int _dommageSaut = 2;

    // Variables pour le calcul de la trajectoire du saut
    private Vector3 _positionDepart;
    private Vector3 _positionArrivee;
    private float _tempsEcoule;

    // Variables pour la zone d'atterrissage et la lumière
    private Light _lumiereAtterrisage;
    private const float _rayonMaxLumiere = 5f;

    public EtatSaut(ComportementEnnemi _sujet) : base(_sujet) { }

    public override void Entrer()
    {
        // Desactiver le NavMeshAgent pour realiser le saut du debut
        sujet.agent.enabled = false;

        _positionDepart = sujet.transform.position;

        // Le monstre cible Olivia == _positionArrivee
        _positionArrivee = sujet.CibleCourante.Position;
        _positionArrivee.y = 0f; // S'assurer que l'atterrisage est au niveau du sol

        _tempsEcoule = 0f;

        InstantierLumiereAtterrisage();
    }

    public override void Executer(float deltaTime)
    {
        _tempsEcoule += Time.deltaTime;

        // p == pourcentage complete du saut, de 0 a 1
        float p = Mathf.Clamp01(_tempsEcoule / _dureeSaut);

        // distance entre depart et arrivee
        Vector3 positionCourante = Vector3.Lerp(_positionDepart, _positionArrivee, p);

        // formule pour calculer la hauteur du saut en fonction du pourcentage de completion
        // y = 4 x h x p x ( 1 − p )
        positionCourante.y = 4f * _hauteurSaut * p * (1f - p);

        // va se deplacer en parabole vers Olivia
        sujet.transform.position = positionCourante;

        // monstre regarde Olivia 
        Vector3 direction = (_positionArrivee - _positionDepart);
        //direction.y = 0f;
        sujet.transform.rotation = Quaternion.LookRotation(direction);

        // simuler le grossissement de la lumiere
        _lumiereAtterrisage.range = _rayonAtterrissage * p;

        // lorsque la parcours de la parabole est complete, alors le monstre attaque
        if (p >= 1) AttaquerAAtterrissage();
    }

    public override void Sortir()
    {
        sujet.agent.enabled = true;
        
        // on ne detruit pas juste le component Light, mais son GameObject aussi
        Object.Destroy(_lumiereAtterrisage.gameObject);
    }

    private void InstantierLumiereAtterrisage()
    {
        _lumiereAtterrisage = Object.Instantiate(sujet.LumiereAtterrissagePrefab, _positionArrivee, Quaternion.identity);

        // on s'assure qu'il n'y ait pas de lumiere prealablement
        _lumiereAtterrisage.range = 0f;
    }

    private void AttaquerAAtterrissage()
    {
        // la "sphere" apparait juste en haut du sol, puis descend
        RaycastHit[] hits = Physics.SphereCastAll(_positionArrivee + Vector3.up, _rayonAtterrissage, Vector3.down, 1f);

        foreach (RaycastHit hit in hits)
        {
            // One ne fait rien si la "sphere" touche le monstre qui a saute
            if (hit.collider.gameObject == sujet.gameObject) continue;

            // on essaye de chercher ICible soit du leurre ou d'Olivia
            if (hit.collider.TryGetComponent<ICible>(out var cible))
            {
                cible.SeFaireAttaquer(_dommageSaut);
            } 
        }

        sujet.ChangerEtat(sujet.etatPoursuite);
    }
}
