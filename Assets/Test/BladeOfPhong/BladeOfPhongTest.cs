using System.Collections;
using UnityEngine;

public class BladeOfPhongTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public Vector3 direction;
    public GameObject tornado;

    public void SpawnTornado()
    {
        GameObject t_tornado = Instantiate(tornado, transform.position, Quaternion.identity);
        t_tornado.GetComponent<EffectTest>().Travel(direction);
    }
}
