using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockViewLiquid : MonoBehaviour {
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
    
    public void Triangulate(HexDirection dir1, HexDirection dir2) {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        Vector3[] corners = HexMetrics.corners;
        Vector3 center = transform.localPosition;
        Vector3 up = new Vector3(0.0f,HexMetrics.depth - HexMetrics.carveDepth / 4f,0.0f);
        Vector3 cu = center + up;
        Vector3 tcu = new Vector2(cu.x,cu.z);
        for (int i = 0; i < 6; i++){
            Vector3 tmp3 = Vector3.Lerp(corners[i], corners[i+1], 0.5f);
            Vector3 tmp2 = Vector3.Lerp(corners[i], corners[i+1], 0.5f - HexMetrics.carveWidth);
            Vector3 tmp4 = Vector3.Lerp(corners[i], corners[i+1], 0.5f + HexMetrics.carveWidth);
            Vector3 tmpc = tmp3 * (1.0f - 2f*HexMetrics.carveWidth);
            
            Vector3 v2 = cu + tmp2;
            Vector3 v4 = cu + tmp4;
            
            Vector3 c1 = v2 - tmpc;
            Vector3 c3 = v4 - tmpc;
            
            Vector2 tv2 = new Vector2(v2.x,v2.z);
            Vector2 tv4 = new Vector2(v4.x,v4.z);
            Vector2 tc1 = new Vector2(c1.x,c1.z);
            Vector2 tc3 = new Vector2(c3.x,c3.z);
            
            AddTriangle(v4, c3, c1, tv4, tc3, tc1);
            AddTriangle(v4, c1, v2, tv4, tc1, tv2);
            AddTriangle(c3, cu, c1, tc3, tcu, tc1);
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
