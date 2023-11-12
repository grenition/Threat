using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void OpenScene(int index)
    {
        MusicController.instance.ChangeToGameMusic();
        SceneManager.LoadScene(index);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void PlayButtonSound()
    {
        AudioController.PlayButtonSound();
    }
}
