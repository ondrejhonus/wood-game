using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq; // Needed for list filtering

public class SaveManager : MonoBehaviour
{
    [Header("References")]
    public Transform truckTransform;
    public SaveZone saveZone;
    public GameObject logPrefab;
    
    // player stats
    public PlayerStats playerStats;
    // public PlayerInventory playerInventory; 

    private string saveFilePath;

    private void Awake()
    {
        // define save file path
        saveFilePath = Path.Combine(Application.persistentDataPath, "game_save.json");
    }

    public void SaveGame()
    {
        GameData data = new GameData();

        // stave player stats
        if(playerStats != null)
        {
            data.savedMoney = playerStats.GetMoney();
            data.savedHP = playerStats.GetHealth();
        }

        // svave truck position
        data.truckX = truckTransform.position.x;
        data.truckY = truckTransform.position.y;
        data.truckZ = truckTransform.position.z;
        data.truckRotX = truckTransform.eulerAngles.x;
        data.truckRotY = truckTransform.eulerAngles.y;
        data.truckRotZ = truckTransform.eulerAngles.z;

        // save logs in save zone
        // get box collider of save zone
        BoxCollider zoneBox = saveZone.GetComponent<BoxCollider>();
        
        // get world position, half extents and rotation of the box, this is because OverlapBox needs world coords, not local position
        Vector3 center = saveZone.transform.TransformPoint(zoneBox.center);
        Vector3 halfExtents = Vector3.Scale(zoneBox.size, saveZone.transform.lossyScale) * 0.5f;
        Quaternion rotation = saveZone.transform.rotation;

        // find all colliders in the box
        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        foreach (Collider hit in hits)
        {
            // check if colision was a log or not
            SaveableLog logScript = hit.GetComponent<SaveableLog>();
            
            // if its a log, save it
            if (logScript != null)
            {
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

        // load truck position, teleport it there
        CharacterController cc = truckTransform.GetComponent<CharacterController>();
        if(cc) cc.enabled = false;

        truckTransform.position = new Vector3(data.truckX, data.truckY, data.truckZ);
        truckTransform.eulerAngles = new Vector3(data.truckRotX, data.truckRotY, data.truckRotZ);

        if(cc) cc.enabled = true;

        // remove existing logs in the save zone, maybe delete this later, idk if its neccesary
        BoxCollider zoneBox = saveZone.GetComponent<BoxCollider>();
        Vector3 center = saveZone.transform.TransformPoint(zoneBox.center);
        Vector3 halfExtents = Vector3.Scale(zoneBox.size, saveZone.transform.lossyScale) * 0.5f;
        
        Collider[] hits = Physics.OverlapBox(center, halfExtents, saveZone.transform.rotation);
        
        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<SaveableLog>())
            {
                Destroy(hit.gameObject);
            }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5)) SaveGame();
        if (Input.GetKeyDown(KeyCode.F9)) LoadGame();
    }
}