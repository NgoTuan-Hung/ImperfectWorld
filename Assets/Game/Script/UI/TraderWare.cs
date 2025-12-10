using TMPro;
using UnityEngine;

public class TraderWare : MonoBehaviour
{
    TextMeshProUGUI priceTMP;

    private void Awake()
    {
        priceTMP = transform.Find("PriceTMP").GetComponent<TextMeshProUGUI>();
    }
}
