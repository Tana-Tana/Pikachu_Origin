using UnityEngine;

public class GameUnit : MonoBehaviour
{
    [SerializeField] private PoolType poolType;
    public PoolType PoolType => poolType; // PoolType là enum, có thể dùng để phân loại các loại GameUnit khác nhau
    
    // Cache Transform để tối ưu hiệu năng
    private Transform tf;
    public Transform TF
    {
        get
        {
            if (tf == null)
            {
                tf = transform;
            }

            return tf;
        }
    }
}

