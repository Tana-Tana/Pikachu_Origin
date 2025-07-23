using _Game.Common;
using UnityEngine;

namespace _Game.Scripts.Tile
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D col; // collider de nhan su kien click
        [SerializeField] private SpriteRenderer spriteRenderer; // sprite hien thi hinh anh tile
        [SerializeField] private GameObject highlight; // hieu ung highlight khi tile duoc chon
        [SerializeField] private Animator anim;
        
        private Transform tf;
        private DataTile dataTile; // du lieu ve tile
		private bool isSelected; // trang thai cua tile, co duoc chon hay khong
        private bool isMatching; // trang thai cua tile co dang duoc match hay chua
        private bool isDoneTask; // trang thai cua tile da hoan thanh nhiem vu hay chua
        private int x;
        private int y;
        private string animName; // ten anim hien tai

        public void OnInit()
        {
            isSelected = false;
            isMatching = false;
            isDoneTask = false;
            highlight.SetActive(isSelected);
            spriteRenderer.color = Color.white;
            
            ActiveGameObject();
        }

        public virtual void OnDespawn()
        {
            PlayEffectDisappear();
            Invoke(nameof(DeActiveGameObject), 0.35f);
        }
        
        public virtual void SetData(DataTile dataTile)
        {
            this.dataTile = dataTile;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = dataTile.sprite; // gan hinh anh cho spriteRenderer
            }
        }
        
        public void ActiveGameObject()
        {
            gameObject.SetActive(true);
            ChangeAnim(GameConfig.ANIM_TILE_APPEAR);
        }
        
        public void DeActiveGameObject()
        {
            gameObject.SetActive(false);
        }

        public void SetLocationInMatrix(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public void SetName(string name)
        {
            this.name = name;
        }
        
        private void PlayEffectDisappear()
        {
            ChangeAnim(GameConfig.ANIM_TILE_DISAPPEAR);
        }
        
        public void SetActiveMatching()
        {
            isMatching = true;
        }

        public void SetDeActiveMatching()
        {
            isMatching = false;
        }
        
        public void SetActiveSelected()
        {
            ChangeAnim(GameConfig.ANIM_TILE_SELECT);
            isSelected = true;
            highlight.SetActive(isSelected);
        }

        public void SetDeActiveSelected()
        {
            ChangeAnim(GameConfig.ANIM_TILE_UNSELECT);
            isSelected = false;
            highlight.SetActive(isSelected);
        }

        private void ChangeAnim(string animName)
        {
            if (this.anim != null && this.animName != animName)
            {
                if (this.animName != null && !this.animName.Equals("")) anim.ResetTrigger(this.animName);
                this.animName = animName;
                anim.SetTrigger(this.animName);   
            }
        }
        
        public void SetDoneTask()
        {
            isDoneTask = true;
        }
        
        public int X => x;
        public int Y => y;
        public DataTile Data => dataTile;
        public bool IsMatching => isMatching;
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

        public bool IsDoneTask => isDoneTask;
    }
}
