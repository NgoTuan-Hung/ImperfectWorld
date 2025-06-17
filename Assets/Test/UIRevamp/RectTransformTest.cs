using UnityEngine;

public class RectTransformTest : MonoBehaviour
{
    public RectTransform panel,
        coners,
        coner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print(
            "Panel: "
                + panel.rect.size
                + " Coners: "
                + coners.rect.size
                + " Coner: "
                + coner.rect.size
        );
    }

    // Update is called once per frame
    void Update() { }
}
