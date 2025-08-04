using System.Collections.Generic;
using _Game.Extensions.DP;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<System.Type, UICanvas> canvasActives = new Dictionary<System.Type, UICanvas>(); // luu canvas dang hoat dong
    private Dictionary<System.Type, UICanvas> canvasPrefabs = new Dictionary<System.Type, UICanvas>(); // luu canvas da duoc khoi tao

    [SerializeField] private Transform parent; // canvas cha, canvas con se duoc gan vao

    public void Awake()
    {
        UICanvas[] prefabs = Resources.LoadAll<UICanvas>(GameConfig.PATH_UI); // load tat ca canvas tu Resources
        foreach (UICanvas canvas in prefabs) // luu lai prefab canvas
        {
            canvasPrefabs.Add(canvas.GetType(), canvas);
        }
    }

    public void OnInit()
    {
        OpenUI<CanvasMenu>();
    }

    public T OpenUI<T>() where T : UICanvas // mo 1 canvasUI
    {
        T canvas = GetUI<T>(); // lay canvas ra

        canvas.SetUp(); // khoi tao canvas
        canvas.Open(); // mo canvas

        return canvas;
    }

    public void CloseUI<T>(float delay) where T : UICanvas
    {
        if (IsOpened<T>())
        {
            canvasActives[typeof(T)].Close(delay); // dong canvas sau delay time
        }
    }

    public void CloseDirectly<T>() where T : UICanvas
    {
        if (IsOpened<T>())
        {
            canvasActives[typeof(T)].CloseDirectly(); // dong canvas ngay lap tuc
        }
    }

    public void CloseAll() // dong tat ca canvas dang hoat dong
    {
        foreach (var canvas in canvasActives)
        {
            if (canvas.Value != null && canvas.Value.gameObject.activeSelf)
            {
                canvas.Value.Close(0); // uu tien dung dong trong 0s thay vi dong ngay lap tuc
            }
        }
    }

    public bool IsLoaded<T>() where T : UICanvas // kiem tra xem canvas da duoc tai len hay chua
    {
        return canvasActives.ContainsKey(typeof(T)) && canvasActives[typeof(T)] != null;
    }

    public bool IsOpened<T>() where T : UICanvas // kiem tra xem canvas da duoc mo hay chua
    {
        return IsLoaded<T>() && canvasActives[typeof(T)].gameObject.activeSelf;
    }

    public T GetUI<T>() where T : UICanvas
    {
        if (!IsLoaded<T>()) // neu canvas chua duoc tai len
        {
            T prefab = GetPrefab<T>(); // lay prefab tuong ung voi canvas
            T canvas = Instantiate(prefab, parent); // khoi tao canvas moi tu prefab
            canvasActives[typeof(T)] = canvas; // luu canvas moi vao danh sach canvas dang hoat dong
        }

        return canvasActives[typeof(T)] as T;
    }

    private T GetPrefab<T>() where T : UICanvas
    {
        if (canvasPrefabs.ContainsKey(typeof(T)))
        {
            return canvasPrefabs[typeof(T)] as T; // tra ve canvas prefab
        }
        else
        {
            Debug.LogError($"Canvas {typeof(T).Name} not found in prefabs.");
            return null; // neu khong tim thay canvas, tra ve null
        }
    }
}
