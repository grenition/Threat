using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    public static AudioController instance;
    public static AudioController controllerPrefab;

    [Header("Sounds")]
    [SerializeField] private AudioClip _buttonSound;
    [SerializeField] private AudioClip _wrongSound;
    [SerializeField] private AudioClip _rightSound;
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _closeSound;
    [SerializeField] private AudioClip _cleanSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _loseSound;

    private AudioSource _audioSource;
 
    void Awake()
    {
        InitializeSingletone(this);
        _audioSource = GetComponent<AudioSource>();
    }
    public static void Silent()
    {
        if (!CheckSingletone())
            return;
        instance._audioSource.Stop();
    }
    private static void InitializeSingletone(AudioController controller = null)
    {
        if (instance != null) 
        {
            if (controller != null)
                Destroy(controller.gameObject);
            return;
        }
        if(controller != null)
        {
            DontDestroyOnLoad(controller.gameObject);
            instance = controller;
            return;
        }

        AudioController audioPrefab = Resources.Load<AudioController>("Systems/AudioController");
        if (audioPrefab == null)
            return;

        AudioController spawnedAudioController = Instantiate(audioPrefab);
        DontDestroyOnLoad(spawnedAudioController.gameObject);
        instance = spawnedAudioController;
    }

    private static bool CheckSingletone()
    {
        if (instance == null)
        {
            InitializeSingletone();
            if (instance == null)
                return false;
        }
        return true;
    }
    public static void PlaySound(AudioClip clip)
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(clip);
    }
    public static void PlayButtonSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._buttonSound);
    }
    public static void PlayWrongSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._wrongSound);
    }
    public static void PlayRightSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._rightSound);
    }
    public static void PlayOpenSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._openSound);
    }
    public static void PlayCloseSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._closeSound);
    }
    public static void PlayCleanSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._cleanSound);
    }
    public static void PlayWinSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._winSound);
    }
    public static void PlayLoseSound()
    {
        if (!CheckSingletone())
            return;

        instance._audioSource.PlayOneShot(instance._loseSound);
    }
}
