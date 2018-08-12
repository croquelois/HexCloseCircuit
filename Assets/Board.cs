using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {
    public int boardHeight = 15;
    public int boardWidth = 20;
    public Transform piecePrefab;
    public Transform borderPrefab;
    public Text score;
    public Slider timer;
    float time = 0.0f;
    
    Piece current;
    List<Transform> shadow = new List<Transform>();
    List<Vector2> validPositions = new List<Vector2>();
            
    Transform CreateBlock(BlockModel model){
        Transform instance = Instantiate(piecePrefab);
        instance.gameObject.GetComponent<BlockView>().dir1 = model.dir1;
        instance.gameObject.GetComponent<BlockView>().dir2 = model.dir2;
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        instance.localPosition = new Vector3(2f*ri*(((float)model.x)+(model.z%2==0?0.0f:0.5f)),0f,1.5f*ro*(float)model.z);
        return instance;
    }
    
    Transform CreateBorder(int x, int z){
        Transform instance = Instantiate(borderPrefab);
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        instance.localPosition = new Vector3(2f*ri*(((float)x)+(z%2==0?0.0f:0.5f)),0f,1.5f*ro*(float)z);
        return instance;
    }
    
    Transform CreatePlace(int x, int z){
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        Vector2 pos = new Vector2(2f*ri*(((float)x)+(z%2==0?0.0f:0.5f)),1.5f*ro*(float)z);
        validPositions.Add(pos);
        return null;
    }
    
    private void Start () {
        for(int z=-1;z<boardHeight+1;z++)
            CreateBorder(-1,z);
        for(int x=0;x<boardWidth;x++){
            CreateBorder(x,-1);
            for(int z=0;z<boardHeight;z++){
                CreatePlace(x,z);
            }
            CreateBorder(x,boardHeight);
        }
        for(int z=0;z<boardHeight;z++)
            CreateBorder(boardWidth,z);
        CreateBlock(new BlockModel(3,3,HexDirection.NE,HexDirection.E));
        CreateBlock(new BlockModel(5,4,HexDirection.W,HexDirection.E));
        CreateBlock(new BlockModel(4,4,HexDirection.SW,HexDirection.E));
        
        current = transform.Find("Current").gameObject.GetComponent<Piece>();
        newPiece();
        Outside outside = transform.Find("Outside").gameObject.GetComponent<Outside>();
        outside.board = this;
        outside.Triangulate(-5,boardWidth+5,-5,boardHeight+5);
    }
    
    public float distanceTo(float x, float z){
        float min = 9999f;
        Vector2 v = new Vector2(x,z);
        foreach(Vector2 pos in validPositions)
            min = Mathf.Min(min, Vector2.Distance(pos,v));
        min -= HexMetrics.outerRadius;
        min /= HexMetrics.outerRadius;
        return Mathf.Max(0f, min);
        /*float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        int zi = Mathf.RoundToInt(z/(1.5f*ro));
        int xi = Mathf.RoundToInt(x/(2f*ri) - (zi%2==0?0.0f:0.5f));
        if(xi >= 0 && xi < boardWidth && zi >= 0 && zi < boardHeight)
            return 0f;
        
        float dx = 0f;
        float dz = 0f;
        if(xi < 0)
            dx += -(float)xi;
        if(xi >= boardWidth)
            dx += (float)(xi-boardWidth);
        if(zi < 0)
            dz += -(float)zi;
        if(zi >= boardHeight)
            dz += (float)(zi-boardHeight);
        return Mathf.Sqrt(dx*dx+dz*dz);*/
    }
    
    void newPiece(){
        current.clear();
        current.add(new BlockModel(0,0,HexDirection.SW,HexDirection.E));
        current.add(new BlockModel(1,0,HexDirection.SW,HexDirection.E));
        current.add(new BlockModel(0,1,HexDirection.SW,HexDirection.E));
        current.moveToCenter();
    }
    
    void Awake () {
    }
    
    void Update()
    {
        current.incRotation(Input.GetAxis("Mouse ScrollWheel"));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            current.moveTo(hit.point);
            foreach(Transform tr in shadow)
                Destroy(tr.gameObject);
            shadow.Clear();
            List<BlockModel> list = current.getShadow();
            /*foreach(BlockModel blockModel in list)
                shadow.Add(CreateBlock(blockModel));*/
            if(Input.GetButtonDown("Fire1")){
                foreach(BlockModel blockModel in list)
                    CreateBlock(blockModel);
            }
        }
        
        time += Time.deltaTime;
        if(time > 1.0f){
            newPiece();
            time = 0.0f;
        }
        timer.value = time;
    }
}
