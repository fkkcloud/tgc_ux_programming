using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Spiral : GameBehaviour 
{
    public float Length = 20f;
    public Vector2 LengthRange;
    public Vector2 RadiusMapX;
    public Vector2 RadiusMapY;
    public float ScaleZ = 0.3f;
    public bool NoiseOn = false;
    public AnimationCurve RadiusRemapCurve;
    public Image GameScreen;

    [Range(0.05f, 1f)]
    public float SegmentLength = 0.5f;
    public List<Vector3> Positions { get; private set; }

    private float _lengthGrowSpeed = 0f;
    private float _targetLength;

    private float _actualLength;

    void Awake()
    {
        Positions = new List<Vector3>();
    }

    public float GetCurrentRadius(float t, ref Vector2 radiusMap)
    {
        float normalizedLength = Mathf.InverseLerp(0f, Length, t);
        float valueCurveMapped = RadiusRemapCurve.Evaluate(normalizedLength);
        return SpiralMath.Remap(valueCurveMapped, 1f, 0f, radiusMap.x, radiusMap.y);
    }

    void CreateSpiral()
    {
        Positions.Clear();
        float accumulatedLength = 0f;
        while (accumulatedLength < Length)
        {
            Vector3 pos = new Vector3();

            float radiusX = GetCurrentRadius(accumulatedLength, ref RadiusMapX);
            float radiusY = GetCurrentRadius(accumulatedLength, ref RadiusMapY);

            SpiralMath.GetPositionAt(accumulatedLength, ref pos, radiusX, radiusY, ScaleZ, NoiseOn);

            Positions.Add(pos + gameObject.transform.position);

            accumulatedLength += SegmentLength;
        }
    }

    public void OnFriendlistExit()
    {
        if (_actualLength > LengthRange.y)
        {
            _actualLength = LengthRange.y;
            Length = LengthRange.y;
        }
    }

	// Update is called once per frame
	void Update () 
    {
        if (Application.isPlaying) // in editor debug purposes
        {
            _actualLength += GlobalGestureCircle.DragMotion * 1.6f;

            _actualLength = Mathf.Max(0f, _actualLength);

            _targetLength = Mathf.Clamp(_actualLength, LengthRange.x, LengthRange.y);

            float clampedLength = Mathf.Clamp(_actualLength, LengthRange.x, LengthRange.y);

            Length = Mathf.SmoothDamp(clampedLength, _targetLength, ref _lengthGrowSpeed, 0.1f);
        }
        Length = Mathf.Clamp(Length, LengthRange.x, LengthRange.y);

        GameScreen.color = Color.white * SpiralMath.Remap(Length, 2f, 0f, 0.5f, 1f); 

        CreateSpiral();
	}

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Positions.Count; i++)
        {
            if (i >= Positions.Count - 1)
                return;
            Gizmos.DrawLine(Positions[i], Positions[i+1]);
        }
    }

    public float GetActuralLength()
    {
        return _actualLength;
    }
}
