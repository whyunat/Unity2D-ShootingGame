using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject startGameBtn;

    void Start()
    {
        eventSystem.SetSelectedGameObject(startGameBtn);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Stage0");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
