using UnityEngine;

public class ManualController : MonoBehaviour
{
    [SerializeField] private GameObject _manualPanel;
    [SerializeField] private GameObject _previewPanel;

    private void Start()
    {
        if (_previewPanel != null && PlayerPrefs.GetInt("Manual_Preview_showed") != 0)
            _previewPanel.SetActive(false);
    }
    public void ChangeManualActiveState()
    {
        if (_manualPanel.gameObject.activeSelf)
            CloseManual();
        else
            OpenManual();
    }
    public void OpenManual()
    {
        _manualPanel.gameObject.SetActive(true);
        TimerController.Pause();
        TimeController.Pause();

        if (_previewPanel != null)
        {
            PlayerPrefs.SetInt("Manual_Preview_showed", 1);
            if (_previewPanel.TryGetComponent(out UI_ResizingAnimationOnAwake _prevAnim))
                _prevAnim.CloseWithAnimation();
            else
                _previewPanel.SetActive(false);
        }
    }

    public void CloseManual()
    {
        if (_manualPanel.TryGetComponent(out UI_ResizingAnimationOnAwake _manualAnim))
            _manualAnim.CloseWithAnimation();
        else
            _manualPanel.SetActive(false);

        TimerController.UnPause();
        TimeController.UnPause();
    }
}
