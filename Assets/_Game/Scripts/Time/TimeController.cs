using _Game.Extensions.DP.ObserverPattern;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float countdownTime = 60f; // Thời gian giới hạn cho trò chơi
    private float timeLeft = 0f; // Thời gian còn lại

    // freezeTime
    private float freezeTime = 0;

    public void OnInit(float countdownTime)
    {
        this.countdownTime = countdownTime;
        timeLeft = countdownTime;
        UIManager.Instance.GetUI<CanvasGamePlay>().UpdateTimer(1f);
        freezeTime = 0f;
    }

    private void Update()
    {
        if (IsFreezeTime)
        {
            OnFreezeTime();
        }
        else
        {
            OnNormalTime();
        }
    }

    private void OnNormalTime()
    {
        if ((GameManager.Instance.IsState(GameState.GAME_PLAY) || GameManager.Instance.IsState(GameState.HINT)
        || GameManager.Instance.IsState(GameState.SHUFFLE)) && timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            UIManager.Instance.GetUI<CanvasGamePlay>().UpdateTimer(timeLeft / countdownTime);
            CheckAndOpenWarningEffect();

            // khi het gio
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                UIManager.Instance.GetUI<CanvasGamePlay>().UpdateTimer(0f);

                GameManager.Instance.ChangeState(GameState.LOSE_GAME);
                GamePlayManager.Instance.OnLoseGame();
                UIManager.Instance.GetUI<CanvasGamePlay>().SetActiveOverlayUI();
            }
        }
    }

    private void CheckAndOpenWarningEffect()
    {
        if (timeLeft <= 20f)
        {
            if (!UIManager.Instance.GetUI<CanvasGamePlay>().IsActiveTimeOutBorder)
            {
                UIManager.Instance.GetUI<CanvasGamePlay>().SetActiveTimeOutBorder();
            }
        }
    }

    private void OnFreezeTime()
    {
        if ((GameManager.Instance.IsState(GameState.FREEZE_TIME) || GameManager.Instance.IsState(GameState.HINT)
        || GameManager.Instance.IsState(GameState.SHUFFLE)) && freezeTime > 0f)
        {
            freezeTime -= Time.deltaTime;

            if (freezeTime <= 0f)
            {
                freezeTime = 0f;
                UIManager.Instance.GetUI<CanvasGamePlay>().SetDeActiveFreezeAction();
                EffectManager.Instance.OnEffectGamePlay();

                GameManager.Instance.ChangeState(GameState.GAME_PLAY);
                CheckAndOpenWarningEffect();
            }
        }
    }

    public void SetFreezeTime(float time)
    {
        freezeTime = time;
    }

    public bool IsFreezeTime => freezeTime > 0;
}