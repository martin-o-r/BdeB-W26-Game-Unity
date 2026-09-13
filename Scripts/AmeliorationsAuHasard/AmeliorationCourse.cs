using UnityEngine;

public class AmeliorationCourse : StrategieAmelioration
{
    public override string NomAmelioration => $"Course * {ValeurAmelioration}";

    public AmeliorationCourse(float valeurAmelioration) : base(valeurAmelioration) { }

    public override void Ameliorer(ComportementPersonnage personnage)
    {
        personnage.AmeliorerCourse(ValeurAmelioration);
    }
}
