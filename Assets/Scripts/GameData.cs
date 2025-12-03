using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    // player stats
    public int savedMoney;
    public float savedHP;

    // inventory
    // public List<string> InventoryItems = new List<InventoryItem>();

    // truck data
    public float truckX, truckY, truckZ;
    public float truckRotX, truckRotY, truckRotZ;

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