using UnityEngine;
using System.Collections;

public class SandPainting : MonoBehaviour
{
    public enum SandColor
    {
        DEFAULT,
        BOUNCE,
        FAST,
        SIZE
    };

    Terrain m_Terrain;

    [SerializeField]
    SandColor m_SandColor;

    [SerializeField]
    float m_Radius = 2;

    //float[,,] m_OriginalMap;

    public static SandPainting Instance = null;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        //m_OriginalMap = m_Terrain.terrainData.GetAlphamaps(0, 0, m_Terrain.terrainData.alphamapWidth, m_Terrain.terrainData.alphamapHeight);
    }

    void Update()
    {

    }

    private void GetTerrain(Vector3 aPos)
    {
        RaycastHit hit;

        if (Physics.Raycast(aPos, -Vector3.up, out hit, 2.0f))
        {
            if (hit.collider.gameObject.GetComponent<Terrain>())
                m_Terrain = hit.collider.gameObject.GetComponent<Terrain>();
            return;
        }

        m_Terrain = null;
    }

    public void SetTerrainColor(Vector3 aPos, SandColor aColor)
    {
        GetTerrain(aPos);

        if (m_Terrain == null)
            return;

        // get the normalized position of this game object relative to the terrain
        Vector3 relativePos = (aPos - m_Terrain.gameObject.transform.position);
        Vector3 pos;
        pos.x = relativePos.x / m_Terrain.terrainData.size.x;
        pos.y = relativePos.y / m_Terrain.terrainData.size.y;
        pos.z = relativePos.z / m_Terrain.terrainData.size.z;

        // If outside of map, don't paint
        if (relativePos.x < 0 || relativePos.z < 0)
        {
            Debug.Log("Out of Bounds");
            return;
        }
        if (relativePos.x > m_Terrain.terrainData.size.x || relativePos.z > m_Terrain.terrainData.size.z)
        {
            Debug.Log("Out of Bounds");
            return;
        }

        // we set an offset so that all the sampled terrain is under this game object
        int size = Mathf.RoundToInt(m_Radius * 2);
        int offset = size / 2;

        int posXInTerrain = (int)(pos.x * m_Terrain.terrainData.alphamapWidth);
        int posYInTerrain = (int)(pos.z * m_Terrain.terrainData.alphamapHeight);

        //Deal with extremities
        int x = posXInTerrain - offset;
        x = x < 0 ? 0 : x;
        int y = posYInTerrain - offset;
        y = y < 0 ? 0 : y;

        int width = x + size > m_Terrain.terrainData.alphamapWidth ? m_Terrain.terrainData.alphamapWidth - x - 1 : size;
        int height = y + size > m_Terrain.terrainData.alphamapHeight ? m_Terrain.terrainData.alphamapHeight - y - 1 : size;

        float[,,] splatMap = m_Terrain.terrainData.GetAlphamaps(x, y, width, height);

        // we set each sample of the terrain in the size
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // If distance between player and place on the splatMap is less than the specified Radius
                float dist = Vector2.Distance(new Vector2(size / 2, size / 2), new Vector2(i, j));

                if (dist < m_Radius)
                {
                    // Reset SplatMap
                    for (int c = 0; c < (int)SandColor.SIZE; c++)
                    {
                        splatMap[i, j, c] = 0;
                    }

                    // Set the correct SplatMap
                    splatMap[i, j, (int)aColor] = 1;
                }
            }
        }

        m_Terrain.terrainData.SetAlphamaps(x, y, splatMap);
    }

    public SandColor GetSandColor(Vector3 aPos)
    {
        GetTerrain(aPos);

        if (m_Terrain == null)
            return SandColor.SIZE;

        // get the normalized position of this game object relative to the terrain
        Vector3 relativePos = (aPos - m_Terrain.gameObject.transform.position);
        Vector3 pos;
        pos.x = relativePos.x / m_Terrain.terrainData.size.x;
        pos.y = relativePos.y / m_Terrain.terrainData.size.y;
        pos.z = relativePos.z / m_Terrain.terrainData.size.z;

        // If outside of map, don't paint
        if (relativePos.x < 0 || relativePos.z < 0)
        {
            Debug.Log("Out of Bounds");
            return SandColor.SIZE;
        }
        if (relativePos.x > m_Terrain.terrainData.size.x || relativePos.z > m_Terrain.terrainData.size.z)
        {
            Debug.Log("Out of Bounds");
            return SandColor.SIZE;
        }

        int posXInTerrain = (int)(pos.x * m_Terrain.terrainData.alphamapWidth);
        int posYInTerrain = (int)(pos.z * m_Terrain.terrainData.alphamapHeight);

        float[,,] splatMap = m_Terrain.terrainData.GetAlphamaps(posXInTerrain, posYInTerrain, 1, 1);

        for (int i = 0; i < (int)SandColor.SIZE; i++)
        {
            if (splatMap[0, 0, i] == 1)
            {
                return (SandColor)i;
            }
        }

        return SandColor.SIZE;
    }

    private void OnApplicationQuit()
    {
        //m_Terrain.terrainData.SetAlphamaps(0, 0, m_OriginalMap);

        Terrain[] terrains = Object.FindObjectsOfType<Terrain>();

        foreach(Terrain t in terrains)
        {
            float[,,] splatMap = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);

            for (int i = 0; i < t.terrainData.alphamapWidth; i++)
            {
                for (int j = 0; j < t.terrainData.alphamapHeight; j++)
                {
                    splatMap[i, j, 0] = 1;

                    // Reset SplatMap
                    for (int c = 1; c < (int)SandColor.SIZE; c++)
                    {
                        splatMap[i, j, c] = 0;
                    }
                }
            }

            t.terrainData.SetAlphamaps(0, 0, splatMap);
        }
    }
}
