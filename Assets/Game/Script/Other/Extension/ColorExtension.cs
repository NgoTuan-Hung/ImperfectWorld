using UnityEngine;

public static class ColorExtension
{
    public static Color RandomColor() =>
        new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

    public static Color RandomBright() => Color.HSVToRGB(Random.value, 1f, 1f);

    public static Color WithAlpha(this Color color, float alpha) =>
        new(color.r, color.g, color.b, alpha);

    public static Color FromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;

        return Color.white; // fallback if hex is invalid
    }
}
