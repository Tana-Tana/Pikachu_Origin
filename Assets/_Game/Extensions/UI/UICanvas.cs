using UnityEngine;

namespace _Game.Extensions.UI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField] private bool isDestroyOnClose = false;

        public virtual void SetUp() // khoi tao canvas
        { }

        public virtual void Open() // mo canvas
        {
            gameObject.SetActive(true);
        }

        public virtual void Close(float delay) // dong canvas sau delay time
        {
            Invoke(nameof(CloseDirectly), delay);
        }

        public virtual void CloseDirectly() // dong canvas ngay lap tuc
        {
            if (isDestroyOnClose)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}