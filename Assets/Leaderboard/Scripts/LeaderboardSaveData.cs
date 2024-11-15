using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace Leaderboard.Scripts
{
    /// <summary>
    /// Handles the saving and loading of leaderboard data.
    /// </summary>
    public class LeaderboardSaveData : MonoBehaviour
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

        private SaveDataList _dataList;
        private string _path;

        public static LeaderboardSaveData Instance;
    
        private int _level = 1;
        private float _time;
        private string _playerName = "Lebron";

        [Header("Timer Handler")]
        public bool canIncrementTime;
        public bool inLap;
    
        [Header("Leaderboard References")]
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameObject leaderboard;
        [SerializeField] private TMP_Text leaderboardText;
        [SerializeField] private TMP_Dropdown levelDropdown;
        [SerializeField] private TMP_Text latestEntryText;
    
        // Plug this into your PlayerController/GameManager so you can prevent camera movement while in the leaderboard
        public bool leaderboardIsActive; 
        private bool _leaderboardLoaded;
    
        public enum Level
        {
            // Depending on the amount of levels you have, you can add more here
            Level1 = 1,
            Level2 = 2,
            Level3 = 3
        }
    
        [Header("Selected Level")]
        public Level levelEnum = Level.Level1; // Change the selected level in the inspector - default is level one
        private List<SaveData> _filteredData = new();
    
        /// <summary>
        /// Sets the instance of the LeaderboardSaveData. Whilst initialising the data list and loading the data.
        /// </summary>
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
        
            _path = Application.persistentDataPath + "/SaveData.json";
            InitialiseDataList();
            LoadData();
        }

        /// <summary>
        /// Initialises the data list.
        /// </summary>
        private void InitialiseDataList()
        {
            _dataList = new SaveDataList();
        }

        /// <summary>
        /// Adds a listener to the level dropdown on the leaderboard.
        /// </summary>
        private void Start()
        {
            if (levelDropdown != null)
            {
                levelDropdown.onValueChanged.AddListener(OnLevelDropdownValueChanged);
            }
        }
    
        /// <summary>
        /// Updates the selected level when the dropdown value changes.
        /// </summary>
        /// <param name="value"></param>
        private void OnLevelDropdownValueChanged(int value)
        {
            if (leaderboard != null)
            {
                ShowLeaderboard();
            }
        }

        /// <summary>
        /// Handles the lap timer and the call for the leaderboard.
        /// </summary>
        private void Update()
        {
            if (canIncrementTime)
            {
                _time += Time.deltaTime;
            }
        
            HandleLeaderboard();
        }

        /// <summary>
        /// Handles the leaderboard display.
        /// </summary>
        private void HandleLeaderboard()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // Alter this to prevent leaderboard opening when the input field is active in your game
                if (DemoGameManager.Instance.inputFieldActive)
                {
                    return;
                }
                
                leaderboardIsActive = !leaderboardIsActive;

                if (leaderboardIsActive)
                {
                    CursorHandler.Instance.SetCursorState(false);
                }
                else if (!leaderboardIsActive)
                {
                    CursorHandler.Instance.SetCursorState(true);
                }
            }
        
            if (leaderboardIsActive && !_leaderboardLoaded)
            {
                leaderboard.SetActive(true);
                ShowLeaderboard();
            }
            else if (_leaderboardLoaded && !leaderboardIsActive)
            {
                leaderboard.SetActive(false);
                _leaderboardLoaded = false;
            }
        }
    
        /// <summary>
        /// Opens the name input menu.
        /// </summary>
        public void OpenNameMenu()
        {
            CursorHandler.Instance.SetCursorState(false);
            canvas.SetActive(true);
            Time.timeScale = 0f;
        }
        
        /// <summary>
        /// Sets the player name and hides the name input field.
        /// </summary>
        public void AssignName()
        {
            _playerName = nameInputField.text.Trim();
            
            CursorHandler.Instance.SetCursorState(true);
            canvas.SetActive(false);
            Time.timeScale = 1f;
            SetSaveData((int)levelEnum, _time, _playerName);
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <param name="level">The current level</param>
        /// <param name="time">The time of the entry</param>
        /// <param name="playerName">The name of the player making the entry</param>
        private void SetSaveData(int level, float time, string playerName)
        {
            // Initialise dataList if it's null
            if (_dataList == null)
            {
                _dataList = new SaveDataList();
            }

            var newData = new SaveData
            {
                level = level,
                time = time,
                playerName = playerName
            };

            // Check if savedData is initialised
            if (_dataList.savedData == null)
            {
                _dataList.savedData = new List<SaveData>();
            }

            _dataList.savedData.Add(newData);

            // Check if the file exists, create it if it doesn't
            if (!File.Exists(_path))
            {
                var json = JsonUtility.ToJson(new SaveDataList());
                File.WriteAllText(_path, json);
                Debug.Log("New save file created at: " + _path);
            }

            // Write to the existing file
            var saveJson = JsonUtility.ToJson(_dataList);
            File.WriteAllText(_path, saveJson);
            Debug.Log("Data saved to: " + _path);
        }

        /// <summary>
        /// Loads saved leaderboard data from a JSON file. If no data exists, it initializes a new save file.
        /// </summary>
        /// <remarks>
        /// This method attempts to read saved data from the specified file path. 
        /// If the file does not exist, it creates a new save file with an empty data list. 
        /// If there is an error during loading (e.g., file corruption), it initialises a new data list and logs an error.
        /// </remarks>
        private void LoadData()
        {
            if (File.Exists(_path))
            {
                try
                {
                    // Read existing data from file
                    string json = File.ReadAllText(_path);
                    _dataList = JsonUtility.FromJson<SaveDataList>(json);
                    Debug.Log("Loaded existing save data.");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error loading save data: " + ex.Message);
                    InitialiseDataList(); // Reinitialise on error
                }
            }
            else
            {
                Debug.Log("No previous save data found. Initialising new data list and creating save file.");
                // Create a new save file
                InitialiseDataList(); // Ensure dataList is initialised
                string json = JsonUtility.ToJson(_dataList);
                File.WriteAllText(_path, json);
                Debug.Log("New save file created at: " + _path);
            }
        }

        /// <summary>
        /// Handles the leaderboard display.
        /// </summary>
        private void ShowLeaderboard()
        {
            _leaderboardLoaded = true;
            LoadData();

            // Ensure the levelDropdown is not null
            if (levelDropdown != null)
            {
                levelEnum = (Level)(levelDropdown.value + 1);
            }
        
            _filteredData.Clear();
        
            if (_dataList.savedData == null || _dataList.savedData.Count == 0)
            {
                Debug.Log("No save data available for the leaderboard.");
            }

            if (_dataList.savedData != null)
            {
                foreach (var entry in _dataList.savedData)
                {
                    // Compare the entry's level with the selectedLevel
                    if (entry.level == (int)levelEnum)
                    {
                        _filteredData.Add(entry);
                    }
                }

                _filteredData.Sort((a, b) => a.time.CompareTo(b.time));

                var stringBuilder = new StringBuilder();

                if (_filteredData.Count == 0)
                {
                    stringBuilder.AppendLine("No entries");
                }
                else
                {
                    var count = Mathf.Min(10, _filteredData.Count);

                    // Iterate through the top 'count' entries to build the leaderboard display
                    for (int i = 0; i < count; i++)
                    {
                        var entry = _filteredData[i];
                        stringBuilder.AppendLine($"{entry.playerName}, Level: {entry.level}, Time: {entry.time:F2}s");
                    }
                }

                // Set the formatted leaderboard text to the TMP_Text component
                leaderboardText.text = stringBuilder.ToString();

                foreach (var entry in _dataList.savedData)
                {
                    Debug.Log(entry.playerName + ", Time: " + entry.time);
                }

                if (_dataList.savedData != null && _dataList.savedData.Count > 0)
                {
                    // Filter entries to find the last entry for the current selected level
                    var latestEntryForLevel = _dataList.savedData
                        .LastOrDefault(entry => entry.level == (int)levelEnum);

                    if (latestEntryForLevel != null)
                    {
                        // Display the time of the last entry for the selected level
                        latestEntryText.text = $"Last Logged Time for {levelEnum}: {latestEntryForLevel.time:F2}s";
                    }
                    else
                    {
                        // No entries for the selected level
                        latestEntryText.text = $"Last Logged Time for {levelEnum}: No Entries";
                    }
                }
                else
                {
                    latestEntryText.text = "No save data found to display last logged time!";
                }
            }
        }
    }
}
