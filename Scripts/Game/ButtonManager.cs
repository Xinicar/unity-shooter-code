using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Slider _slider;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _playUI;
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int sceneIndex)
    {
        _loadingScreen.SetActive(true);
        StartCoroutine(LoadGame(sceneIndex));
    }

    IEnumerator LoadGame(int index)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(index);
        while (!loadOperation.isDone)
        {
            float progressiveValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            _slider.value = progressiveValue;
            yield return null;
        }
    }

    public void ShowObj(GameObject obj)
    {
        StartCoroutine(ShowUI(obj));
    }

    public void ChangeCamera(GameObject hideObj)
    {
        hideObj.SetActive(false);
    }

    private IEnumerator ShowUI(GameObject showGO)
    {
        yield return new WaitForSeconds(1.5f);
        showGO.SetActive(true);
    }

    public void PauseGame()
    {
        _playUI.SetActive(false);
        _pauseMenu.SetActive(true);
        GameManager.Instace.FPSText().enabled = false;
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        GameManager.Instace.FPSText().enabled = true;
        _playUI.SetActive(true);
        _pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
