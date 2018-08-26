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
    
    public AudioSource musicInGame;
    
    public event EventHandler<EventArgs> actionRejected = delegate {};
    
    Piece current;
    List<Transform> shadow = new List<Transform>();
    List<Vector2> validPositions = new List<Vector2>();
    Grid<Transform> transforms = new Grid<Transform>();
    Grid<PlaceView> places = new Grid<PlaceView>();
    bool gameOver = false;
    bool playing = false;
    float pause = 0f;
    
    BoardModel board;
    
    public bool Playing { 
        get {
            return playing;
        }
        set {
            playing = value;
            Cursor.visible = !value;
            if(value)
                musicInGame.Play();
            else
                musicInGame.Pause();
        }
    }
    
    public float Pause {
        get {
            return pause;
        }
        set {
            pause = Mathf.Max(pause, value);
        }
    }
    
    public void SetBoardModel(BoardModel v){
        if(board != null)
            throw new Exception("board model has already been set");
        if(current == null){
            current = transform.Find("Current").gameObject.GetComponent<Piece>();
            if(current == null)
                throw new Exception("Impossible to found the current pieces object");
        }
        board = v;
        
        board.removeBlock += (o, ev) => {
            if(ev.Blocks.Count == 0)
                return;
            foreach(BlockModel block in ev.Blocks){
                BlockView view = transforms.Remove(block.x,block.z).gameObject.GetComponent<BlockView>();
                view.Explode();
                Pause = view.ExplosionDuration;
            }
        };
        
        board.gameOver += (o, ev) => {
            gameOver = true;
            Playing = false;
        };
        
        board.newPiece += (o, ev) => { current.New(ev.Blocks); };        
    }
    
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
        if(!Playing)
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
            if(Input.GetButtonDown("Fire1") && pause <= 0f){
                if(current.IsBomb)
                    board.Bomb(list[0].x,list[0].z);
                else if(IsOkay(list))
                    CreateBlocks(list);
                else
                    actionRejected(this, new EventArgs());
            }
        }
        
        if(pause > 0f){
            pause -= Time.deltaTime;
            return;
        }
        
        board.Update(Time.deltaTime);
    }
}
