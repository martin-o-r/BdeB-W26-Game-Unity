using UnityEngine;

public class EtatAttaque : EtatEnnemi
{
    public EtatAttaque(ComportementEnnemi comportementEnnemi) : base(comportementEnnemi)
    { }

    private float tempsAttente = 0f;

    private int numeroAttaque;

    public override void Entrer()
    {
        sujet.agent.isStopped = true;
        tempsAttente = 0f;
        numeroAttaque = Random.Range(1, 4);
        sujet.animateur.SetBool("Attaque" + numeroAttaque, true);
    }

    public override void Executer(float deltaTime)
    {
        //Vector3 positionJoueur = sujet.scriptJoueur.gameObject.transform.position;
        Vector3 positionCible = sujet.CibleCourante.Position;

        Vector3 positionEnnemi = sujet.gameObject.transform.position;
        float distanceJoueur = (positionCible - positionEnnemi).magnitude;

        if (sujet.APeur())
        {
            sujet.ChangerEtat(sujet.etatFuite);
            return;
        }
        
        // Change a etat de poursuite
        if (distanceJoueur > sujet.distanceAttaqueToPoursuite + sujet.distancePoursuiteToAttaque)
        {
            sujet.ChangerEtat(sujet.etatPoursuite);
            return;
        }

        // Sinon fait une attaque
        sujet.gameObject.transform.LookAt(positionCible);

        tempsAttente += deltaTime;
        if (tempsAttente >= sujet.tempsEntreAttaque)
        {
            Attaquer();
            tempsAttente = 0f;
        }
    }

    public override void Sortir()
    {
        sujet.animateur.SetBool("Attaque" + numeroAttaque, false);
    }

    private void Attaquer()
    {
        sujet.CibleCourante.SeFaireAttaquer(sujet.dommages);
    }
}
