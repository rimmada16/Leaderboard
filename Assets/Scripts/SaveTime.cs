using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class SaveTime : MonoBehaviour
{
    [Serializable]
    public class SaveData
    {
        public int level;
        public float time;
        public string playerName;
    }

    [Serializable]
    public class SaveDataList
    {
        public List<SaveData> savedData = new List<SaveData>();
    }

    private SaveDataList dataList;
    private string path;

    public int level = 1;
    public float time = 0f;
    public string playerName = "Lebron";

    public bool canIncrementTime;
    
    public static SaveTime Instance;
    
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
        
        path = Application.persistentDataPath + "/SaveData.json";
        InitializeDataList();
        LoadData();
    }

    private void InitializeDataList()
    {
        dataList = new SaveDataList();
    }

    private void Start()
    {
        if (levelDropdown != null)
        {
            levelDropdown.onValueChanged.AddListener(OnLevelDropdownValueChanged);
        }
    }
    
    
    private void OnLevelDropdownValueChanged(int value)
    {
        if (leaderboard != null)
        {
            ShowLeaderboard();
        }
    }
    
    public void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void Update()
    {
        if (canIncrementTime)
        {
            time += Time.deltaTime;
        }
        
        HandleLeaderboard();
    }

    private void HandleLeaderboard()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _leaderboardIsActive = !_leaderboardIsActive;

            if (_leaderboardIsActive)
            {
                SetCursorState(false);
            }
            else if (!_leaderboardIsActive)
            {
                SetCursorState(true);
            }
        }
        
        if (_leaderboardIsActive && !_leaderboardLoaded)
        {
            leaderboard.SetActive(true);
            ShowLeaderboard();
        }
        else if (_leaderboardLoaded && !_leaderboardIsActive)
        {
            leaderboard.SetActive(false);
            _leaderboardLoaded = false;
        }
    }
    
    public void AssignName()
    {
        Debug.Log("called assign name");
        playerName = nameInputField.text.Trim();
        SetCursorState(true);
        canvas.SetActive(false);
        Time.timeScale = 1f;
        SetSaveData(level, time, playerName);
    }
    
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject canvas;

    public void OpenNameMenu()
    {
        Debug.Log("called open name menu");
        canvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void SetSaveData(int level, float time, string name)
    {
        // Initialize dataList if it's null (it shouldn't be, but just in case)
        if (dataList == null)
        {
            dataList = new SaveDataList();
        }

        SaveData newData = new SaveData
        {
            level = level,
            time = time,
            playerName = name
        };

        // Check if savedData is initialized
        if (dataList.savedData == null)
        {
            dataList.savedData = new List<SaveData>();
        }

        dataList.savedData.Add(newData);

        // Check if the file exists, create it if it doesn't
        if (!File.Exists(path))
        {
            string json = JsonUtility.ToJson(new SaveDataList());
            File.WriteAllText(path, json);
            Debug.Log("New save file created at: " + path);
        }

        // Write to the existing file
        string saveJson = JsonUtility.ToJson(dataList);
        File.WriteAllText(path, saveJson);
        Debug.Log("Data saved to: " + path);
    }

    private void LoadData()
    {
        if (File.Exists(path))
        {
            try
            {
                // Read existing data from file
                string json = File.ReadAllText(path);
                dataList = JsonUtility.FromJson<SaveDataList>(json);
                Debug.Log("Loaded existing save data.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error loading save data: " + ex.Message);
                InitializeDataList(); // Reinitialize on error
            }
        }
        else
        {
            Debug.Log("No previous save data found. Initializing new data list and creating save file.");
            // Create a new save file
            InitializeDataList(); // Ensure dataList is initialized
            string json = JsonUtility.ToJson(dataList);
            File.WriteAllText(path, json);
            Debug.Log("New save file created at: " + path);
        }
    }

    [SerializeField] private GameObject leaderboard;
    private bool _leaderboardIsActive;
    private bool _leaderboardLoaded;
    [SerializeField] private TMP_Text leaderboardText;
    [SerializeField] private TMP_Dropdown levelDropdown;
    
    public enum Level
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3
    }

    public Level levelEnum;
    private List<SaveData> filteredData = new List<SaveData>();

    public void ShowLeaderboard()
    {
        _leaderboardLoaded = true;
        LoadData();

        if (levelDropdown != null)
        {
            switch (levelDropdown.value)
            {
                case 0:
                    levelEnum = Level.Level1;
                    break;
                case 1:
                    levelEnum = Level.Level2;
                    break;
                case 2:
                    levelEnum = Level.Level3;
                    break;
            }
        }
        
        
        levelEnum = (Level)(levelDropdown.value + 1);
        filteredData.Clear();
        
        if (dataList.savedData == null || dataList.savedData.Count == 0)
        {
            Debug.Log("No save data available for leaderboard.");
        }
        
        foreach (SaveData entry in dataList.savedData)
        {
            // Compare the entry's level with the selectedLevel directly as enums
            if (entry.level == (int)levelEnum)
            {
                filteredData.Add(entry);
            }
        }
        
        filteredData.Sort((a, b) => a.time.CompareTo(b.time));
        
        var sb = new StringBuilder();
        
        if (filteredData.Count == 0)
        {
            sb.AppendLine("No entries");
        }
        else
        {
            int count = Mathf.Min(10, filteredData.Count);

            // Iterate through the top 'count' entries to build the leaderboard display
            for (int i = 0; i < count; i++)
            {
                SaveData entry = filteredData[i];
                sb.AppendLine($"{entry.playerName}, Level: {entry.level}, Time: {entry.time:F2}s");
            }
        }
        
        // Set the formatted leaderboard text to the TMP_Text component
        leaderboardText.text = sb.ToString();
        
        foreach (SaveData entry in dataList.savedData)
        {
            Debug.Log(entry.playerName + ", Time: " + entry.time);
        }
    }
}
