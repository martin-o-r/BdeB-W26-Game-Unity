using UnityEngine;

public class ComportementCamera : MonoBehaviour
{
    [SerializeField] private GameObject objet;

    [SerializeField] private Vector3 offset;

    private float dureeBrassage = 0;

    void Start()
    {
        if (offset == Vector3.zero)
        {
            offset = transform.position - objet.transform.position;
        }
    }

    public void Brasser()
    {
        dureeBrassage = 0.15f;
    }

    public void ArreterBrassage()
    {
        dureeBrassage = 0;
    }

    void LateUpdate()
    {
        transform.position = objet.transform.position + offset;

        if (dureeBrassage > 0)
        {
            transform.position += new Vector3(
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.15f, 0.15f));
            dureeBrassage -= Time.deltaTime;
        }
    }
}