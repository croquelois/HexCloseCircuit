using System.Collections;
using System.Collections.Generic;

public class Grid<T> where T : class {
    Dictionary<int,Dictionary<int,T>> grid;
    
    public Grid(){
        grid = new Dictionary<int,Dictionary<int,T>>();
    }
    
    public Grid(Grid<T> g){
        grid = new Dictionary<int,Dictionary<int,T>>(g.grid);
        foreach(KeyValuePair<int, Dictionary<int,T>> row in g.grid)
            grid[row.Key] = new Dictionary<int,T>(row.Value);
    }
    
    public T Remove(int x, int y){
        Dictionary<int,T> col;
        if(!grid.TryGetValue(x, out col))
            return null;
        T t;
        if(!col.TryGetValue(y, out t))
            return null;
        col.Remove(y);
        return t;
    }
    
    bool AddIntern(int x, int y, T t, bool force){
        Dictionary<int,T> col;
        if(!grid.TryGetValue(x, out col)){
            col = new Dictionary<int,T>();
            grid.Add(x, col);
            col.Add(y, t);
            return true;
        }
        if(!col.ContainsKey(y)){
            col.Add(y, t);
            return true;
        }
        if(force)
            col[y] = t;
        return false;
    }
    
    public bool AddIfEmpty(int x, int y, T t){
        return AddIntern(x, y, t, false);
    }
    
    public void Add(int x, int y, T t){
        AddIntern(x, y, t, true);
    }
    
    public void Clear(){
        grid = new Dictionary<int,Dictionary<int,T>>();
    }
    
    public T Get(int x, int y){
        Dictionary<int,T> col;
        if(!grid.TryGetValue(x, out col))
            return null;
        T t;
        if(!col.TryGetValue(y, out t))
            return null;
        return t;
    }
    
    public bool Exist(int x, int y){
        Dictionary<int,T> col;
        if(!grid.TryGetValue(x, out col))
            return false;
        return col.ContainsKey(y);
    }
    
    public List<T> GetList(){
        List<T> list = new List<T>();
        foreach(KeyValuePair<int, Dictionary<int,T>> row in grid)
            foreach(KeyValuePair<int, T> cell in row.Value)
                list.Add(cell.Value);
        return list;
    }
}