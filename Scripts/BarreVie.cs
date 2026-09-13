using UnityEngine;

public class BarreVie : MonoBehaviour
{
    [SerializeField]
    private RectTransform devantTransform;
    
    public void SetPourcentage(float pourcentage)
    {
        devantTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pourcentage);
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
