
public class Pos {
    int x;
    int z;
    
    public Pos(int x, int z){
        this.x = x;
        this.z = z;
    }
    
    public static bool operator ==(Pos lhs, Pos rhs){
        return (lhs.x == rhs.x) && (lhs.z == rhs.z);
    }

    public static bool operator !=(Pos lhs, Pos rhs){
        return (lhs.x != rhs.x) || (lhs.z != rhs.z);
    }

    public override bool Equals(object obj){
        if (obj == null || GetType() != obj.GetType())
            return false;

        Pos other = (Pos)obj;
        return (x == other.x) && (z == other.z);
    }
    
    public override int GetHashCode(){
        int hash = 17;
        hash = hash * 23 + x;
        hash = hash * 23 + z;
        return hash;
    }
}