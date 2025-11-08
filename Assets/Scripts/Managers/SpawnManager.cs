using Singleton.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Enum의 순서로 넣어주세요
//shoom, s1_slimem, ... S5_Slime, Bat, Dragon
public enum MonsterType
{
    Shoom, S1_Slime, //1
    GoblinTNT, GoblinBarrel, //2
    Ghoul, Skeleton, //3
    Spider, Golem, //4
    S5_Slime, Bat, Dragon //5
}

public class SpawnManager : SingletonComponent<SpawnManager>
{
    //public GameObject monster;
    //public GameObject monster2;
    [Header("Enum의 순서로 넣어주세요.")]
    [SerializeField] private GameObject[] monsterPrefabs;// 몬스터 프리팹 리스트 (스테이지 순서로 몬스터 넣기)
    private Dictionary<int, MonsterType[]> stageMonsters;

    #region Singleton
    protected override void AwakeInstance()
    {
        //PoolManager.Instance.CreatePool(monster, 10);
        //PoolManager.Instance.CreatePool(monster2, 10);
        //StartCoroutine(MonsterSpawn(monster, 10));
        //StartCoroutine(MonsterSpawn(monster2, 10));

        SceneManager.sceneLoaded += OnSceneLoaded;

        stageMonsters = new Dictionary<int, MonsterType[]>
        {
            { 1, new MonsterType[] { MonsterType.Shoom, MonsterType.S1_Slime } },
            { 2, new MonsterType[] { MonsterType.GoblinTNT, MonsterType.GoblinBarrel } },
            { 3, new MonsterType[] { MonsterType.Ghoul, MonsterType.Skeleton } },
            { 4, new MonsterType[] { MonsterType.Spider, MonsterType.Golem } },
            { 5, new MonsterType[] { MonsterType.Bat, MonsterType.S5_Slime } }
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            ReleaseSingleton();
        }
    }

    public void SpawnMonsters(int currentStage, bool isBoss = false, int spawnCount = 10, float delay = 2f, int amount = 1)
    {
        if (!stageMonsters.ContainsKey(currentStage))
        {
            Debug.LogWarning("잘못된 스테이지 번호");
            return;
        }

        MonsterType[] monsters = stageMonsters[currentStage];

        foreach (MonsterType monsterType in monsters)
        {
            GameObject monsterPrefab = GetMonsterPrefab(monsterType);
            if (monsterPrefab != null)
            {
                StartCoroutine(MonsterSpawn(monsterPrefab, spawnCount, delay, amount));
            }
            GameManager.Instance.TotalMonsterCount += spawnCount;
        }
        if (isBoss) GameManager.Instance.TotalMonsterCount++;
    }

    private GameObject GetMonsterPrefab(MonsterType type)
    {
        if ((int)type >= monsterPrefabs.Length)
        {
            Debug.LogWarning($"프리팹 인덱스 초과: {type}");
            return null;
        }
        return monsterPrefabs[(int)type];
    }

    //몬스터 스폰 코루틴
    IEnumerator MonsterSpawn(GameObject prefab, int count, float delay = 2f, int amount = 1)
    {
        while(count > 0)
        {
            yield return new WaitForSeconds(delay);
            for(int i = 0; i < amount; i++)
            {
                GameObject mob = PoolManager.Instance.Get(prefab);
                mob.transform.position = GetRandomPosition();
                mob.GetComponent<Monster>().Initiate();
            }
            count--;
        }
    }

    public void SpawnBoss()
    {
        Instantiate(monsterPrefabs[(int)MonsterType.Dragon], new Vector3(0,0,0), Quaternion.identity);
    }

    //무작위 스폰 위치를 반환하는 메서드
    private Vector2 GetRandomPosition()
    {
        Vector2 randomPosition = Vector2.zero;
        float min = 0.05f;
        float max = 0.95f;

        int flag = Random.Range(0, 4);
        switch (flag)
        {
            case 0:
                randomPosition = new Vector2(max, Random.Range(min, max));
                break;
            case 1:
                randomPosition = new Vector2(min, Random.Range(min, max));
                break;
            case 2:
                randomPosition = new Vector2(Random.Range(min, max), max);
                break;
            case 3:
                randomPosition = new Vector2(Random.Range(min, max), min);
                break;
        }
        randomPosition = Camera.main.ViewportToWorldPoint(randomPosition);

        return randomPosition;
    }

    public void StopRoutine()
    {
        StopAllCoroutines();
    }
}