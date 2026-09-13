using UnityEngine;

/*
 * Logique suivi:
 * Dans le GestionnaireJeu.cs on appel...
 * -> UtilitaireApparitionPrefab.Instantier(prefab)
 *      -> Instantier() appel ApparaitreAuHasard() appel ApparaitreSiLibre() && GetPositionrandom
 *      -> ApparaitreSiLibre() return null | GameObject a ApparaitreAuHasard()
 *      -> ApparaitreAuHasard() return null | GameObject a Instantier()
 *      -> Instantier() return GameObject | null
 *          - par contre, lorsque Instantier() return null, ca veut dire qu'on a deja essaye 5 fois de spawn un monstre
 *          - donc theoriquement il n'y a plus d'espace disponible
 *          
 * ApparaitreSiLibre() et ApparaitreAuHasard() ont la meme ligne de code, mais necessaire pour tests
 */

public static class UtilitaireApparitionPrefab
{
    private static int _plan = 50;

    // return un GameObject prefab ou null apres 5 tentatives
    public static GameObject Instantier(GameObject prefab, int tentatives)
    {
        for (int i = 0; i < tentatives; i++)
        {
            GameObject prefabInstance = ApparaitreAuHasard(prefab); // la methode return  GameObject | null

            if (prefabInstance != null) return prefabInstance;
        }

        return null; // rien n'est retourne, car plus aucun espace disponible
    }

    // utilise ApparaitreSiLibre() et qui lui specifie une position au hasard dans le niveau
    public static GameObject ApparaitreAuHasard(GameObject prefab)
    {
        // on garde une reference au rayon du collider du prefab
        float radius = prefab.GetComponent<CapsuleCollider>().radius;

        Vector3 position = GetPositionRandom(radius);

        return ApparaitreSiLibre(prefab, position); // la methode return GameObject | null
    }

    // Fait apparaitre le prefab <=> position specifie est libre
    public static GameObject ApparaitreSiLibre(GameObject prefab,Vector3 position)
    {
        // on garde une reference au rayon du collider du prefab
        float radius = prefab.GetComponent<CapsuleCollider>().radius;
        
        // On prend 0.9*le radius pour éviter de détecter le plancher dans la collision
        if (Physics.CheckSphere(position, radius * 0.9f))
        {
            // Déjà un objet à l'endroit prévu
            return null;
        }

        // Rien à cet endroit : on peut Instantiate() le prefab
        return Object.Instantiate(prefab, position, Quaternion.identity);
    }

    public static Vector3 GetPositionRandom(float rayon)
    {
        // 50 -> _plan : on enleve les nombres magiques
        Vector3 position = new Vector3(0f, rayon, 0f);
        position.x = UnityEngine.Random.Range(-_plan, _plan);
        position.z = UnityEngine.Random.Range(-_plan, _plan);

        return position;
    }

}
