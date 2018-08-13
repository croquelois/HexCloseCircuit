using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceView : MonoBehaviour {
    public const float textureScale = 0.1f;
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;
    bool selected = false;
    
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, bool border) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        float col = border ? 0.9f : 0.95f;
        if(selected)
            col = 1f - col;
        uvs.Add(new Vector2(col,col));
        uvs.Add(new Vector2(col,col));
        uvs.Add(new Vector2(col,col));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    
    private void Start () {
        Triangulate();
    }
    
    public void Selected(bool val){
        if(val != selected){
            selected = val;
            Triangulate();
        }
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
            AddTriangle(c, c1, c2, false);
            AddTriangle(c1, v1, v2, true);
            AddTriangle(c1, v2, c2, true);
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
