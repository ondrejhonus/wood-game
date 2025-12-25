using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // player stats
    public int savedMoney;
    public float savedHP;

    public GameObject[] InventoryItems;

    // truck data
    public float truckX, truckY, truckZ;
    public float truckRotX, truckRotY, truckRotZ;

    // player pos data
    public float playerX, playerY, playerZ;
    public float playerRotX, playerRotY, playerRotZ;

    // logs
    public List<LogData> allLogs = new List<LogData>();
}

[System.Serializable]
public class LogData
{
    public float x, y, z;       // pos
    public float rx, ry, rz;    // rotation
    public float sx, sy, sz;    // scale
}