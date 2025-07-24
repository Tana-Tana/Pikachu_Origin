using System;
using System.Collections;
using _Game.Scripts.Manager;
using UnityEngine;

namespace _Game.Scripts.Timer
{
    public class TimeControl : MonoBehaviour
    {
        [SerializeField] private float countdownTime = 60f; // Thời gian giới hạn cho trò chơi
        private float timeLeft; // Thời gian còn lại
        private float currentTime; // Thời gian hiện tại
        private float smoothFill = 1f;
        private float fillVelocity = 0f;
        public float smoothTime = 0.15f; // càng nhỏ càng mượt
        
        private Coroutine countdownCoroutine; // Coroutine để đếm ngược thời gian
        
        private void Start()
        {
            StartCountdown();
        }

        private void Update()
        {
            if (GameManager.IsState(GameState.GAME_PLAY) && timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                float targetFill = Mathf.Clamp01(timeLeft / countdownTime);

                // Tween mượt tới giá trị mới
                smoothFill = Mathf.SmoothDamp(smoothFill, targetFill, ref fillVelocity, smoothTime);
                Messenger.Broadcast(EventKey.UpdateTimer, smoothFill);


                // OPTIONAL: Khi hết giờ
                if (timeLeft <= 0f)
                {
                    timeLeft = 0f;
                    Messenger.Broadcast(EventKey.UpdateTimer, 0f);
                    GameEvent.Instance.OnTimeUp();
                }
            }
        }

        private void StartCountdown()
        {
            timeLeft = countdownTime;
            smoothFill = 1f;
            Messenger.Broadcast(EventKey.UpdateTimer, 1f);
        }
    }
}
