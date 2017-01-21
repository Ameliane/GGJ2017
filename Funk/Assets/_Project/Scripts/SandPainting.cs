using UnityEngine;
using System.Collections;

public class SandPainting : MonoBehaviour
{

    public enum SandColor
    {
        BEIGE,
        AQUA,
        PURPLE,
        SIZE
    };

    public Terrain m_Terrain;
    public SandColor m_SandColor;
    public int m_Radius = 40;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            m_SandColor++;

        if (m_SandColor >= SandColor.SIZE)
            m_SandColor = 0;

        SetTerrainColor();
        
    }

    private void SetTerrainColor()
    {
        // get the normalized position of this game object relative to the terrain
        Vector3 relativePos = (transform.position - m_Terrain.gameObject.transform.position);
        Vector3 pos;
        pos.x = relativePos.x / m_Terrain.terrainData.size.x;
        pos.y = relativePos.y / m_Terrain.terrainData.size.y;
        pos.z = relativePos.z / m_Terrain.terrainData.size.z;

        // we set an offset so that all the sampled terrain is under this game object
        int size = m_Radius * 2;
        int offset = size / 2;

        int posXInTerrain = (int)(pos.x * m_Terrain.terrainData.alphamapWidth);
        int posYInTerrain = (int)(pos.z * m_Terrain.terrainData.alphamapHeight);

        // TODO: Deal with extremities

        float[,,] splatMap = m_Terrain.terrainData.GetAlphamaps(posXInTerrain - offset, posYInTerrain - offset, size, size);

        // we set each sample of the terrain in the size
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                // If distance between player and place on the splatMap is less than the specified Radius
                float dist = Vector2.Distance(new Vector2(pos.x, pos.y), new Vector2(i, j));

                if (dist < m_Radius)
                {
                    // Reset SplatMap
                    for (int c = 0; c < (int)SandColor.SIZE; c++)
                    {
                        splatMap[i, j, c] = 0;
                    }

                    // Set the correct SplatMap
                    splatMap[i, j, (int)m_SandColor] = 1;
                }
            }
        }

        m_Terrain.terrainData.SetAlphamaps(posXInTerrain - offset, posYInTerrain - offset, splatMap);
    }
}
