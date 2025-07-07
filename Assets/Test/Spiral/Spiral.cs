using System;
using UnityEngine;

public class Spiral : MonoBehaviour
{
    public float startingRadius = 5,
        currentRadius;
    public float angle = 2 * Mathf.PI,
        doublePi = 2 * Mathf.PI;
    public float someValue = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        angle -= Mathf.PI * Time.deltaTime;
        someValue += 1f / 6 * Time.deltaTime;

        if (angle < 0)
            angle = doublePi;

        currentRadius = startingRadius - someValue * startingRadius;

        transform.position = new Vector3(
            currentRadius * (float)Math.Cos(angle),
            currentRadius * (float)Math.Sin(angle),
            0
        );
    }
}
