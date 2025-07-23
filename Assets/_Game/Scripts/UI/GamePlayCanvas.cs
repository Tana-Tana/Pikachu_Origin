using _Game.Scripts.Booster;
using UnityEngine;

namespace _Game.Scripts.UI
{
    public class GamePlayCanvas : MonoBehaviour
    {
        public void HintButton()
        {
            BoosterControl.Instance.PlayHint();
        }
    
        public void ShuffleButton()
        {
            BoosterControl.Instance.ShuffleArray();
        }
    }
}
