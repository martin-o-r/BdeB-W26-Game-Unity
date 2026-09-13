using System.Linq.Expressions;
using UnityEngine;

/*
 * https://www.w3schools.com/cs/cs_switch.php
 * https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/switch-expression
 *      -> la selection de la position de la ligne sera determine grace a un switch
 *      -> un nombre representant un cote
 */

public class StrategieLigne : IStrategieSpawn
{
    // rayon du plan grand capsule collider d'un monstre, pris manuellement de l'inspector
   private float _radius = 0.5f;

    private float _distance = 5f;
    public Vector3[] GetPositions(int nbEnnemis, Vector3 positionJoueur)
    {
        Vector3[] positions = new Vector3[nbEnnemis];

        // 0 = haut, 1 = bas, 2 = gauche, 3 = droite
        int direction = Random.Range(0, 4);

        Vector3 centreLigne = direction switch
        {
            0 => new Vector3(positionJoueur.x, _radius, positionJoueur.z + _distance),
            1 => new Vector3(positionJoueur.x, _radius, positionJoueur.z - _distance),
            2 => new Vector3(positionJoueur.x - _distance, _radius, positionJoueur.z),
            _ => new Vector3(positionJoueur.x + _distance, _radius, positionJoueur.z)
        };

        float espacement = _radius * 2; // diametre du monstre

        /*
         * la logique est que ligne == -2 -1 0 1 2
         * On veut commencer a "gauche" de la ligne, donc du cote negatif
         * (nbEnnemis - 1) representent le nombre d'espace entre chaque monstre
         * on divise par 2f pour trouver le milieu de la ligne de spawn
         * on * par l'espacement savoir combien d'espacement necessaire
         * le negatif pour determiner le cote "le plus possible a gauche*
         */
        float offsetDepartLigne = -(nbEnnemis - 1) / 2f * espacement;

        for (int i = 0; i < nbEnnemis; i++)
        {
            // logique : on part d'une extrimite vers une autre

            float offset = offsetDepartLigne + i * espacement;

            Vector3 directionOffset;

            if (direction == 2 || direction == 3) // gauche ou droite
            {
                directionOffset = new Vector3(0f, 0f, offset);
            }
            else
            {
                directionOffset = new Vector3(offset, 0f, 0f);
            }

            positions[i] = centreLigne + directionOffset;
        }

        return positions;
    }
}
