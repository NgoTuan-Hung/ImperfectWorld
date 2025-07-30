using System.Collections;
using UnityEngine;

public class PoolRevampPoolObject : MonoSelfAware
{
    public override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        StartCoroutine(FlyIE());
    }

    public override void Start()
    {
        base.Start();
    }

    IEnumerator FlyIE()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        float currentTime = 0;
        while (currentTime < 5)
        {
            transform.position += 2 * Time.fixedDeltaTime * transform.right;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            currentTime += Time.fixedDeltaTime;
        }

        deactivate();
    }
}
