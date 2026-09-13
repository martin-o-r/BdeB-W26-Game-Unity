using UnityEngine;

public class StrategieHasard : IStrategieSpawn
{
    // rayon du plan grand capsule collider d'un monstre, pris manuellement de l'inspector
    private float _radius = 0.5f;

    public Vector3[] GetPositions(int nbEnnemis, Vector3 positionJoueur)
    {
        Vector3[] positions = new Vector3[nbEnnemis];

        for(int i = 0; i < nbEnnemis; i++)
        {
            positions[i] = UtilitaireApparitionPrefab.GetPositionRandom(_radius);
        }

        return positions;
    }
}
