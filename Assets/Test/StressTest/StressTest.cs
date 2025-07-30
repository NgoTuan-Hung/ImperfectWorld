using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressTest : MonoBehaviour
{
    public GameObject veryComplicatedObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StressIE());
    }

    public int spawn_number = 100;
    public float interval = 100;

    IEnumerator StressIE()
    {
        while (true)
        {
            for (int i = 0; i < spawn_number; i++)
            {
                GameObject t_gO = Instantiate(
                    veryComplicatedObject,
                    new Vector3(Random.Range(-999, 999), Random.Range(-999, 999), 0),
                    Quaternion.identity
                );
                // Destroy(t_gO, Random.Range(0, 5f));
            }

            yield return new WaitForSeconds(interval);
        }
    }

    // Update is called once per frame
    void Update() { }
}
