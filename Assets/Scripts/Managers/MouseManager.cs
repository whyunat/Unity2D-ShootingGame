using Singleton.Component;
using UnityEngine;


public class MouseManager : SingletonComponent<MouseManager>
{
    public Vector3 mousePos;

    #region Singleton
    protected override void AwakeInstance()
    {

    }

    protected override bool InitInstance()
    {
        return true;
    }

    protected override void ReleaseInstance()
    {

    }
    #endregion

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        mousePos.z = 0;
        transform.position = mousePos;

        Debug.Log(transform.position);
    }
}