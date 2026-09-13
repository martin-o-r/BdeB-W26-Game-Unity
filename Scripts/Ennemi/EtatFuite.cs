using UnityEngine;

public class EtatFuite : EtatEnnemi
{
    public EtatFuite(ComportementEnnemi comportementEnnemi) : base(comportementEnnemi) { }


    public override void Entrer()
    {
        sujet.agent.isStopped = false;
        sujet.animateur.SetBool("Fuite", true);
        sujet.agent.speed *= 2;
    }

    public override void Executer(float deltaTime)
    {
        var directionJoueur = sujet.scriptJoueur.transform.position - sujet.transform.position;
        
        directionJoueur.Normalize();
        
        // Objectif : partir en direction inverse du joueur
        sujet.agent.SetDestination(sujet.transform.position - directionJoueur * 10);
    }
}