using UnityEngine;

[System.Serializable]

public class UserData
{
    [field: SerializeField] public string Id { get; private set; } = "@User14032004"; // phan biet cac user voi nhau
    [field: SerializeField] public string Name { get; private set; } = "Default User"; // ten cua user
    [field: SerializeField] public int CurrentLevel { get; private set; } = 1; // level hien tai cua user
    
    // thong tin ve sao ---> sau co the lay cai nay de lam bang xep hang
    [field: SerializeField] public int CurrentStar { get; private set; } = 0; // So sao hien tai cua user
    // thong tin ve coin --> sau co the dung de mua booster
    [field: SerializeField] public int CurrentCoin { get; private set; } = 0; // So coin hien tai cua user

    // booster amount --> so luong booster hien tai cua user
    [field: SerializeField] public int ShuffleAmount { get; private set; } = 0; // so luong booster shuffle
    [field: SerializeField] public int HintAmount { get; private set; } = 0; // so luong booster hint
    [field: SerializeField] public int FreezeAmount { get; private set; } = 0; // so luong booster freeze
    
    public UserData(string id = "@User14032004", string name = "Default User"
        , int currentLevel = 1, int currentStar = 0, int currentCoin = 0,
        int shuffleAmount = 0, int hintAmount = 0, int freezeAmount = 0)
    {
        Id = id;
        Name = name;
        CurrentLevel = currentLevel;
        CurrentStar = currentStar;
        CurrentCoin = currentCoin;
        ShuffleAmount = shuffleAmount;
        HintAmount = hintAmount;
        FreezeAmount = freezeAmount;
    }

    public void SetDataLevelIndex(int levelIndex) {
        this.CurrentLevel = levelIndex;
    }
}