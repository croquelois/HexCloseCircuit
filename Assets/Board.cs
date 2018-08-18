using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour {
    public Transform piecePrefab;
    public Transform borderPrefab;
    public Transform placePrefab;
    public GameObject gameOverOverlay;
    public Text score;
    public Text life;
    public Slider timer;
    BoardModel board = new BoardModel();
    
    Piece current;
    List<Transform> shadow = new List<Transform>();
    List<Vector2> validPositions = new List<Vector2>();
    HashSet<PlaceView> selectedPlaces = new HashSet<PlaceView>();
    Grid<Transform> transforms = new Grid<Transform>();
    Grid<PlaceView> places = new Grid<PlaceView>();
    bool gameOver = false;
    bool pause = false;
    
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
    
    void Unpause(){
        pause = false;
    }
    
    private void Start () {
        int boardHeight = Options.boardHeight;
        int boardWidth = Options.boardWidth;
        /*
        pause = true;
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
            if(ev.Blocks.Count == 0)
                return;
            pause = true;
            float callbackTime = 0f;
            foreach(BlockModel block in ev.Blocks){
                BlockView view = transforms.Remove(block.x,block.z).gameObject.GetComponent<BlockView>();
                view.Explode();
                callbackTime = view.ExplosionDuration;
            }
            Invoke("Unpause", callbackTime);
            timer.value = board.Time;
        };
        
                
        board.updateScore += (o, ev) => { score.text = "Score: " + ev.Score; };
        board.updateLife += (o, ev) => { life.text = "Life: " + ev.Life; };
        board.gameOver += (o, ev) => { 
            gameOver = true; 
            gameOverOverlay.SetActive(true); 
            Cursor.visible = true;
        };
        
        current = transform.Find("Current").gameObject.GetComponent<Piece>();
        board.newPiece += (o, ev) => { current.New(ev.Blocks); };
        
        Outside outside = transform.Find("Outside").gameObject.GetComponent<Outside>();
        outside.board = this;
        outside.Triangulate(-5,boardWidth+5,-5,boardHeight+5);
        
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
    
    void doShadow(List<BlockModel> list){
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
        if(pause)
            return false;
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
        if(gameOver)
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
            doShadow(list);
            if(Input.GetButtonDown("Fire1")){
                if(IsOkay(list))
                    CreateBlocks(list);
            }
        }
        
        if(pause)
            return;
        
        board.Update(Time.deltaTime);
        timer.value = board.Time;
    }
}
