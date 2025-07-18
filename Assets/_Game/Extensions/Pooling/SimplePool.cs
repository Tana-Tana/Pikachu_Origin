using System.Collections.Generic;
using UnityEngine;

public static class SimplePool // quan ly cac pool
{
    private static Dictionary<PoolType, Pool> poolInstance = new Dictionary<PoolType, Pool>(); // quan ly cac pool
    
    public static void PreLoad(GameUnit prefab, int amount, Transform parent) // khoi tao pool
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab cannot be null");
            return;
        }
        
        if (!poolInstance.ContainsKey(prefab.PoolType) || poolInstance[prefab.PoolType] == null) // kiem tra pool da ton tai hay chua hoac da ton tai nhung van con null
        {
            Pool pool = new Pool();
            poolInstance[prefab.PoolType] = pool;
            poolInstance[prefab.PoolType].Preload(prefab, amount, parent);
        }
    }

    public static T Spawn<T>(PoolType poolType, Vector3 pos, Quaternion rot) where T : GameUnit // lay phan tu ra khoi pool
    {
        if (!poolInstance.ContainsKey(poolType)) // kiem tra pool da ton tai hay chua
        {
            Debug.LogError("Pool does not exist for type: " + poolType);
            return null;
        }
        
        return poolInstance[poolType].Spawn(pos, rot) as T;
    }

    public static void Despawn(GameUnit unit) // tra phan tu ve pool
    {
        if (!poolInstance.ContainsKey(unit.PoolType)) // kiem tra pool da ton tai hay chua
        {
            Debug.LogError("Pool does not exist for type: " + unit.PoolType);
            return;
        }
        
        poolInstance[unit.PoolType].Despawn(unit); // tra phan tu ve pool
    }

    public static void Collect(PoolType poolType) // thu thap cac phan tu ve pool
    {
        if (!poolInstance.ContainsKey(poolType)) // kiem tra pool da ton tai hay chua
        {
            Debug.LogError("Pool does not exist for type: " + poolType);
            return;
        }
        
        poolInstance[poolType].Collect(); // thu thap cac phan tu ve pool
    }

    public static void CollectAll() // thu thap tat ca cac phan tu ve tat ca cac pool
    {
        foreach (var pool in  poolInstance)
        {
            pool.Value.Collect();
        }
    }

    public static void Release(PoolType poolType) // giai phong tat ca phan tu trong pool 
    {
        if (!poolInstance.ContainsKey(poolType)) // kiem tra pool da ton tai hay chua
        {
            Debug.LogError("Pool does not exist for type: " + poolType);
            return;
        }
        
        poolInstance[poolType].Release(); // giai phong pool, xoa het cac phan tu trong pool
    }

    public static void ReleaseAll() // giai phong tat ca cac phan tu trong tat ca cac pool
    {
        foreach (var pool in poolInstance)
        {
            pool.Value.Release(); // goi phuong thuc Release cua tung pool
        }
        
        poolInstance.Clear(); // xoa het cac pool trong danh sach quan ly
    }
}

public class Pool // dac diem cua 1 pool
{
    private Transform parent; // cha chua pool nay khi duoc sinh ra
    private GameUnit prefab; // prefab cua pool nay
    
    private Queue<GameUnit> inactives = new Queue<GameUnit>(); // hang doi cac GameUnit dang co trong pool
    private List<GameUnit> actives = new List<GameUnit>(); // danh sach cac GameUnit dang duoc su dung

    public Pool() // khoi tao pool
    {
        inactives.Clear();
        actives.Clear();
    }
    
    public void Preload(GameUnit prefab, int amount, Transform parent) // chuan bi cac GameUnit cho pool
    {
        this.prefab = prefab;
        this.parent = parent;
        
        if (this.prefab == null)
        {
            Debug.LogError("Prefab cannot be null");
            return;
        }
        
        for (int i = 0; i < amount; ++i)
        {
            Despawn(GameObject.Instantiate(this.prefab, this.parent)); // sinh ra cac prefab cua pool voi so luong = amount roi tra ve pool
        }
    }

    public GameUnit Spawn(Vector3 pos, Quaternion rot) // lay phan tu ra khoi pool
    {
        GameUnit unit;

        if (inactives.Count <= 0) // neu trong pool chua co phan tu nao thi sinh ra mot phan tu moi
        {
            unit = GameObject.Instantiate(prefab,parent);
        }
        else // neu co phan tu trong pool thi lay phan tu dau tien trong hang doi
        {
            unit = inactives.Dequeue();
        }
        
        actives.Add(unit); // them phan tu vao danh sach dang su dung
        unit.gameObject.SetActive(true); // kich hoat phan tu
        unit.TF.SetPositionAndRotation(pos, rot); // dat vi tri va huong cua phan tu

        return unit;
    }
    
    public void Despawn(GameUnit unit) // tra phan tu ve pool
    {
        if (unit != null && unit.gameObject.activeSelf) // kiem tra xem phan tu co ton tai va dang duoc su dung
        {
            actives.Remove(unit); // xoa phan tu khoi danh sach dang su dung
            inactives.Enqueue(unit); // dua phan tu vao hang doi
            unit.gameObject.SetActive(false); // tat phan tu
        }
    }

    public void Collect() // thu thap tat ca cac phan tu ve pool
    {
        while (actives.Count > 0)
        {
            Despawn(actives[0]);
        }
    }
    
    public void Release() // giai phong pool, xoa het cac phan tu
    {
        Collect();

        while (inactives.Count > 0)
        {
            GameObject.Destroy(inactives.Dequeue().gameObject); // xoa het cac phan tu trong hang doi
        }
        
        inactives.Clear();
    }
}