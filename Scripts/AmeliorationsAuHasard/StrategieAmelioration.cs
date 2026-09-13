using UnityEngine;

/// <summary>
/// Cette classe abstraite represente une strategie d'amelioration pour le joueur.
/// Elle contient les informatios necessaires pour ameliorer le joueur en fonction de la valeur d'amelioration.
/// </summary>

public abstract class StrategieAmelioration
{
    // valeur de l'amelioration ajoute au joueur
    public float ValeurAmelioration;

    // Nom pour le label dans l'affichage des ameliorations
    public abstract string NomAmelioration { get;}

    // Constructeur qui initialise la valeur d'amelioration qui sera dicte par le gestionnaire d'amelioration
    public StrategieAmelioration(float valeurAmelioration)
    {
        ValeurAmelioration = valeurAmelioration;
    }

    // methode qui va ameliorer le joueur en fonction de la valeur d'amelioration
    public abstract void Ameliorer(ComportementPersonnage personnage);
}
