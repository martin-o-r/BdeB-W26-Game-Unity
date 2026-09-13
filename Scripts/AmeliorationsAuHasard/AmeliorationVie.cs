using UnityEngine;

public class AmeliorationVie : StrategieAmelioration
{
    public override string NomAmelioration => $"Vie + {ValeurAmelioration}";

    public AmeliorationVie(float valeurAmelioration) : base(valeurAmelioration) { }

    public override void Ameliorer(ComportementPersonnage personnage)
    {
        personnage.AmeliorerVie((int)ValeurAmelioration);
    }
}
