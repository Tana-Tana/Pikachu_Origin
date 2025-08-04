using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData 
{
    [field: SerializeField] public int LevelIndex { get; private set; } = -1; // chi so level
    [field: SerializeField] public string NameLevel { get; private set; } = "Level -1"; // ten level
    [field: SerializeField] public float TimeLevel { get; private set; } = 0f; // thoi gian level, co the dung sau nay de tinh thoi gian cho tung level
    [field: SerializeField] public int Rows { get; private set; } = 10; // so hang
    [field: SerializeField] public int Columns { get; private set; } = 7; // so cot

    [field: SerializeField]
    public List<int> ValuesToAssign { get; private set; } = new List<int>(); // danh sach cac gia tri se duoc gan vao o
    
    public LevelData(int levelIndex, string nameLevel, float timeLevel, int rows, int columns, List<int> valuesToAssign)
    {
        LevelIndex = levelIndex;
        NameLevel = nameLevel;
        TimeLevel = timeLevel;
        Rows = rows;
        Columns = columns;
        ValuesToAssign = valuesToAssign;
    }
}
