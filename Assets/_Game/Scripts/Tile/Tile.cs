using System;
using UnityEngine;

namespace _Game.Scripts.Tile
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D col; // collider de nhan su kien click
        public BoxCollider2D Col => col;
        
        [SerializeField] private SpriteRenderer spriteRenderer; // sprite hien thi hinh anh tile
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        
        [SerializeField] private GameObject highlight; // hieu ung highlight khi tile duoc chon

		private bool isSelected = false; // trang thai cua tile, co duoc chon hay khong
		public bool IsSelected => isSelected;
        
        private bool isMatching = false; // trang thai cua tile co dang duoc match hay chua
        public bool IsMatching => isMatching;
        
        private int x;
        private int y;
        
        private TileData tileData; // du lieu ve tile
        public TileData Data => tileData;

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

        public virtual void OnInit(TileData tileData)
        {
            this.tileData = tileData;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = tileData.sprite; // gan hinh anh cho spriteRenderer
            }
        }

        public virtual void OnDespawn()
        {
            gameObject.SetActive(false);
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
            //
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
            isSelected = true;
            highlight.SetActive(isSelected);
        }

        public void SetDeActiveSelected()
        {
            isSelected = false;
            highlight.SetActive(isSelected);
        }
    }
}
