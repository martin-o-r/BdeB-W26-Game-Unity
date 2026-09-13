using UnityEngine;

/*
 * Exemple donne par l'enseignant :
 * Si on a 5 ennemis par vague, chaque ennemi devrait être placé par bonds de 360/5 = 72 degrés.
 * 
 * degre total = 360
 * nbEnnemis = x
 * espacement (en angle) = degre/nbEnnemis
 * 
 * https://discussions.unity.com/t/how-to-instantiate-objects-in-a-circle-formation-around-a-point/226980/2
 *      -> pour utiliser cos/sin dans Mathf unity, il faut utiliser radian pour determiner (x,y)
 * 
 * https://docs.unity3d.com/ScriptReference/Mathf.Deg2Rad.html
 *      -> angleDegre * Mathf.Deg2Rad
 *      -> conversion en radian pour unity
 */

public class StrategieCercle : IStrategieSpawn
{
    // rayon du plan grand capsule collider d'un monstre, pris manuellement de l'inspector
    private float _radius = 0.5f;

    private float _rayonSpawn = 3.5f;

    public Vector3[] GetPositions(int nbEnnemis, Vector3 positionJoueur)
    {
        Vector3[] positions = new Vector3[nbEnnemis];

        float espacement = 360f / nbEnnemis;

        for (int i = 0; i < nbEnnemis; i++)
        {
            // espacement * i = degre de chaque ennemi selon leur position
            float radian = (espacement * i) * Mathf.Deg2Rad;

            Vector3 position = new Vector3(positionJoueur.x + _rayonSpawn * Mathf.Cos(radian), _radius, positionJoueur.z + _rayonSpawn * Mathf.Sin(radian));

            positions[i] = position;
        }

        return positions;
    }
}
