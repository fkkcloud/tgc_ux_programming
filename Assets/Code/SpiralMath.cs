using UnityEngine;

public static class SpiralMath 
{
    public static void GetPositionAt(float t, ref Vector3 position, float radiusX = 1.3f, float radiusY = 1.3f, float zScale = 0.3f, bool enableNoise = false)
    {
        float offsetX = 0f;
        float offsetY = 0f;
        if (enableNoise)
        {
            offsetX = Mathf.PerlinNoise(position.x + Time.time * 0.35f + t, 0f) * 0.15f * radiusX;
            offsetY = Mathf.PerlinNoise(0f, position.y + Time.time * 0.35f + t) * 0.15f * radiusY;
        }

        position.x = Mathf.Cos(t) * radiusX + offsetX;
        position.y = Mathf.Sin(t) * radiusY + offsetY;
        position.z = t * zScale;
    }

    public static float Remap(float value, float low1, float high1, float low2, float high2)
    {
        return Mathf.Clamp(low2 + (value - low1) * (high2 - low2) / (high1 - low1), low2, high2);
    }

    public static bool Vector2AboutEqual(ref Vector2 a, ref Vector2 b)
    {
        return (Mathf.Approximately(a.x, b.x) && Mathf.Approximately(a.y, b.y));
    }

    public static float GetAngleAtan2(Vector2 A, Vector2 B)
    {
        // |A·B| = |A| |B| COS(θ)
        // |A×B| = |A| |B| SIN(θ)
        return Mathf.Atan2(Cross(A, B), Dot(A, B));
    }

    public static float Dot(Vector2 A, Vector2 B)
    {
        return A.x * B.x + A.y * B.y;
    }

    public static float Cross(Vector2 A, Vector2 B)
    {
        return A.x * B.y - A.y * B.x;
    }
}
