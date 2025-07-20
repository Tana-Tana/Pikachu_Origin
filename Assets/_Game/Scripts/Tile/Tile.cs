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
        private int x;
        private int y;
        private string animName; // ten anim hien tai


        public virtual void OnInit(DataTile dataTile)
        {
            this.dataTile = dataTile;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = dataTile.sprite; // gan hinh anh cho spriteRenderer
            }
        }

        public virtual void OnDespawn()
        {
            DeActiveGameObject();
        }
        
        public void ActiveGameObject()
        {
            gameObject.SetActive(true);
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
        
        public virtual void PlayEffectMatch()
        {
            ChangeAnim(GameConfig.ANIM_TILE_MATCH);
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
            isSelected = false;
            highlight.SetActive(isSelected);
        }

        private void ChangeAnim(string animName)
        {
                if (this.animName != null && !this.animName.Equals("")) anim.ResetTrigger(this.animName);
                this.animName = animName;
                anim.SetTrigger(this.animName);
        }
        
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
    }
}
