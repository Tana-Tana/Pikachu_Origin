using _Game.Extensions.DP;
using _Game.Scripts.Effect;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    [Header("Effect match", order = 1)]
    [SerializeField] private Transform lineParent;

    [Header("Effect Background", order = 2)]
    [SerializeField] private GameObject shineGamePlay;
    [SerializeField] private GameObject shineFreeze;
    [SerializeField] private GameObject shineEndGame;

    #region effect matching
    public void OnDraw(int numberPoint, Vector2Int[] pointsPos)
    {
        SimplePool.Spawn<LineDrawer>(PoolType.LINE_MATCH, lineParent.transform.position,
                lineParent.transform.rotation)
            .DrawLine(numberPoint, pointsPos);
    }

    public void OnEffectMatching(Vector3 positionOne, Vector3 positionTwo)
    {
        SimplePool.Spawn<EffectMatching>(PoolType.EFFECT_MATCH, positionOne, Quaternion.identity).OnInit(0.5f);
        SimplePool.Spawn<EffectMatching>(PoolType.EFFECT_MATCH, positionTwo, Quaternion.identity).OnInit(0.5f);
    }
#endregion

    #region effect shuffle

    public void OnDespawnEffectShuffle()
    {
        for (int i = 1; i <= LevelManager.Instance.GetRows; ++i)
        {
            for (int j = 1; j <= LevelManager.Instance.GetColumns; ++j)
            {
                LevelManager.Instance.GetTiles[i,j].OnDespawn();
            }
        }
    }

    public void OnInitEffectShuffle()
    {
        for (int i = 1; i <= LevelManager.Instance.GetRows; ++i)
        {
            for (int j = 1; j <= LevelManager.Instance.GetColumns; ++j)
            {
                if (!LevelManager.Instance.GetTiles[i,j].IsDone) // neu chua duoc match
                {
                    LevelManager.Instance.GetTiles[i,j].OnInit();
                }
            }
        }
    }

#endregion

    #region effect gamePlay

    public void OnEffectGamePlay() 
    {
        shineGamePlay.SetActive(true);
        shineFreeze.SetActive(false);
        shineEndGame.SetActive(false);
    }

    public void OnEffectFreezeTime() 
    {
        shineGamePlay.SetActive(false);
        shineEndGame.SetActive(false);
        shineFreeze.SetActive(true);
    }

    public void OnEffectEndGame()
    {
        shineGamePlay.SetActive(false);
        shineFreeze.SetActive(false);
        shineEndGame.SetActive(true);
    }

    public void OnInitEffect()
    {
        shineGamePlay.SetActive(false);
        shineFreeze.SetActive(false);
        shineEndGame.SetActive(false);
    }
#endregion
}