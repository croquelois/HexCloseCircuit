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
    BoardModel board = new BoardModel();
    
    Piece current;
    List<Transform> shadow = new List<Transform>();
    List<Vector2> validPositions = new List<Vector2>();
    Dictionary<Pos,Transform> transforms = new Dictionary<Pos,Transform>();
    
    void CreateBlocks(List<BlockModel> blocks, bool init = false){
        foreach(BlockModel block in blocks)
            transforms.Add(new Pos(block.x,block.z), CreateBlock(block));
        if(init)
            board.Add(blocks);
        else
            board.Push(blocks);
    }
    
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
        /*
        BoardModelTest test = new BoardModelTest();
        test.Main();
        return;
        */
        
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
        List<BlockModel> blocks = new List<BlockModel>();
        blocks.Add(new BlockModel(4,5,HexDirection.SW,HexDirection.E));
        blocks.Add(new BlockModel(5,5,HexDirection.W,HexDirection.SE));
        blocks.Add(new BlockModel(4,4,HexDirection.SE,HexDirection.NE));
        blocks.Add(new BlockModel(6,4,HexDirection.SW,HexDirection.NW));
        blocks.Add(new BlockModel(4,3,HexDirection.NW,HexDirection.E));
        //blocks.Add(new BlockModel(5,3,HexDirection.NE,HexDirection.W));
        CreateBlocks(blocks, true);
        
        board.removeBlock += (o, ev) => {
            foreach(BlockModel block in ev.Blocks){
                Pos pos = new Pos(block.x,block.z);
                Destroy(transforms[pos].gameObject);
                transforms.Remove(pos);
            }
        };
        
        current = transform.Find("Current").gameObject.GetComponent<Piece>();
        board.newPiece += (o, ev) => {
            Debug.Log("new piece called");
            current.clear();
            foreach(BlockModel block in ev.Blocks)
                current.add(block);
            current.moveToCenter();
        };
        
        Outside outside = transform.Find("Outside").gameObject.GetComponent<Outside>();
        outside.board = this;
        outside.Triangulate(-5,boardWidth+5,-5,boardHeight+5);
        
        board.Start();
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
    }
    
    void Awake () {
    }
    
    void Update()
    {
        if(current == null)
            return;
        
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
                CreateBlocks(list);
            }
        }
        board.Update(Time.deltaTime);
        timer.value = board.Time;
    }
}
