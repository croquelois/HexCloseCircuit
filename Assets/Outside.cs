using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outside : MonoBehaviour {
    public const float textureScale = 0.0025f;
    public Texture2D noiseSource;
    public Board board;
    public float noiseXscale = 30f;
    public float noiseZscale = 30f;
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
    float noise(float x, float z){
        x /= noiseXscale*HexMetrics.outerRadius*5f;
        z /= noiseZscale*HexMetrics.outerRadius*5f;
        return noiseSource.GetPixelBilinear(x, z).r;
    }
    void fillYCoord(ref Vector3 v){
        float d = 10f*board.distanceTo(v.x,v.z);
        if(d == 0f)
            v.y = -0.1f;
        else
            v.y = d * noise(v.x,v.z); //(d > 10f ? 10f : d)
    }
    public void Triangulate(int xi0,int xi1,int zi0, int zi1) {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        Vector3[] corners = HexMetrics.corners;
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        for (int xi=xi0;xi<=xi1;xi++){
            for (int zi=zi0;zi<=zi1;zi++){
                Vector3 c = new Vector3(2f*ri*(((float)xi)+(zi%2==0?0.0f:0.5f)),0f,1.5f*ro*(float)zi);
                for (int i = 0; i < 6; i++){
                    Vector3 v1 = c + corners[i];
                    Vector3 v2 = c + corners[i+1];                    
                    fillYCoord(ref v1);
                    fillYCoord(ref v2);
                    fillYCoord(ref c);
                    AddTriangle(c, v1, v2);
                }
            }
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
