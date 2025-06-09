using UnityEngine;

public class ClassTest1 : MonoBehaviour
{
    public PropTest1 p1 = new PropTest1();

    private void OnCollisionEnter2D(Collision2D other)
    {
        print(p1.f1);
    }
}
