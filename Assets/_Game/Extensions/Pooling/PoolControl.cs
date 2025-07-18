using UnityEngine;

public class PoolControl : MonoBehaviour
{
    // cach 1: su dung keo tha tren scene
    [SerializeField] private PoolData[] poolData;

    private void Awake()
    {
        // cach 1: su dung keo tha tren scene
        for (int i = 0; i < poolData.Length; i++)
        {
            SimplePool.PreLoad(poolData[i].Prefab, poolData[i].Amount, poolData[i].Parent);
        }
    }
}

[System.Serializable] 
public class PoolData
{
    [SerializeField] private GameUnit prefab;
    public GameUnit Prefab => prefab;

    [SerializeField] private int amount;
    public int Amount => amount;
    
    [SerializeField] private Transform parent;
    public Transform Parent => parent;
}

public enum PoolType
{
    LINE_MATCH = 0,
}