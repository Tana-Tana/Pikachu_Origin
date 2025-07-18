using UnityEngine;

namespace _Game.Extensions.UI
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField] private bool isDestroyOnClose = false;

        public virtual void SetUp() // khoi tao canvas
        {
            // tai tho doi voi man hinh dai
            RectTransform rect = GetComponent<RectTransform>();
            float ratio = (float)Screen.width / Screen.height;

            if (ratio > 2.1f)
            {
                Vector2 leftBottom = rect.offsetMin;
                Vector2 rightTop = rect.offsetMax;

                leftBottom.y = 0;
                rightTop.y = -100f;
                
                rect.offsetMin = leftBottom;
                rect.offsetMax = rightTop;
            }
            
        }

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