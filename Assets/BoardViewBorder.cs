using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardViewBorder : MonoBehaviour {
    public const float textureScale = 0.1f;
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;
    
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        Vector2 c1 = new Vector2(v1.x,v1.z);
        Vector2 c2 = new Vector2(v2.x,v2.z);
        Vector2 c3 = new Vector2(v3.x,v3.z);
        AddTriangle(v1,v2,v3,c1,c2,c3);
    }
    
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 c1, Vector2 c2, Vector2 c3) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        uvs.Add(textureScale * c1);
        uvs.Add(textureScale * c2);
        uvs.Add(textureScale * c3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    
    private void Start () {
        Triangulate();
    }
    
    public void Triangulate() {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        Vector3 up = new Vector3(0.0f,HexMetrics.depth,0.0f);
        Vector3[] corners = HexMetrics.corners;
        Vector3 cd = transform.localPosition;
        Vector3 cu = cd + up;
        for (int i = 0; i < 6; i++){
            Vector3 d1 = cd + corners[i];
            Vector3 d2 = cd + corners[i+1];
            Vector3 u1 = d1 + up;
            Vector3 u2 = d2 + up;
            AddTriangle(cu, u1, u2);
            AddTriangle(cu, u1, u2);
            AddTriangle(d2, u2, u1);
            AddTriangle(d2, u1, d1);
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }
    
    void Awake () {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
    }
}
