using UnityEngine;
using UnityEngine.UI;

public class HealthAndManaIndicator : MonoBehaviour
{
    public Image healthImage,
        manaImage;

    [ContextMenu("Test")]
    public void Test()
    {
        SetHealth(Random.Range(0, 1f));
        SetMana(Random.Range(0, 1f));
    }

    public void SetHealth(float p_progress)
    {
        healthImage.color = Color.Lerp(Color.red, Color.green, p_progress);
        healthImage.fillAmount = p_progress;
    }

    public static Color manaMax = new(0, 1, 1, 1),
        manaMin = new(0, 0, 1, 1);

    public void SetMana(float p_progress)
    {
        manaImage.color = Color.Lerp(manaMin, manaMax, p_progress);
        manaImage.fillAmount = p_progress;
    }
}
