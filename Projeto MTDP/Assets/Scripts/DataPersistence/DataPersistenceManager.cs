using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    private SequenceManager sequenceManager;
    public static DataPersistenceManager instance { get; private set; }
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one DataPersistenceManager in the scene!");
        }
        instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        sequenceManager = FindAnyObjectByType<SequenceManager>();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void SaveGame()
    {
        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            obj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }

    public void LoadGame()
    {
        gameData = dataHandler.Load();
        
        if (gameData == null)
        {
            Debug.Log("No save data was found, initializing defaults...");
            NewGame();
        }

        foreach (IDataPersistence obj in dataPersistenceObjects)
        {
            obj.LoadData(gameData);
        }

        if(sequenceManager != null)
        {
            sequenceManager.PositionPlayerAtSpawn();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void ResetSaveData()
    {
        NewGame();
        SaveGame();
        Debug.Log("Save reset to defaults");
    }

}
