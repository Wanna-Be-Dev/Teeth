
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    private GameData gameData;
    public static DataManager instance { get; private set; }

    private List<IDataPersistence> dataPersistencesObjects;
    [Header("File Settings")]
    [SerializeField] private string fileName;

    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Found more then one saver");
        instance = this;
    }
    private void Start()
    {
        dataHandler = new FileDataHandler(Application.streamingAssetsPath, fileName);
        dataPersistencesObjects = FindAllDataPersistenceObjects();
        LoadSession();
    }
    public void NewSession()
    {
        this.dataPersistencesObjects = FindAllDataPersistenceObjects();
        this.gameData = new GameData();
    }

    public void LoadSession()
    {
        // To do 
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data found");
            NewSession();
        }

        foreach(IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistencesObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistencesObjects);

    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
