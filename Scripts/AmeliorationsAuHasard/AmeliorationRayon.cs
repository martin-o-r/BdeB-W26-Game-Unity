using UnityEngine;

public class AmeliorationRayon : StrategieAmelioration
{
    public override string NomAmelioration => $"Rayon + {ValeurAmelioration}";

    public AmeliorationRayon(float valeurAmelioration) : base(valeurAmelioration) { }

    public override void Ameliorer(ComportementPersonnage personnage)
    {
        personnage.AmeliorerRayon((int)ValeurAmelioration);
    }
}
