using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null; // sprite hien thi hinh anh tile
    [SerializeField] private GameObject highlight = null; // hieu ung highlight khi tile duoc chon
    [SerializeField] private Animator anim = null;

    private Transform tf = null;
    private TileData tileData = null; // du lieu ve tile
    private bool isSelected = false; // trang thai cua tile, co duoc chon hay khong
    private bool isDone = false; // trang thai cua tile da hoan thanh nhiem vu hay chua
    private bool isMatching = false; // trang thai cua tile dang matching hay khong
    private string animName = ""; // ten anim hien tai
    private int x,y = 0; // luu vi tri hang cot hien tai

    public void OnInit()
    {
        ChangeAnim(GameConfig.ANIM_TILE_APPEAR);
        SetActiveObject();
        isSelected = false;
        isDone = false;
        isMatching = false;
        highlight.SetActive(isSelected);
    }

    public void OnDespawn()
    {
        if (GameManager.Instance.IsState(GameState.NEXT_LEVEL))
        {
            Destroy(gameObject);
        }
        else if (GameManager.Instance.IsState(GameState.SHUFFLE) || GameManager.Instance.IsState(GameState.WIN_GAME) || GameManager.Instance.IsState(GameState.LOSE_GAME))
        {
            ChangeAnim(GameConfig.ANIM_TILE_DISAPPEAR);
            Invoke(nameof(SetDeActiveObject), 0.5f);
        }
        else
        {
            SetDeActiveObject();
        }
    }

    public void OnSelect()
    {
        ChangeAnim(GameConfig.ANIM_TILE_SELECT);
        isSelected = true;
        highlight.SetActive(isSelected);
    }

    public void OnDeselect()
    {
        ChangeAnim(GameConfig.ANIM_TILE_UNSELECT);
        isSelected = false;
        highlight.SetActive(isSelected);
    }

    public void OnMatch()
    {
        ChangeAnim(GameConfig.ANIM_TILE_MATCH);
        isDone = true;
        isMatching = true;
        Invoke(nameof(OnDespawn), 0.5f);
    }
    
    private void ChangeAnim(string animName)
    {
        if (this.animName != animName)
        {
            if (!this.animName.Equals("")) anim.ResetTrigger(this.animName);
            this.animName = animName;
            anim.SetTrigger(this.animName);
        }
    }

    #region Getters

    public int X => x;
    public int Y => y;

    public TileData TileData => tileData;
    public ETypeTile GetTypeData => tileData.eTypeTile;
    public Sprite GetSprite => tileData.sprite;

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

    public bool IsDone => isDone;
    public bool IsMatching => isMatching;

    #endregion
    
    #region Setters
    public void SetActiveObject()
    {
        gameObject.SetActive(true);
    }
    
    public void SetDeActiveObject()
    {
        gameObject.SetActive(false);
    }
    
    public void SetTileData(TileData tileData)
    {
        this.tileData = tileData;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = this.tileData.sprite; // gan hinh anh cho spriteRenderer
        }
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public void SetPosInMatrix(int x, int y) 
    {
        this.x = x;
        this.y = y;
    }
    #endregion
    
    
}