using System;
using UnityEngine;

/*
 * Toutes les methodes d'amelioration passe par ComportementPersonnage, c'est lui qui va faire les changements sur le joueur.
 * 
 * Dans le cas de cette amelioraton, le scritp ci haut ne vas que changer le dommage des attaques de base
 * 
 * Utilisation du patron observer pour faire le lien entre l'amelioration et les attaques supplementaires : livre possede et attaque par boules de feu
 */

public class AmeliorationDommageAttaques : StrategieAmelioration
{
    public static Action<int> OnAmeliorerDommageAttaques;

    public override string NomAmelioration => $"Dommage + {ValeurAmelioration}";

    public AmeliorationDommageAttaques(float valeurAmelioration) : base(valeurAmelioration) { }

    public override void Ameliorer(ComportementPersonnage personnage)
    {
        OnAmeliorerDommageAttaques?.Invoke((int)ValeurAmelioration);
    }
}
