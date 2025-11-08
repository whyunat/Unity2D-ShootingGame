using Singleton.Component;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : SingletonComponent<UIManager>
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private bool isMenuOpen = false;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI expText;

    [Header("Skill UI")]
    [SerializeField] private bool isSkillTreeOpen = false;
    [SerializeField] private GameObject skillTreePanel;

    [Header("Escape Menu UI")]
    [SerializeField] private bool isEscapeMenuOpen = false;
    [SerializeField] private GameObject escapeMenuPanel;

    [Header("DeathMessage UI")]
    [SerializeField] private bool isDeathMessageOpen = false;
    [SerializeField] private GameObject deathMessagePanel;
    [SerializeField] private TextMeshProUGUI killScoreText;

    [Header("Skill Tree")]
    [SerializeField] private SkillTree[] skillTree;

    [Header("Clear Message UI")]
    [SerializeField] private GameObject ClearMessage;

    public bool IsMenuOpen { get => isMenuOpen; }
    public Slider HealthBar { get => healthBar; set => healthBar = value; }
    public bool IsSkillTreeOpen { get => isSkillTreeOpen; }
    public bool IsEscapeMenuOpen { get => isEscapeMenuOpen; }
    public bool IsDeathMessageOpen { get => isDeathMessageOpen; }

    #region Singleton
    protected override void AwakeInstance()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Initialize();
    }

    protected override bool InitInstance()
    {
        return true;
    }

    protected override void ReleaseInstance()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }
    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            ReleaseSingleton();
        }
    }

    public void SetExp()
    {
        expText.text = $"경험치: {GameManager.Instance.Experience}";
    }

    public void ToggleMenu()
    {
        if (isMenuOpen)
        {
            isMenuOpen = !isMenuOpen;

            switch (true)
            {
                case bool _ when isSkillTreeOpen:
                    ToggleSkillTree();
                    break;
                case bool _ when isEscapeMenuOpen:
                    ToggleEscapeMenu();
                    break;
                default:
                    break;
            }
        }
    }

    public void ToggleSkillTree()
    {
        isSkillTreeOpen = !isSkillTreeOpen;
        SetBooleans(skillTreePanel, isSkillTreeOpen);
    }

    public void ToggleEscapeMenu()
    {
        isEscapeMenuOpen = !isEscapeMenuOpen;
        SetBooleans(escapeMenuPanel, isEscapeMenuOpen);
    }

    public void ToggleDeathMessage()
    {
        isDeathMessageOpen = !isDeathMessageOpen;
        SetBooleans(deathMessagePanel, isDeathMessageOpen);

        if (isDeathMessageOpen)
        {
            killScoreText.text = $"몬스터 킬 수: {GameManager.Instance.KillScore}";
        }
    }

    private void SetBooleans(GameObject menu, bool menuBool)
    {
        isMenuOpen = menuBool;
        menu.GetComponent<Canvas>().enabled = menuBool;
        GameManager.Instance.SetPause(menuBool);
    }

    public void ReturnToMain()
    {
        ToggleMenu();
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ResetAllSkillsColor()
    {
        for (int i = 0; i < skillTree.Length; i++)
        {
            skillTree[i].SetOriginSkilColor();
        }
    }
    public void ShowClearText()
    {
        ClearMessage.SetActive(true);
    }

    public void HideClearText()
    {
        ClearMessage.SetActive(false);
    }


}
