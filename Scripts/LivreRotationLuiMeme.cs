using UnityEngine;
/*
 * https://www.reddit.com/r/Unity2D/comments/32l6tu/how_do_i_rotate_an_object_around_its_center/
 *      -> on peut overload transform.rotate(Vector3.up, Space.World)
 * https://docs.unity3d.com/ScriptReference/Transform.Rotate.html
 *      -> transform.rotate() possede des overloads
 *      -> transform.rotate(degre de rotation, referece de rotation)
 *      -> par defaut, la propriete de rotation fait reference a Space.Self, ce qui veut dire a lui meme ou a l'objet attache
 * https://docs.unity3d.com/ScriptReference/Space.World.html
 *      -> on peut utiliser Space.World pour referencer la rotation a Space.World
 */

/// <summary>
/// Ce script est utilise que pour la rotation sur son propre axe y du livre possede.
/// </summary>

public class LivreRotationLuiMeme : MonoBehaviour
{
    [SerializeField]
    private float _rotationVitesse = 100f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * _rotationVitesse * Time.deltaTime, Space.World);
    }
}
