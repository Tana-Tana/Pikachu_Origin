using System.IO;
using _Game.Extensions.DP;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private LevelData[] levelsData = new LevelData[100];
    [SerializeField] private UserData userData = new();
    [SerializeField] private TileDatabase tileDatabase; // keo tu Resources

    private readonly string userId = "@User14032004"; // id cua user, co the thay doi sau nay de phan biet cac user voi nhau neu lam online

    public void OnInit()
    {
        LoadData();
    }

    public void OnDespawn()
    {
        //
    }

    public LevelData GetLevelData(int level)
    {
        if (level >= levelsData.Length)
        {
            return null;
        }

        if (levelsData[level].LevelIndex == -1)
        {
            Debug.LogWarning($"Level data for level {level} is not initialized.");
            return null;
        }

        return levelsData[level]; // tra ve level data tu mang co san
    }

    public void SaveUserData()
    {
        string json = JsonUtility.ToJson(userData, true); // format JSON cho de doc
        string path = Path.Combine(GetPathFolder(GameConfig.NAME_FOLDER_SAVE_USERS), userId + ".json");

        File.WriteAllText(path, json);
        Debug.Log("Saved data to: " + path);
    }

    public TileData GetTileData(int type)
    {
        foreach (TileData tileData in tileDatabase.TilesData)
        {
            if ((int)tileData.eTypeTile == type)
            {
                return tileData; // tra ve tile data tu database
            }
        }

        Debug.LogWarning("Chua ton tai data cua tile nay");
        return null;
    }

    #region  Getters and Setters
    public UserData UserData => userData;

    public void SetUserData(UserData userData)
    {
        this.userData = userData;
    }
    
    public TileDatabase TileDatabase => tileDatabase;

    #endregion

    private void LoadData()
    {
        LoadUserData();
        LoadAllLevelData();
    }

    private void LoadAllLevelData()
    {
        string[] files = Directory.GetFiles(GetPathFolder(GameConfig.NAME_FOLDER_SAVE_LEVELS), "*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            LevelData level = JsonUtility.FromJson<LevelData>(json);

            if (level != null)
            {
                LevelData levelData = new LevelData(level.LevelIndex, level.NameLevel, level.TimeLevel, level.Rows, level.Columns, level.ValuesToAssign);
                // Add to levelsData array or list
                levelsData[level.LevelIndex] = levelData;
            }
        }
    }

    private void LoadUserData()
    {
        string path = Path.Combine(GetPathFolder(GameConfig.NAME_FOLDER_SAVE_USERS), userId + ".json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            UserData data = JsonUtility.FromJson<UserData>(json);

            userData = new UserData(data.Id, data.Name, data.CurrentLevel,
                data.CurrentStar, data.CurrentCoin,
                data.ShuffleAmount, data.HintAmount, data.FreezeAmount);

            Debug.Log("Loaded data from: " + path);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + path);
        }
    }

    private void SaveLevelData(string path)
    {
        // chua can vi hien tai level giu nguyen va khong thay doi, khong co continue
    }

    private string GetPathFolder(string nameFolder)
    {
        string customFolderPath = Path.Combine(Application.dataPath, nameFolder);
        if (!Directory.Exists(customFolderPath))
        {
            Directory.CreateDirectory(customFolderPath); // Tao thu muc neu chua ton tai
        }

        return customFolderPath;
    }
}
