using UnityEngine;

public class PrefabGetComponentTest2 : MonoBehaviour
{
    public GameObject prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var test = prefab.GetComponent<PrefabGetComponentTest1>();
        print(test.gameEffectSO.name);
    }

    // Update is called once per frame
    void Update() { }
}
