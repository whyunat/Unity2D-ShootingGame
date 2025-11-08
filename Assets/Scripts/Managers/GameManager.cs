using Singleton.Component;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonComponent<GameManager>
{
    [SerializeField] private bool isGamePaused = false;
    [SerializeField] private int experience = 0; //플레이어 경험치
    [SerializeField] private int killScore = 0; //몬스터 킬 수

    [SerializeField] private GameObject portalPrefab; // 포털 프리팹

    private bool isStageCleared = false;

    [SerializeField] private int currentStage = 0; // 현재 스테이지
    private const int maxStage = 6; // 총 스테이지 수

    public Vector2 portalSpawnPosition; // 포털 생성 위치

    bool isLoading = false;

    public bool IsGamePaused { get => isGamePaused; }
    public int Experience { get => experience; }
    public int TotalMonsterCount { get; set; } = 0; //spawn manager에서 += 해줍니다.
    //total monster count랑 kill score 비교해서 같으면 clear인걸로 해두긴 했습니다. 더 좋은 방법 있을 겁니다.
    public int KillScore { get => killScore; set => killScore = value; }

    private PlayerInitStatus playerInitStatus;

    #region Singleton
    protected override void AwakeInstance()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        playerInitStatus = new PlayerInitStatus
        {
            atkPower = 1,
            health = 5,
            maxHealth = 2,
            atkSpeed = 1,
            evasionChance = 0,
            projectileSpeed = 10,
            knockbackPower = 1,
            projectilePenetration = 1
        };

        Initialize();
    }

    protected override bool InitInstance()
    {
        return true;
    }

    protected override void ReleaseInstance()
    {
        StopAllCoroutines();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Destroy(gameObject);
    }
    #endregion

    public void SetPause(bool pauseBool)
    {
        isGamePaused = pauseBool;
        Time.timeScale = isGamePaused ? 0f : 1f;
    }

    public void UpdateExp(int amount)
    {
        experience += amount;
        UIManager.Instance.SetExp();
    }

    public void UpdatekillScore(int score)
    {
        killScore += score;
        UIManager.Instance.ToggleDeathMessage();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //CreatePortalForCurrentStage();
        if (scene.name == "MainMenu")
        {
            currentStage = 0;
            ReleaseSingleton();
        }
        else if(scene.name == "Stage0")
        {
            StartGame();
        }
        else
        {
            if (killScore < 101)
            {
                SpawnManager.Instance.SpawnMonsters(currentStage);
            }
            else
            {
                SpawnManager.Instance.SpawnMonsters(currentStage, isBoss: true);
            }
        }
        AudioManager.Instance.BgmController(currentStage);
        StartCoroutine(CreatePortalForCurrentStage());
    }

    public IEnumerator CreatePortalForCurrentStage()
    {
        Debug.Log($"tot : {TotalMonsterCount}");
        isStageCleared = TotalMonsterCount == KillScore;
        while (!isStageCleared)
        {
            isStageCleared = TotalMonsterCount == KillScore;
            if (killScore == 100) SpawnManager.Instance.SpawnBoss();
            yield return null;
        }
        yield return null;
        if (currentStage <= maxStage)
        {
            // 포털 프리팹을 현재 스테이지 위치에 생성
            Instantiate(portalPrefab, portalSpawnPosition, Quaternion.identity);
            Debug.Log($"Stage {currentStage}에 포털이 생성되었습니다.");
            UIManager.Instance.ShowClearText();
        }
    }

    public void LoadNextStage()
    {
        //클리어 안됐을 때 포탈 생성 안 되게하면 코루틴으로 안해도됨.
        if (!isLoading)
        {
            StartCoroutine(LoadSceneRoutine());
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        isLoading = true;

        if (currentStage < maxStage)
        {
            currentStage++;
            string nextScene = $"Stage{currentStage}";
            Debug.Log($"다음 스테이지로 이동: {nextScene}");

            yield return SceneManager.LoadSceneAsync(nextScene);
            UIManager.Instance.HideClearText();
        }
        else
        {
            Debug.Log("모든 스테이지 클리어!");
        }

        yield return new WaitForSeconds(1f);
        isLoading = false;
    }

    private void StartGame()
    {
        if (!Player.Instance.isAlive)
        {
            UIManager.Instance.ToggleDeathMessage();
            UIManager.Instance.ResetAllSkillsColor();
        }
        SpawnManager.Instance.StopRoutine();
        SkillManager.Instance.ResetAllSkills();

        //experience = 0;
        KillScore = 0;
        TotalMonsterCount = 0;
        UpdateExp(-experience);

        Player.Instance.transform.position = new Vector3(0, 0, 0);
        Player.Instance.isAlive = true;
        Player.Instance.atkPower = playerInitStatus.atkPower;
        Player.Instance.health = playerInitStatus.health;
        Player.Instance.maxHealth = playerInitStatus.maxHealth;
        Player.Instance.atkSpeed = playerInitStatus.atkSpeed;
        Player.Instance.evasionChance = playerInitStatus.evasionChance;
        Player.Instance.projectileSpeed = playerInitStatus.projectileSpeed;
        Player.Instance.knockbackPower = playerInitStatus.knockbackPower;
        Player.Instance.projectilePenetration = playerInitStatus.projectilePenetration;
        Player.Instance.SetMaxHealth(playerInitStatus.health);
    }
}
