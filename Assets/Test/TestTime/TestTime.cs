using System.Collections;
using UnityEngine;

public class TestTime : MonoBehaviour
{
    public float timeScale = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // StartCoroutine(PrintFixedDeltaTime());
    }

    IEnumerator PrintFixedDeltaTime()
    {
        while (true)
        {
#if false
            speed = 1;
            realSpeed = 
#endif
            yield return new WaitForSeconds(1);
            Debug.Log(Time.fixedDeltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
    }

    void FixedUpdate()
    {
        print("Fixed Update");
    }
}
