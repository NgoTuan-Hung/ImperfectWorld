using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RectTransformSizePrinter : MonoBehaviour
{
    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(PrintWidthIE());
    }

    IEnumerator PrintWidthIE()
    {
        yield return null;
        var r = RectTransformUtility.PixelAdjustRect(rectTransform, UIRManager.canvas);
        print(r.size);
    }
}
