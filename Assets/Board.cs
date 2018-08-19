using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {
    public Transform piecePrefab;
    public Transform borderPrefab;
    public Transform placePrefab;
    public GameObject gameOverOverlay;
    public InGamePanel ingamePanel;
    BoardModel board = new BoardModel();
    
    Piece current;
    List<Transform> shadow = new List<Transform>();
    List<Vector2> validPositions = new List<Vector2>();
    Grid<Transform> transforms = new Grid<Transform>();
    Grid<PlaceView> places = new Grid<PlaceView>();
    bool gameOver = false;
    float pause = 0f;
    bool playing = false;
    
    void CreateBlocks(List<BlockModel> blocks, bool init = false){
        foreach(BlockModel block in blocks)
            transforms.Add(block.x, block.z, CreateBlock(block));
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
        /*Transform instance = Instantiate(borderPrefab);
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        instance.localPosition = new Vector3(2f*ri*(((float)x)+(z%2==0?0.0f:0.5f)),0f,1.5f*ro*(float)z);
        return instance;*/
        return null;
    }
    
    Transform CreatePlace(int x, int z){
        Transform instance = Instantiate(placePrefab);
        float ro = HexMetrics.outerRadius;
        float ri = HexMetrics.innerRadius;
        Vector2 pos = new Vector2(2f*ri*(((float)x)+(z%2==0?0.0f:0.5f)),1.5f*ro*(float)z);
        instance.localPosition = new Vector3(pos.x,0f,pos.y);
        validPositions.Add(pos);
        places.Add(x, z, instance.Find("Mesh").gameObject.GetComponent<PlaceView>());
        return instance;
    }
    
    public void StartGame(){
        playing = true;
    }
    
    private void Start () {
        int boardHeight = GameApplication.GetOptions().BoardHeight;
        int boardWidth = GameApplication.GetOptions().BoardWidth;
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
        /*List<BlockModel> blocks = new List<BlockModel>();
        blocks.Add(new BlockModel(4,5,HexDirection.SW,HexDirection.E));
        blocks.Add(new BlockModel(5,5,HexDirection.W,HexDirection.SE));
        blocks.Add(new BlockModel(4,4,HexDirection.SE,HexDirection.NE));
        blocks.Add(new BlockModel(6,4,HexDirection.SW,HexDirection.NW));
        blocks.Add(new BlockModel(4,3,HexDirection.NW,HexDirection.E));
        //blocks.Add(new BlockModel(5,3,HexDirection.NE,HexDirection.W));
        CreateBlocks(blocks, true);
        transforms.Get(4,5).GetComponent<BlockView>().Highlight(true);*/
        
        board.removeBlock += (o, ev) => {
            if(ev.Blocks.Count == 0)
                return;
            foreach(BlockModel block in ev.Blocks){
                BlockView view = transforms.Remove(block.x,block.z).gameObject.GetComponent<BlockView>();
                view.Explode();
                pause = Mathf.Max(view.ExplosionDuration, pause);
            }
            ingamePanel.SetTimer(board.Time);
        };
        
                
        board.updateScore += (o, ev) => { ingamePanel.SetScore(ev.Score); };
        board.updateLife  += (o, ev) => { 
            ingamePanel.SetLife(ev.Life);
            pause = Mathf.Max(ingamePanel.BlinkSpeed, pause);
        };
        board.gameOver += (o, ev) => {
            gameOver = true;
            playing = false;
            gameOverOverlay.SetActive(true); 
            Cursor.visible = true;
        };
        
        current = transform.Find("Current").gameObject.GetComponent<Piece>();
        if(current == null)
            throw new Exception("Impossible to found the current peices object");
        board.newPiece += (o, ev) => { current.New(ev.Blocks); };
        
        Outside outside = transform.Find("Outside").gameObject.GetComponent<Outside>();
        outside.board = this;
        outside.Triangulate(-5,boardWidth+5,-5,boardHeight+5);
        
        ingamePanel.SetScore(board.Score, true);
        ingamePanel.SetLife(board.Life, true); 
        board.Start();
        Cursor.visible = false;
    }
    
    public float distanceTo(float x, float z){
        float min = 9999f;
        Vector2 v = new Vector2(x,z);
        foreach(Vector2 pos in validPositions)
            min = Mathf.Min(min, Vector2.Distance(pos,v));
        min -= HexMetrics.outerRadius;
        min /= HexMetrics.outerRadius;
        return Mathf.Max(0f, min);
    }
    
    HashSet<BlockView> prevHighlight = new HashSet<BlockView>();
    HashSet<PlaceView> selectedPlaces = new HashSet<PlaceView>();
    
    void doHighlight(int x, int z){
        foreach(PlaceView placeToUnselect in selectedPlaces)
            placeToUnselect.Selected(false);
        selectedPlaces.Clear();
        Transform tranBlock = transforms.Get(x,z);
        BlockView block = null;
        if(tranBlock != null) 
            block = tranBlock.GetComponent<BlockView>();
        if(block != null && prevHighlight.Contains(block))
            return;
        foreach(BlockView prevBlock in prevHighlight)
            prevBlock.Highlight(false);
        prevHighlight.Clear();
        if(block == null)
            return;        
        foreach(BlockModel connBlock in board.GetConnected(x,z)){
            BlockView connBlockView = transforms.Get(connBlock.x,connBlock.z).GetComponent<BlockView>();
            connBlockView.Highlight(true);
            prevHighlight.Add(connBlockView);
        }
    }
    
    void doShadow(List<BlockModel> list){
        foreach(BlockView prevBlock in prevHighlight)
            prevBlock.Highlight(false);
        prevHighlight.Clear();
        PlaceView place;
        HashSet<PlaceView> newSelectedPlaces = new HashSet<PlaceView>();
        foreach(BlockModel block in list){
            place = places.Get(block.x, block.z);
            if(place != null)
                newSelectedPlaces.Add(place);
        }
        foreach(PlaceView placeToSelect in newSelectedPlaces.Except(selectedPlaces))
            placeToSelect.Selected(true);
        foreach(PlaceView placeToUnselect in selectedPlaces.Except(newSelectedPlaces))
            placeToUnselect.Selected(false);
        selectedPlaces = newSelectedPlaces;
    }
    
    void Awake () {
    }
    
    bool IsOkay(List<BlockModel> list){
        foreach(BlockModel block in list){
            if(!places.Exist(block.x, block.z))
                return false;
            if(transforms.Exist(block.x, block.z))
                return false;
        }
        return true;
    }
    
    void Update()
    {
        if(!playing)
            return;
        
        current.IncRotation(Input.GetAxis("Mouse ScrollWheel"));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){
            current.MoveTo(hit.point);
            foreach(Transform tr in shadow)
                Destroy(tr.gameObject);
            shadow.Clear();
            List<BlockModel> list = current.GetShadow();
            if(current.IsBomb)
                doHighlight(list[0].x,list[0].z);
            else
                doShadow(list);
            if(Input.GetButtonDown("Fire1") && playing && pause == 0f){
                if(current.IsBomb)
                    board.Bomb(list[0].x,list[0].z);
                else if(IsOkay(list))
                    CreateBlocks(list);
            }
        }
        
        if(pause > 0f){
            pause -= Time.deltaTime;
            return;
        }
        
        board.Update(Time.deltaTime);
        ingamePanel.SetTimer(board.Time);
    }
}
