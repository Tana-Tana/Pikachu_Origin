using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Random = UnityEngine.Random;

public class MatrixGenerator : MonoBehaviour
{
    
    [SerializeField] private string nameLevel = "Default Level"; // ten level
    [SerializeField] private int levelIndex = 1; // level so may
    [SerializeField] private float timeLevel = 0f; // thoi gian level, co the dung sau nay de tinh thoi gian cho tung level
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 5;
    [SerializeField] private TMP_InputField prefabInputField;
    [SerializeField] private float cellSize = 100f;
    [SerializeField] private Transform parentTransform;

    private List<TMP_InputField> inputFields = new List<TMP_InputField>();
    [SerializeField] private List<int> valuesToAssign = new List<int>(); // list so duoc hien thi trong ma tran

    private void GenerateMatrix()
    {
        int totalCells = rows * columns;
        // neu tong o bang le => set gia tri mac dinh
        if (totalCells % 2 != 0)
        {
            rows = Mathf.Min(10, rows);
            columns = Mathf.Min(7, columns);

            totalCells = rows * columns; // cap nhat lai tong o
            if (totalCells % 2 != 0) // neu van le sau khi cap nhat
            {
                rows = 10;
                columns = 7;
            }
        }

        totalCells = rows * columns;
        // gan gia tri can co vao list
        valuesToAssign.Clear();
        for (int i = 0; i < totalCells; i += 2)
        {
            // random gia tri tu 1 den 100
            int value = (int)RandomEnumValue<EDataToSpawn>();
            valuesToAssign.Add(value);
            valuesToAssign.Add(value); // add 2 lan de co cap
        }

        // xao tron danh sach
        Shuffle(valuesToAssign);

        // gan cac gia tri vao input fields
        UpdateListResult(0);
    }

    private void UpdateListResult(int cntValuesToAssign)
    {
        while (inputFields.Count > 0)
        {
            Destroy(inputFields[0].gameObject);
            inputFields.RemoveAt(0);
        }

        inputFields.Clear();
        for (int i = 0; i <= rows + 1; i++)
        {
            for (int j = 0; j <= columns + 1; j++)
            {
                TMP_InputField inputField = Instantiate(prefabInputField, parentTransform);
                inputField.transform.localPosition = GetTilePosition(i, j, cellSize);

                if (i == 0 || j == 0 || i == rows + 1 || j == columns + 1)
                {
                    inputField.text = "0"; // set gia tri mac dinh cho cac o bien
                    inputField.interactable = false; // khong cho nhap vao o bien
                    inputFields.Add(inputField);
                }
                else
                {
                    int index = cntValuesToAssign;
                    inputField.text = valuesToAssign[cntValuesToAssign++].ToString();
                    inputField.interactable = true; // cho nhap vao o trong
                    inputFields.Add(inputField);

                    inputField.onValueChanged.AddListener((value) =>
                    {
                        int idxCouple = 0;
                        for (int i = 0; i < valuesToAssign.Count; i++) // tim cap doi cua gia tri da nhap
                        {
                            if (valuesToAssign[i] == valuesToAssign[index] && index != i)
                            {
                                idxCouple = i;
                                break;
                            }
                        }

                        if (int.TryParse(value, out int intValue))
                        {
                            if (int.Parse(value) > 50 &&
                                int.Parse(value) < 1000) // neu nhap vao so lon hon 50 va nho hon 1000
                            {
                                valuesToAssign[index] =
                                    (int)RandomEnumValue<
                                        EDataToSpawn>(); // random gia tri neu nhap lon hon 50 vi du lieu chi toi 50 va 1 so 1000, neu muon hon thi tu chinh
                                valuesToAssign[idxCouple] = valuesToAssign[index];
                            }
                            else if (int.Parse(value) > 1001) // neu nhap so lon hon 1000 vi du lieu chi toi 1000
                            {
                                valuesToAssign[index] = (int)RandomEnumValue<EDataToSpawn>();
                                valuesToAssign[idxCouple] = valuesToAssign[index];
                            }
                            else
                            {
                                valuesToAssign[index] = intValue;
                                valuesToAssign[idxCouple] = valuesToAssign[index];
                            }
                        }
                        else
                        {
                            valuesToAssign[index] =
                                (int)RandomEnumValue<EDataToSpawn>(); // gan 1 so bat ki neu khong phai la so
                            valuesToAssign[idxCouple] = valuesToAssign[index];
                        }
                    });
                }
            }
        }
    }

    private Vector3 GetTilePosition(int row, int column, float cellSize) // lay vi tri cua tile dua tren hang va cot
    {
        float x = column * cellSize;
        float y = row * cellSize;
        return new Vector3(x, y, 0f);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    //random gia enum trong mot kieu enum
    private static System.Random random = new System.Random();

    public static T RandomEnumValue<T>() where T : Enum
    {
        var v = System.Enum.GetValues(typeof(T));
        return (T)v.GetValue(random.Next(v.Length));
    }


    // ham cho cac nut an o tren scene
    public void OnGenerateButtonClicked()
    {
        GenerateMatrix();
    }

    public void OnShuffleButtonClicked()
    {
        Shuffle(inputFields);
        UpdateListResult(0);
    }

    public void SaveDataToJson()
    {
        string customFolderPath = Path.Combine(Application.dataPath, "SavedLevels");
        if (!Directory.Exists(customFolderPath))
        {
            Directory.CreateDirectory(customFolderPath); // Tao thu muc neu chua ton tai
        }
        
        LevelData data = new LevelData(levelIndex, nameLevel, timeLevel, rows, columns, valuesToAssign);
        
        string json = JsonUtility.ToJson(data, true); // format JSON cho de doc
        string path = Path.Combine(customFolderPath, levelIndex + ".json");

        File.WriteAllText(path, json);
        Debug.Log("Saved data to: " + path);
    }

    public void LoadDataFromJson()
    {
        string customFolderPath = Path.Combine(Application.dataPath, "SavedLevels");
        if (!Directory.Exists(customFolderPath))
        {
            Debug.LogWarning("Custom folder does not exist: " + customFolderPath);
            return; // Không có thư mục, không làm gì cả
        }
        
        string path = Path.Combine(customFolderPath, levelIndex + ".json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LevelData data = JsonUtility.FromJson<LevelData>(json);

            // cap nhat cac truong du lieu
            levelIndex = data.LevelIndex;
            nameLevel = data.NameLevel;
            timeLevel = data.TimeLevel;
            rows = data.Rows;
            columns = data.Columns;
            valuesToAssign = data.ValuesToAssign;
            
            Debug.Log("Loaded data from: " + path);
        }
        else
        {
            Debug.LogWarning("No save file found at: " + path);
        }
    }
}