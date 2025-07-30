using UnityEngine;

public class SizeOf : MonoBehaviour
{
    private void Start()
    {
        print(sizeof(decimal));
        print("Epsilon: " + double.Epsilon);
        const int col1 = 37,
            col2 = 10;
        print(new string('-', col1 + col2 + 10) + "\n" + $"{"Vip", col1} | {"Name", col2}");
    }
}
