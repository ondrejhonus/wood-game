using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System; // Needed for list filtering
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using System.Reflection.Emit;
using TMPro;

public class SaveManager : MonoBehaviour
{
    [Header("References")]
    public Transform truckTransform;
    public Transform playerTransform;
    public SaveZone saveZone;
    public GameObject logPrefab;

    // player stats
    public PlayerStats playerStats;
    public PlayerInventory playerInventory;
    public GameManager gameManager;

    private string saveFilePath;

    // Choppablelog thingies
    public AudioSource audioSource;
    public LayerMask chopLayer;
    public int hitsToChop = 6;

    public static bool loadAfterSceneLoad = false;

    public static SaveManager Instance;

    [Header("Loading UI")]
    public CanvasGroup loadingScreen;
    public float fadeSpeed = 5f;

    // button text
    public TextMeshProUGUI buttonText;

    private void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

        // define save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "game_save.json");
        Debug.Log($"Save file path: {saveFilePath}");
    }

    public void Start()
    {
        if (loadAfterSceneLoad)
        {
            loadAfterSceneLoad = false;
            // Start the sequence: Stay dark -> Load -> Fade out
            StartCoroutine(LoadSequence());
        }
        else
        {
            // If it's a new game, just fade the screen out immediately
            loadingScreen.gameObject.SetActive(true);
            StartCoroutine(FadeOutOverlay());
        }
    }

    public void SaveButton()
    {
        SaveGame();
        // Set button text to "Saved"
        buttonText.text = "Saved!";
        // After 2 seconds, revert text back to "Save Game"
        StartCoroutine(RevertSaveButtonText());
    }

    IEnumerator RevertSaveButtonText()
    {
        yield return new WaitForSeconds(2f);
        buttonText.text = "Save Game";
    }

    public void LoadButton()
    {
        StartCoroutine(LoadSequence());
        gameManager.TogglePauseMenu();
    }

    IEnumerator LoadSequence()
    {
        // 1. Ensure overlay is visible immediately
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.alpha = 1f;

        // 2. Wait a tiny bit for the engine to settle (.2s)
        yield return new WaitForSeconds(0.2f);

        // 3. Trigger your existing Load function
        LoadGame();

        // 4. Wait the remaining time to reach your .5s goal
        yield return new WaitForSeconds(.5f);

        // 5. Fade out
        yield return StartCoroutine(FadeOutOverlay());
    }

    IEnumerator FadeOutOverlay()
    {
        while (loadingScreen.alpha > 0)
        {
            loadingScreen.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        loadingScreen.gameObject.SetActive(false);
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        // stave player stats
        if (playerStats != null)
        {
            data.savedMoney = playerStats.GetMoney();
            data.savedHP = playerStats.GetHealth();
        }

        if (playerInventory != null)
        {
            playerInventory = playerInventory.GetComponent<PlayerInventory>();
            GameObject[] itemsToSave = playerInventory.GetItemsInInventory();
            data.InventoryItems = itemsToSave;
            Debug.Log($"Saved {itemsToSave.Length} inventory items.");
        }

        // svave truck position
        data.truckX = truckTransform.position.x;
        data.truckY = truckTransform.position.y;
        data.truckZ = truckTransform.position.z;
        data.truckRotX = truckTransform.eulerAngles.x;
        data.truckRotY = truckTransform.eulerAngles.y;
        data.truckRotZ = truckTransform.eulerAngles.z;

        // save player position
        data.playerX = playerTransform.position.x;
        data.playerY = playerTransform.position.y;
        data.playerZ = playerTransform.position.z;
        data.playerRotX = playerTransform.eulerAngles.x;
        data.playerRotY = playerTransform.eulerAngles.y;
        data.playerRotZ = playerTransform.eulerAngles.z;

        // save logs in save zone
        // get box collider of save zone
        BoxCollider zoneBox = saveZone.GetComponent<BoxCollider>();

        // get world position, half extents and rotation of the box, this is because OverlapBox needs world coords, not local position
        Vector3 center = saveZone.transform.TransformPoint(zoneBox.center);
        Vector3 halfExtents = Vector3.Scale(zoneBox.size, saveZone.transform.lossyScale) * 0.5f;
        Quaternion rotation = saveZone.transform.rotation;

        // find all colliders in the box
        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        // Save a list of saved logs, so we only save once (because the log has two types of colliders)
        List<SaveableLog> processedLogs = new List<SaveableLog>();

        foreach (Collider hit in hits)
        {
            // check if colision was a log or not
            SaveableLog logScript = hit.GetComponent<SaveableLog>();

            // if its a log, save it
            if (logScript != null && !processedLogs.Contains(logScript))
            {
                // mark as processed
                processedLogs.Add(logScript);

                // save log data
                LogData lData = new LogData();
                lData.x = logScript.transform.position.x;
                lData.y = logScript.transform.position.y;
                lData.z = logScript.transform.position.z;
                lData.rx = logScript.transform.eulerAngles.x;
                lData.ry = logScript.transform.eulerAngles.y;
                lData.rz = logScript.transform.eulerAngles.z;
                lData.sx = logScript.transform.localScale.x;
                lData.sy = logScript.transform.localScale.y;
                lData.sz = logScript.transform.localScale.z;

                data.allLogs.Add(lData);
            }
        }

        // write to file
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Game Saved! Saved {data.allLogs.Count} logs in the zone.");
    }

    public void LoadGame()
    {
        // assign all objects if they are null
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (truckTransform == null)
            truckTransform = GameObject.FindWithTag("Truck").transform;
        if (playerTransform == null)
            playerTransform = GameObject.FindWithTag("Player").transform;
        if (saveZone == null)
            saveZone = FindFirstObjectByType<SaveZone>();
        if (audioSource == null)
            audioSource = FindFirstObjectByType<AudioSource>();


        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        // read from file
        string json = File.ReadAllText(saveFilePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // loda player stats
        if (playerStats != null)
        {
            playerStats.SetMoney(data.savedMoney);
            playerStats.SetHealth((int)data.savedHP);
        }

        if (playerInventory != null)
        {
            playerInventory = playerInventory.GetComponent<PlayerInventory>();
            GameObject[] itemsToLoad = data.InventoryItems;
            playerInventory.LoadInventoryItems(itemsToLoad);
            Debug.Log($"Loaded {itemsToLoad.Length} inventory items.");
        }


        // load truck position, teleport it there
        CharacterController cc = truckTransform.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        truckTransform.position = new Vector3(data.truckX, data.truckY, data.truckZ);
        truckTransform.eulerAngles = new Vector3(data.truckRotX, data.truckRotY, data.truckRotZ);

        if (cc) cc.enabled = true;

        // disable player for a bit, so it can teleport without issues
        CharacterController pcc = playerTransform.GetComponent<CharacterController>();
        if (pcc) pcc.enabled = false;

        // load player position
        playerTransform.position = new Vector3(data.playerX, data.playerY, data.playerZ);
        playerTransform.eulerAngles = new Vector3(data.playerRotX, data.playerRotY, data.playerRotZ);

        if (pcc) pcc.enabled = true;

        // remove existing logs in the save zone, maybe delete this later, idk if its neccesary
        // BoxCollider zoneBox = saveZone.GetComponent<BoxCollider>();
        // Vector3 center = saveZone.transform.TransformPoint(zoneBox.center);
        // Vector3 halfExtents = Vector3.Scale(zoneBox.size, saveZone.transform.lossyScale) * 0.5f;

        // Collider[] hits = Physics.OverlapBox(center, halfExtents, saveZone.transform.rotation);

        // foreach (Collider hit in hits)
        // {
        //     if (hit.GetComponent<SaveableLog>())
        //     {
        //         Destroy(hit.gameObject);
        //     }
        // }
        // delete all logs in the scene that are not planted
        SaveableLog[] allLogsInScene = UnityEngine.Object.FindObjectsByType<SaveableLog>(FindObjectsSortMode.None);
        foreach (SaveableLog log in allLogsInScene)
        {
            // destroy all logs in the game, if they dont have .isPlanted true
            ChoppableLog choppableLog = log.GetComponent<ChoppableLog>();
            if (choppableLog && choppableLog.isPlanted == false) Destroy(log.gameObject);
        }

        // spawn logs to the save zone
        foreach (LogData lData in data.allLogs)
        {
            // set position, rotation and scale
            Vector3 pos = new Vector3(lData.x, lData.y, lData.z);
            Quaternion rot = Quaternion.Euler(lData.rx, lData.ry, lData.rz);

            // create log
            GameObject newLog = Instantiate(logPrefab, pos, rot);
            // set scale
            newLog.transform.localScale = new Vector3(lData.sx, lData.sy, lData.sz);

            // add choppablelog to it, and assign basic values
            ChoppableLog ch = newLog.GetComponent<ChoppableLog>();
            if (ch == null) ch = newLog.AddComponent<ChoppableLog>();
            ch.logPiecePrefab = logPrefab;
            ch.hitsToChop = hitsToChop;
            ch.playerInventory = playerInventory;
            ch.audioSource = audioSource;
            ch.chopLayer = chopLayer;

            // Rigidbody rb = newLog.GetComponent<Rigidbody>();
            // if(rb) 
            // {
            //     // Ensure velocity is zeroed out
            //     rb.linearVelocity = Vector3.zero;
            //     rb.angularVelocity = Vector3.zero;
            // }
        }

        Debug.Log("Game Loaded!");
    }
}