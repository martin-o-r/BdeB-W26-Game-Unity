using UnityEngine;

/*
 * En ce moment, le code original de l'enseignant ComportementPersonnage.cs>AmeliorerDommage() fait reference a l'amelioration de la VITESSE D'ATTAQUE!
 *      -> AmeliorerDommage() fait appel a ScriptAttaque.cs>AmeliorerRapidite()
 *      -> la rapidite d'attaque est calcule selon le niveauAttaque du joueur
 *      -> lorsque l'amelioration "Dommage" est selectionne, elle incremente de +1 le niveau d'attaque
 *      -> dans les consignes du issue #9, il est indique qu'il monte de +5?
 *      
 * logique de transfer de parametres :
 *      -> (ComportementPersonnage)AmeliorerDommage(int amelioration)
 *      -> scriptAttaque.AmeliorerRapidite(int amelioration);
 *      -> (ScriptAttaque)AmeliorerRapidite(int amelioration)
 *          -> niveauAttaque += amelioration;
 */

public class AmeliorationDommage : StrategieAmelioration
{
    public override string NomAmelioration => $"Attaque + {ValeurAmelioration}";
    
    public AmeliorationDommage(float valeurAmelioration) : base (valeurAmelioration) { }

    public override void Ameliorer(ComportementPersonnage personnage)
    {
        personnage.AmeliorerDommage((int)ValeurAmelioration);
    }
}
