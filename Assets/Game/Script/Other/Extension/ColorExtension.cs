using UnityEngine;

public static class ColorExtension
{
    public static Color RandomColor() =>
        new(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

    public static Color RandomBright() => Color.HSVToRGB(Random.value, 1f, 1f);
}
