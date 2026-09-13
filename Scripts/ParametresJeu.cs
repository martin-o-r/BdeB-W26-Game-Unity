public class ParametresJeu
{
    public static ParametresJeu Instance { get; } = new ParametresJeu();

    public int VieDepart { get; private set; } = 14;
    public int ExperienceRequise { get; private set; } = 30;
    public int EnnemisParVague { get; private set; } = 5;

    // Selon la difficulte choisie, le taux d'argent gagne par ennemis vaincus sera different
    public int ArgentTauxDifficulte { get; private set; } = 1;

    private IStrategieSpawn[] _strategiesSpawn = new IStrategieSpawn[] { new StrategieHasard() };
    public IStrategieSpawn[] StrategiesSpawn => _strategiesSpawn;

    private ParametresJeu() { }

    public void ModeFacile()
    {
        VieDepart = 14;
        ExperienceRequise = 30;
        EnnemisParVague = 5;

        ArgentTauxDifficulte = 1;

        _strategiesSpawn = new IStrategieSpawn[]
        {
            new StrategieHasard()
        };
    }

    public void ModeDifficile()
    {
        VieDepart = 8;
        ExperienceRequise = 50;
        EnnemisParVague = 10;

        ArgentTauxDifficulte = 2;

        _strategiesSpawn = new IStrategieSpawn[]
        {
            new StrategieHasard(),
            new StrategieCercle(),
            new StrategieLigne()
        };
    }
}