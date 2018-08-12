
public class BlockModel {
    public int x;
    public int z;
    public HexDirection dir1;
    public HexDirection dir2;
    
    public BlockModel(int _x, int _z, HexDirection _dir1, HexDirection _dir2){
        x = _x;
        z = _z;
        dir1 = _dir1;
        dir2 = _dir2;
    }
}
