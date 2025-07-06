using UnityEngine;

public class MonoEditorSingleton<T> : MonoEditor
    where T : MonoEditor
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();

                if (instance == null)
                    instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }
            return instance;
        }
    }

    public override void Start()
    {
        base.Start();
#if UNITY_EDITOR
        onExitPlayModeEvent += () => instance = null;
#endif
    }
}
