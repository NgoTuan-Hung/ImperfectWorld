using UnityEngine;

public class FlatTopHexGridGizmoXY : MonoBehaviour
{
    public int radius = 5;

    [Tooltip("Flat-to-flat width of a hex")]
    public float hexWidth = 1f;

    float HexRadius => hexWidth * 0.5f;
    public bool drawGizmos = true;

    void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.cyan;

        for (int q = -radius; q <= radius; q++)
        {
            int rMin = Mathf.Max(-radius, -q - radius);
            int rMax = Mathf.Min(radius, -q + radius);

            for (int r = rMin; r <= rMax; r++)
            {
                DrawHex(HexToWorld(q, r), HexRadius);
            }
        }
    }

    /// Flat-top axial → XY plane
    Vector3 HexToWorld(int q, int r)
    {
        float R = HexRadius;

        float x = (3f / 2f) * R * q;
        float y = Mathf.Sqrt(3f) * R * (r + q * 0.5f);

        return new Vector3(x, y, 0f);
    }

    void DrawHex(Vector3 center, float R)
    {
        Vector3 first = Vector3.zero;
        Vector3 prev = Vector3.zero;

        for (int i = 0; i < 6; i++)
        {
            float angleRad = Mathf.Deg2Rad * (60f * i);

            Vector3 corner =
                center + new Vector3(Mathf.Cos(angleRad) * R, Mathf.Sin(angleRad) * R, 0f);

            if (i == 0)
                first = corner;
            else
                Gizmos.DrawLine(prev, corner);

            prev = corner;
        }

        Gizmos.DrawLine(prev, first);
    }
}
