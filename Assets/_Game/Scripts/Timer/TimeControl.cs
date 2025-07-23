using _Game.Scripts.Manager;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
    [SerializeField] private float timeLimit = 60f; // Thời gian giới hạn cho trò chơi
    private float currentTime; // Thời gian hiện tại

    private void Start()
    {
        currentTime = timeLimit; // Khởi tạo thời gian hiện tại bằng thời gian giới hạn
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime; // Giảm thời gian hiện tại theo thời gian đã trôi qua
            
            if (currentTime <= 0)
            {
                currentTime = 0; // Đảm bảo thời gian không âm
                GameEvent.Instance.OnTimeUp(); // Gọi sự kiện khi thời gian kết thúc
            }
            
            // update UI
        }
    }
}
