using Singleton.Component;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : SingletonComponent<PoolManager>
{
    //프리팹 이름을 키로 사용하는 딕셔너리
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    #region Singleton
    protected override void AwakeInstance()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        Initialize();
    }

    protected override bool InitInstance()
    {
        return true;
    }

    protected override void ReleaseInstance()
    {
        foreach (var item in pools)
        {
            item.Value.Reset();
        }
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
    #endregion

    //새로운 오브젝트 풀을 생성하는 메서드
    //prefab: 풀링할 프리팹, initialSize: 초기 풀 크기
    public void CreatePool(GameObject prefab, int initialSize)
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
        {
            pools.Add(key, new ObjectPool(prefab, initialSize, transform));
        }
    }

    //풀에서 오브젝트를 가져오는 메서드
    //요청한 프리팹의 풀이 없다면 새로 생성
    public GameObject Get(GameObject prefab)
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
        {
            CreatePool(prefab, 10);
        }
        return pools[key].Get();
    }

    //사용이 끝난 오브젝트를 풀로 반환하는 메서드
    public void Return(GameObject obj)
    {
        string key = obj.name.Replace("(Clone)", "");
        if (pools.ContainsKey(key))
        {
            pools[key].Return(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        foreach(var item in pools)
        {
            item.Value.Reset();
        }
    }
}
