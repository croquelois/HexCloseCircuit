using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideStep : MonoBehaviour {
    public const float textureScale = 0.0025f;
    public Texture2D noiseSource;
    public Board board;
    public float noiseXscale = 1f;
    public float noiseZscale = 1f;
    public float noiseMult = 50f;
    public float distMult = 0f;
    public float noiseMin = 115f/255f;
    public float noiseMax = 245f/255f;
    
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
    
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        Vector2 c1 = new Vector2(v1.x,v1.z);
        Vector2 c2 = new Vector2(v2.x,v2.z);
        Vector2 c3 = new Vector2(v3.x,v3.z);
        Vector2 c4 = new Vector2(v4.x,v4.z);
        AddQuad(v1,v2,v3,v4,c1,c2,c3,c4);
    }
    
    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector2 c1, Vector2 c2, Vector2 c3, Vector2 c4) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        uvs.Add(textureScale * c1);
        uvs.Add(textureScale * c2);
        uvs.Add(textureScale * c3);
        uvs.Add(textureScale * c4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
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
        x /= noiseXscale*HexMetrics.outerRadius;
        z /= noiseZscale*HexMetrics.outerRadius;
        float v = noiseSource.GetPixelBilinear(x, z).r;
        return Mathf.Clamp01((v-noiseMin)/(noiseMax-noiseMin));
    }
    float getNoise(Vector3 v){
        float dist = board.distanceTo(v.x,v.z);
        if(dist == 0f)
            return -0.1f;
        else
            return 0.2f + distMult*dist + noiseMult*noise(v.x,v.z);
    }
    
    float getNoise(Vector3 v, HexDirection dir){
        return getNoise(v + HexMetrics.GetNeighbor(dir));
    }

    
    public void Triangulate(int xi0,int xi1,int zi0, int zi1){
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        for (int xi=xi0;xi<=xi1;xi++)
            for (int zi=zi0;zi<=zi1;zi++)
                Triangulate(new Vector3(2f*ri*(((float)xi)+(zi%2==0?0.0f:0.5f)),0f,1.5f*ro*(float)zi));
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    void Triangulate(Vector3 c){
        c.y = getNoise(c);
        for(HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            Triangulate(d, c);
    }

    void Triangulate(HexDirection direction, Vector3 c){
        Vector3 v1 = c + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = c + HexMetrics.GetSecondSolidCorner(direction);

        AddTriangle(c, v1, v2);
        if(direction <= HexDirection.SE)
            TriangulateConnection(direction, c, v1, v2);
    }

    void TriangulateConnection(HexDirection direction, Vector3 c, Vector3 v1, Vector3 v2){
        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = getNoise(c, direction);

        AddQuad(v1, v2, v3, v4);
        if(direction <= HexDirection.E){
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = getNoise(c, direction.Next());
            AddTriangle(v2, v4, v5);
        }
    }
    
    void Awake () {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();
    }
}
