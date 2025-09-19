using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopupUI : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    Vector2 initialVelocity;
    static readonly float duration = 1f;
    static readonly Vector2 acceleration = new(0, -9.81f);
    float defaultFontSize;

    private void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        defaultFontSize = textMeshProUGUI.fontSize;
    }

    public IEnumerator StartPopupIE(Vector3 p_initialPos, float p_damage)
    {
        textMeshProUGUI.text = p_damage.ToString();
        transform.position = p_initialPos;
        float t_currentTime = 0;
        initialVelocity =
            new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(0.5f, 1f)).normalized * 5f;
        float oneMinusProgress;
        while (t_currentTime < duration)
        {
            transform.position += (initialVelocity * Time.fixedDeltaTime).AsVector3();
            initialVelocity += acceleration * Time.fixedDeltaTime;
            oneMinusProgress = (1 - t_currentTime) / duration;
            textMeshProUGUI.color = ((Vector4)textMeshProUGUI.color).WithW(oneMinusProgress);
            textMeshProUGUI.fontSize = defaultFontSize * oneMinusProgress;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            t_currentTime += Time.fixedDeltaTime;
        }
    }
}
