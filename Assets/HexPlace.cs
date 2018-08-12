using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPlace : MonoBehaviour {
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
        Vector3[] corners = HexMetrics.corners;
        Vector3 c = transform.localPosition;
        for (int i = 0; i < 6; i++){            
            Vector3 v1 = c + corners[i];
            Vector3 v2 = c + corners[i+1];
            
            Vector3 c1 = c + 0.9f*corners[i];
            Vector3 c2 = c + 0.9f*corners[i+1];          
            
            // Top            
            AddTriangle(c, c1, c2);
            AddTriangle(c1, v1, v2);
            AddTriangle(c1, v2, c2);
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
