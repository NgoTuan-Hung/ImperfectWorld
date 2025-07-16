using UnityEngine;
using UnityEngine.UI;

public class HealthUIRevamp : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    [ContextMenu("Test")]
    public void Test()
    {
        image = GetComponent<Image>();
        SetHealth(Random.Range(0, 1f));
    }

    public void SetHealth(float p_progress)
    {
        image.color = Color.Lerp(Color.red, Color.green, p_progress);
        image.fillAmount = p_progress;
    }
}
