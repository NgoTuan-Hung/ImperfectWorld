using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoObjectPoolPerformance : MonoBehaviour
{
    public GameObject prefab;
    public bool spawn = false;
    List<GameObject> prefabs = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            spawn = false;
            for (int i = 0; i < 100; i++)
            {
                GameObject go = Instantiate(prefab);
                go.transform.position = new Vector3(
                    Random.Range(-10, 10),
                    Random.Range(-10, 10),
                    0
                );
                prefabs.Add(go);
            }

            StartCoroutine(DestroyIE());
        }
    }

    IEnumerator DestroyIE()
    {
        yield return new WaitForSeconds(1f);

        prefabs.ForEach(gO => Destroy(gO));
    }
}
