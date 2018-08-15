using System.Collections;
using System.Collections.Generic;

public class Grid<T> where T : class {
    Dictionary<int,Dictionary<int,T>> grid = new Dictionary<int,Dictionary<int,T>>();
    
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
}