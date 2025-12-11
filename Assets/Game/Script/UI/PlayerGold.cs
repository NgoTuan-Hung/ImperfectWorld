using TMPEffects.Components;
using TMPro;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public TMPAnimator tMPAnimator;
    public TMPWriter tMPWriter;

    private void Awake()
    {
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        tMPAnimator = GetComponentInChildren<TMPAnimator>();
        tMPWriter = GetComponentInChildren<TMPWriter>();
    }

    public void SetGold(int gold)
    {
        textMeshProUGUI.text = "<+char dur=0.5>" + gold.ToString();
        tMPWriter.RestartWriter();
    }
}
