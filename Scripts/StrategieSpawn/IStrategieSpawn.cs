using UnityEngine;

/*
 * Pour l'implemetation du patron strategie, j'ai prefere utiliser une interface
 * Pour les ameliorations, il fallait consever des informations propre a chaque amelioration e.g. nom et valeur d'amelioration
 * 
 * Ma logique pour cette strategie, c'est que chaque strategie differente va retourner un tableau de positions specifique, donc 1 seule methode a implementer dans chaque extend
 */

public interface IStrategieSpawn
{
    // nbEnnemis = varie selon le niveau de difficulte choisi
    Vector3[] GetPositions(int nbEnnemis, Vector3 positionJoueur);
}
