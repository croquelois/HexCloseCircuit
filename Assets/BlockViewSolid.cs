using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockViewSolid : MonoBehaviour {
    public const float textureScale = 0.01f;
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
        Vector3 up = new Vector3(0.0f,HexMetrics.depth,0.0f);
        Vector3 nop = new Vector3(0.0f,0.0f,0.0f);
        Vector3 carve = new Vector3(0.0f,HexMetrics.carveDepth,0.0f);
        Vector3 cd = center;
        Vector3 cu = cd + up - carve;
        Vector3 tcu = new Vector2(cu.x,cu.z);
        for (int i = 0; i < 6; i++){
            Vector3 tmp1 = corners[i];
            Vector3 tmp3 = Vector3.Lerp(corners[i], corners[i+1], 0.5f);
            Vector3 tmp2 = Vector3.Lerp(corners[i], corners[i+1], 0.5f - HexMetrics.carveWidth);
            Vector3 tmp4 = Vector3.Lerp(corners[i], corners[i+1], 0.5f + HexMetrics.carveWidth);
            Vector3 tmp5 = corners[i+1];
            float r1 = HexMetrics.outerRadius;
            float r2 = tmp2.magnitude;
            float r3 = HexMetrics.innerRadius;
            float r4 = tmp4.magnitude;
            float r5 = HexMetrics.outerRadius;
            Vector3 tmpc = tmp3 * (1.0f - 2f*HexMetrics.carveWidth);
            
            Vector3 d1 = center + tmp1;
            Vector3 d2 = center + tmp2;
            Vector3 d3 = center + tmp3;
            Vector3 d4 = center + tmp4;
            Vector3 d5 = center + tmp5;
            
            Vector3 v1 = d1 + up;
            Vector3 v2 = d2 + up;
            Vector3 v3 = d3 + up - (i == (int)dir1 || i == (int)dir2 ? carve : nop);
            Vector3 v4 = d4 + up;
            Vector3 v5 = d5 + up;
            
            Vector3 c1 = v2 - tmpc;
            Vector3 c2 = v3 - tmpc;
            Vector3 c3 = v4 - tmpc;
            
            Vector3 ttd1 = (r1 + HexMetrics.depth) * tmp1 / r1;
            Vector3 ttd2 = (r2 + HexMetrics.depth) * tmp2 / r2;
            Vector3 ttd3 = (r3 + HexMetrics.depth) * tmp3 / r3;
            Vector3 ttd4 = (r4 + HexMetrics.depth) * tmp4 / r4;
            Vector3 ttd5 = (r5 + HexMetrics.depth) * tmp5 / r5;
            Vector3 td1 = new Vector2(ttd1.x, ttd1.z);
            Vector3 td2 = new Vector2(ttd2.x, ttd2.z);
            Vector3 td3 = new Vector2(ttd3.x, ttd3.z);
            Vector3 td4 = new Vector2(ttd4.x, ttd4.z);
            Vector3 td5 = new Vector2(ttd5.x, ttd5.z);
            Vector2 tv1 = new Vector2(v1.x,v1.z);
            Vector2 tv2 = new Vector2(v2.x,v2.z);
            Vector2 tv3 = new Vector2(v3.x,v3.z);
            Vector2 tv4 = new Vector2(v4.x,v4.z);
            Vector2 tv5 = new Vector2(v5.x,v5.z);
            Vector2 tc1 = new Vector2(c1.x,c1.z);
            Vector2 tc2 = new Vector2(c2.x,c2.z);
            Vector2 tc3 = new Vector2(c3.x,c3.z);            
            
            // Top            
            AddTriangle(v5, c3, v4, tv5, tc3, tv4);
            AddTriangle(v2, c1, v1, tv2, tc1, tv1);
            AddTriangle(v4, c3, c2, tv4, tc3, tc2);
            AddTriangle(v2, c2, c1, tv2, tc2, tc1);
            AddTriangle(v4, c2, v3, tv4, tc2, tv3);
            AddTriangle(v2, v3, c2, tv2, tv3, tc2);
            AddTriangle(c3, cu, c2, tc3, tcu, tc2);
            AddTriangle(c2, cu, c1, tc2, tcu, tc1);
            
           // Side
            AddTriangle(d5, v5, v4, td5, tv5, tv4);
            AddTriangle(d5, v4, d4, td5, tv4, td4);
            AddTriangle(v4, v3, d4, tv4, tv3, td4);
            AddTriangle(v3, d3, d4, tv3, td3, td4);
            
            AddTriangle(d1, v2, v1, td1, tv2, tv1);
            AddTriangle(d1, d2, v2, td1, td2, tv2);
            AddTriangle(v2, d2, v3, tv2, td2, tv3);
            AddTriangle(v3, d2, d3, tv3, td2, td3);
            
            
            // Bottom
            AddTriangle(cd, d5, d1);
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
