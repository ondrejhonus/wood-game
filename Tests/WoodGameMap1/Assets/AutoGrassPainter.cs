using UnityEngine;

[ExecuteInEditMode]
public class SlopeAutoGrassPainter : MonoBehaviour
{
    [Header("Terrain Settings")]
    public Terrain terrain;

    [Tooltip("Index of the detail prototype that should be used for grass.")]
    public int detailPrototypeIndex = 0;

    [Range(0f, 1f)]
    public float grassDensity = 0.7f;

    [Tooltip("Maximum slope (degrees) where grass can appear. Steeper areas will have no grass.")]
    public float maxSlope = 30f;

    [ContextMenu("Apply Grass Automatically")]
    public void ApplyGrass(){
        if (!terrain) terrain = Terrain.activeTerrain;
        if (!terrain)
        {
            Debug.LogError("❌ No terrain assigned!");
            return;
        }

        TerrainData data = terrain.terrainData;

        int resDetail = data.detailResolution;
        int[,] detailMap = new int[resDetail, resDetail];

        int grassPlaced = 0;

        for (int y = 0; y < resDetail; y++)
        {
            for (int x = 0; x < resDetail; x++)
            {
                // Calculate normalized terrain coordinates
                float normX = (float)x / resDetail;
                float normY = (float)y / resDetail;

                // Convert to world position
                float worldX = normX * data.size.x;
                float worldZ = normY * data.size.z;

                // Sample slope at this point
                float slope = data.GetSteepness(normX, normY);

                // Place grass if slope is below maxSlope and passes random density check
                if (slope <= maxSlope && Random.value < grassDensity)
                {
                    detailMap[y, x] = 1;
                    grassPlaced++;
                }
