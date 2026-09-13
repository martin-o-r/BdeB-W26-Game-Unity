using UnityEngine;

public class EtatAttente : EtatEnnemi
{
    public EtatAttente(ComportementEnnemi comportementEnnemi) : base(comportementEnnemi)
    { }

    private float tempsAttente;

    public override void Entrer()
    {
        sujet.agent.isStopped = true;
        tempsAttente = Random.Range(3f, 5f);
    }

    public override void Executer(float deltaTime)
    {
        tempsAttente -= Time.deltaTime;
        if (tempsAttente < 0f)
        {
            sujet.ChangerEtat(sujet.etatPoursuite);
        }
    }   
}
