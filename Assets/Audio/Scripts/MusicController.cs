using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    [HideInInspector] public static MusicController instance = null;

    [Header("Music")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _gameMusic;

    private AudioSource _audioSource;

    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void Initialize()
    {
        _audioSource = GetComponent<AudioSource>();
        ChangeToMenuMusic();
    }
    public void ChangeToMenuMusic()
    {
        _audioSource.clip = _menuMusic;
        _audioSource.Play();
    }
    public void ChangeToGameMusic()
    {
        _audioSource.clip = _gameMusic;
        _audioSource.Play();
    }
}
