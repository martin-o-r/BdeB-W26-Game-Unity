using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ComportementPersonnage : MonoBehaviour, ICible
{
    [SerializeField] 
    private float vitesseBase = 10f;
    [SerializeField] 
    private float vitesseRotation = 180f;

    private int vies;
    private int maxVies;
    private int experience = 0;

    private int experienceRequise;

    [SerializeField] 
    private TMP_Text affichagePointsXp;
    [SerializeField] 
    private BarreVie barreVie;
    [SerializeField] 
    private VisualEffectAsset[] vfx;

    // pas besoin de l'utilier techniquement pour Olivia, c'est pour respecter l'interface ICible
    public event Action OnCibleDetruite; 
    public event Action OnAmeliorationAtteinte;

    [SerializeField] 
    private ScriptAttaque scriptAttaque;
    public int Dommages => scriptAttaque.dommages;

    // Provient de l'interface ICible, renvoi la position d'Olivia
    public Vector3 Position => transform.position;

    private InputAction mouvementAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    private Animator animator;
    private CharacterController characterController;

    private Quaternion targetRotation;

    private AnimationViePerdue animationViePerdue;

    [SerializeField] 
    private ComportementCamera comportementCamera;

    private GestionnaireLeurre _gestionnaireLeurre;

    private void OnEnable()
    {
        OrbRemplirVie.OnOrbVieAttrape += RedonnerMaxVies;
        
        AmeliorationDommageAttaques.OnAmeliorerDommageAttaques += AmeliorerDommageAttaqueDeBase;
    }

    private void OnDisable()
    {
        OrbRemplirVie.OnOrbVieAttrape -= RedonnerMaxVies;
        
        AmeliorationDommageAttaques.OnAmeliorerDommageAttaques -= AmeliorerDommageAttaqueDeBase;
    }

    void Start()
    {
        mouvementAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        jumpAction = InputSystem.actions.FindAction("Jump");

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        vies = ParametresJeu.Instance.VieDepart;
        maxVies = ParametresJeu.Instance.VieDepart;
        
        experienceRequise = ParametresJeu.Instance.ExperienceRequise;

        animationViePerdue = GetComponent<AnimationViePerdue>();

        // on recupere le component pour pouvoir activer le leurre
        _gestionnaireLeurre = GetComponent<GestionnaireLeurre>();
    }

    void Update()
    {
        bool isRunning = sprintAction.IsPressed();
        Vector2 mouvement = mouvementAction.ReadValue<Vector2>();

        float vitesseApplique = vitesseBase;
        if (isRunning) vitesseApplique *= 2f;

        Vector3 mouvementApplique = new Vector3(mouvement.x, 0f, mouvement.y) * vitesseApplique;
        characterController.SimpleMove(mouvementApplique);

        if (mouvementApplique.magnitude > 0)
        {
            targetRotation = Quaternion.LookRotation(mouvementApplique.normalized);
        }

        transform.rotation =
            Quaternion.RotateTowards(transform.rotation, targetRotation, vitesseRotation * Time.deltaTime);

        animator.SetFloat("Vitesse", mouvementApplique.magnitude);

        RefreshBarreVie();

        if (jumpAction.WasPressedThisFrame())
        {
            _gestionnaireLeurre.ActiverLeurre();
        }
    }

    private void RefreshBarreVie()
    {
        barreVie.SetPourcentage(vies * 100f / maxVies);
    }

    public void GagnerVies(int nb)
    {
        vies += nb;
        if (vies > maxVies)
            vies = maxVies;
        
        RefreshBarreVie();
    }

    public void PerdreVies(int dommage)
    {
        vies -= dommage;
        RefreshBarreVie();

        if (vies <= 0) OnCibleDetruite?.Invoke();

        animationViePerdue.Demarrer();
        comportementCamera.Brasser();
    }

    public void GagnerExperience(int experienceGagne)
    {
        ChangerExperience(experience + experienceGagne);

        if (experience >= experienceRequise)
        {
            OnAmeliorationAtteinte.Invoke();
        }
    }

    public void AmeliorerVie(int vieGagne)
    {
        maxVies += vieGagne;
        vies = maxVies;
        RefreshBarreVie();

        ChangerExperience(0);
    }

    public void AmeliorerRayon(int rayonGagne)
    {
        scriptAttaque.AmeliorerRayon(rayonGagne);
        ChangerExperience(0);
    }

    public void AmeliorerDommage(int amelioration)
    {
        scriptAttaque.AmeliorerRapidite(amelioration);
        ChangerExperience(0);
    }

    // Amelioration de la vitesse de deplacement d'Olivia
    public void AmeliorerCourse(float amelioration)
    {
        vitesseBase *= amelioration;
        ChangerExperience(0);
    }

    // Amelioration du dommage fait par les attaques de base d'Olivia
    private void AmeliorerDommageAttaqueDeBase(int amelioration)
    {
        scriptAttaque.AmeliorerDommageDeBase(amelioration);
        ChangerExperience(0);
    }

    private void ChangerExperience(int nombreExperience)
    {
        experience = nombreExperience;
        affichagePointsXp.text = experience.ToString();
    }

    public void RedonnerMaxVies()
    {
        vies = maxVies;
        RefreshBarreVie();
    }

    // La methode qui fait perdre des vies provient de l'interface ICible, elle est appelee par les ennemis quand ils attaquent Olivia
    public void SeFaireAttaquer(int dommage)
    {
        PerdreVies(dommage);
    }
}