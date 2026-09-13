using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * https://discussions.unity.com/t/make-music-continue-playing-through-scenes/175434
 *      -> utiliser Object.DontDestroyOnLoad() pour preserver un objet a travers plusieurs scenes
 * 
 * https://docs.unity3d.com/6000.4/Documentation/ScriptReference/Object.DontDestroyOnLoad.html
 *      -> Object.DontDestroyOnLoad()
 *      -> permet de ne pas detruire un objet lorsqu'on change de scenes
 *      
 * https://discussions.unity.com/t/singleton-creating/570306/4
 *      -> implementation d'un singleton + Obecjt.DontDestroyOnLoad()
 *      -> verification qu'il n'y ait qu'une seule instance d'un singleton AudioManager
 *      -> permet de s'assurer que si on revient a la scene contenant l'objet contenant le AudioSource, celui-ci ne soit pas dupliquer
 *      
 * https://docs.unity3d.com/ScriptReference/AudioSource-volume.html
 *      -> AudioSource.volume permet de { get; set; } le niveau de volume
 *      
 * Suggestion de l'enseignant :
 *      -> private vs public Instance
 *          -> private : seulement la classe du singleton a le droit de manipuler l'audio, peut etre contraignant dans un grand developpement
 *          -> public : on peut manipuler l'audio a partir d'une autre classe
 *      -> preferable d'utiliser public, car on veut le plus de scalability possible dans le future
 *      -> ex : on pourrait vouloir arreter/changer la musique lorsqu'on termine le jeu
 *      
 * Bug rencontre : 
 *      -> lorsqu'une partie est terminee, on retourne au menu principal, mais le slider intial n'est plus "connecte" avec le audioSource, car l'objet Slider a ete detruit lors du changement de scene!!
 *      https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnEnable.html
 *          -> OnEnable();
 *              - methode appele lorsque le component dans un GameObject est "construit" dans une scene
 *              - dans notre cas, ca permet de determiner quand on revient a la scnene principal
 *      https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDisable.html
 *          -> OnDisable();
 *              - methode qui roule lorsque l'objet parent est desative/detruit
 *              - dans notre cas, on l'utilise avec le SceneManager.sceneLoaded
 *              - permet de retirer l'appel a la methode TrouverSliderDansScene() (bug principal)
 *              - si un joueur, joueu plusieurs fois, il peut y avoir un bug, car la meme est appele constamment
 *      https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager-sceneLoaded.html
 *          -> SceneManager.sceneLoaded
 *          -> on peut developper du code qui s'active lorsque la scene contenant le script est chargee (principe de callbakcs)
 *          -> dans notre cas, lorsque la scene est chargee, on s'assure d'aller chercher le slider qui controle le volume
 *      https://docs.unity3d.com/540/Documentation/ScriptReference/UI.Slider-onValueChanged.html
 *          -> Slider.onValueChanged
 *              - principe d'un listener
 *          -> fait rouler une methode lorsqu'un changement est detecte par le slider
 */

/// <summary>
/// Cette classe sera un singleton afin de preserver l'objet qui contient le AudioSource
/// Elle permettra :
///     - de fixer le niveau de volume dans le menu
///     - garder le niveau fixe a travers l'entierete du jeu (changement de scene)
///     - faire la sauvegarde du niveau de de volume avec PlayerPrefs
/// </summary>

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource _audioSource;

    private Slider _audioSlider;

    [SerializeField]
    private float _volumeParDefaut = 0.8f;

    void Awake()
    {
        // Verification qu'il existe qu'un seul singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();

        float volumeSauvegarde = PlayerPrefs.GetFloat("musiqueVolume", _volumeParDefaut);
        _audioSource.volume = volumeSauvegarde;
    }

    // Lorsque la scene du menu principal est chargee
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ChargementDeScene;
    }

    // Lorsqu'on "quitte" la scene du menu principal
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ChargementDeScene;
    }

    // Lorsque la scene contenant this script est charge, cette methode s'active
    private void ChargementDeScene(Scene scene, LoadSceneMode mode)
    {
        TrouverSliderDansScene();
    }

    // Methode qui permet de trouver le slider dans la scene avec son tag
    private void TrouverSliderDansScene()
    {
        // On s'assure de ne pas chercher "l'ancien" slider avant de continuer
        if (_audioSlider != null)
        {
            _audioSlider.onValueChanged.RemoveListener(VolumeAEteChange);
            _audioSlider = null;
        }

        // si la scene suivant ne possede pas de slider, alors on sort de la methode
        GameObject sliderObjet = GameObject.FindWithTag("VolumeSlider");
        if (sliderObjet == null) return;

        _audioSlider = sliderObjet.GetComponent<Slider>();

        _audioSlider.value = _audioSource.volume;

        _audioSlider.onValueChanged.AddListener(VolumeAEteChange);
    }

    // Methode qui set le volume a la suite d'un changement detecte par le slider
    private void VolumeAEteChange(float volume)
    {
        _audioSource.volume = volume;

        PlayerPrefs.SetFloat("musiqueVolume", volume);
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("musiqueVolume", _audioSource.volume);
        PlayerPrefs.Save();
    }

}
