using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProceduralMeshGenerator : GameBehaviour
{
    [Header("Create Procedural Mesh")]
    public Spiral SpiralReference;
    public GameObject MeshObj;

    public bool CreateMesh = false;
    public bool FlipUV = false;
    public float CustomCurveMultL = 1.0f;
    public float CustomCurveMultR = 1.0f;

    public AnimationCurve CurveL = AnimationCurve.Linear(0f, 1f, 1f, 1f); //AnimationCurve.EaseInOut(0f, 0.1f, 1f, 0.9f);
    public AnimationCurve CurveR = AnimationCurve.Linear(0f, 1f, 1f, 1f); // AnimationCurve.EaseInOut(0f, 0.1f, 1f, 0.9f);
    public Material MeshMat;

    private MeshFilter _filter;
    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector2> _uv = new List<Vector2>();
    private List<int> _triangles = new List<int>();
    private Vector3[] _crossVectors = new Vector3[2];
    private Mesh _proceduralMesh;

    void Update()
    {
        if (CreateMesh)
        {
            if (!MeshObj)
            {
                MeshObj = new GameObject();
                MeshObj.AddComponent<MeshFilter>();
                MeshObj.AddComponent<MeshRenderer>();
                MeshObj.transform.parent = transform;
                MeshObj.transform.position = new Vector3(0f, 0f, 0f);
                MeshObj.transform.localScale = new Vector3(1f, 1f, 1f);
                MeshObj.name = "ProceduralMesh";
                _filter = MeshObj.GetComponent<MeshFilter>();
                _filter.sharedMesh = new Mesh();
            }
            else if (MeshObj && !_filter)
            {
                _filter = MeshObj.GetComponent<MeshFilter>();
                _filter.sharedMesh = new Mesh();
            }

            if (SpiralReference.Positions.Count > 1)
            {
                CreateProceduralMesh();
                _filter.gameObject.GetComponent<Renderer>().material = MeshMat;
            }
            else
            {
                RemoveProceduralMesh();
            }
            
        }
        else if (MeshObj && _filter)
        {
            RemoveProceduralMesh();
        }
    }

    private void CreateProceduralMesh()
    {
        _vertices.Clear();
        _triangles.Clear();
        _uv.Clear();
        
        Vector3[] pos = SpiralReference.Positions.ToArray();

        float t;

        float lastId = (float)(pos.Length - 1);
        for (int i = 0; i < pos.Length; i++)
        {
            t = i / (lastId); //  0.0 ~ 1.0
            CalculateSideVectors(ref pos, i, CustomCurveMultR * CurveR.Evaluate(t), CustomCurveMultL * CurveL.Evaluate(t));
            AddCurvePoint(_crossVectors[1], _crossVectors[0], i, pos.Length - 1);
        }

        Mesh mesh = _filter.sharedMesh;
        mesh.Clear();

        /*
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < normales.Length; n++)
            normales[n] = Vector3.up;
        */

        mesh.vertices = _vertices.ToArray();
        //mesh.normals = normales;
        mesh.uv = _uv.ToArray();
        mesh.triangles = _triangles.ToArray();

        mesh.RecalculateBounds();
        //mesh.RecalculateNormals();

        if (MeshObj != null)
        {
            MeshObj.transform.position = Vector3.zero;
            MeshObj.transform.localScale = Vector3.one;
            MeshObj.transform.rotation = Quaternion.identity;
        }
    }

    private void RemoveProceduralMesh()
    {
        _vertices.Clear();
        _triangles.Clear();
        _uv.Clear();
        Mesh mesh = _filter.sharedMesh;
        mesh.Clear();
        DestroyImmediate(MeshObj);
    }

    private void CalculateSideVectors(ref Vector3[] pos, int i, float WidthR, float WidthL)
    {
        Vector3 tangent;
        if (i + 1 >= pos.Length)
        {
            tangent = (pos[i] - pos[i - 1]).normalized;
        }
        else
            tangent = (pos[i + 1] - pos[i]).normalized;

        Vector3 upvector = pos[i] - Vector3.forward;

        Vector3 toUpvector = (upvector - pos[i]);

        Vector3 CrossL = Vector3.Cross(tangent, toUpvector).normalized;
        Vector3 CrossR = CrossL * -1f;

        Vector3 r = pos[i] + CrossR * WidthR;
        Vector3 l = pos[i] + CrossL * WidthL;

        _crossVectors[0] = r;
        _crossVectors[1] = l;
    }

    private void AddCurvePoint(Vector3 R, Vector3 L, int id, int count)
    {
        int start;

        _vertices.Add(R);
        _vertices.Add(L);

        if (FlipUV)
        {
            _uv.Add(new Vector2(0f, (float)id / count));
            _uv.Add(new Vector2(1f, (float)id / count));
        }
        else
        {
            _uv.Add(new Vector2((float)id / count, 0f));
            _uv.Add(new Vector2((float)id / count, 1f));
        }

        if (_vertices.Count >= 4)
        {
            start = _vertices.Count - 4;
            _triangles.Add(start + 0);
            _triangles.Add(start + 2);
            _triangles.Add(start + 1);
            _triangles.Add(start + 1);
            _triangles.Add(start + 2);
            _triangles.Add(start + 3);
        }
    }

    void OnDestroy()
    {
        DestroyImmediate(MeshObj);
    }
}