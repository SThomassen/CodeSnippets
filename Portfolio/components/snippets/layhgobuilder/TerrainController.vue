<template>
	<v-container class="line-numbers">
<pre><code class="language-js line-numbers" style="white-space: pre-wrap; background-color: transparent;">
/// Raise or lower the terrain height
private void RaiseTerrain(Vector3 a_point, int a_raise = 1)
{
    // Get position relative to terrain position within heightmap resolution.
    Vector3 pos = a_point - m_terrainPosition;
    int posX = (int)(((pos.x) / m_terrainData.size.x) * m_heightmapResolution);
    int posZ = (int)(((pos.z) / m_terrainData.size.z) * m_heightmapResolution);

    posX = Mathf.Clamp(posX - m_brushSize, 0, m_heightmapResolution - m_brushSize * 2 - 1);
    posZ = Mathf.Clamp(posZ - m_brushSize, 0, m_heightmapResolution - m_brushSize * 2 - 1);

    // Get the heightmap values in an area
    float[,] heights = m_terrainData.GetHeights(posX, posZ, m_brushSize * 2, m_brushSize * 2);

    // Loop through the brush size to change the terrain heightmap values
    int diameter = m_brushSize * m_brushSize;
    for (int i = -m_brushSize; i < m_brushSize; i++)
    {
        for (int j = -m_brushSize; j < m_brushSize; j++)
        {
            if (i * i + j * j >= diameter)
                continue;

            int startX = i + m_brushSize;
            int startZ = j + m_brushSize;

            float y = heights[startZ, startX];
            float value = (m_brushStrength / m_terrainData.size.y) * a_raise;

            y += value;
            y = Mathf.Clamp(y, 0, m_terrainData.size.y);

            heights[startZ, startX] = y;
        }
    }
    m_terrainData.SetHeights(posX, posZ, heights);
}

/// Paint the terrain with another texture
private void PaintBrushTerrain(Vector3 a_point)
{
    // Get position relative to terrain position within the alhamap resolution.
    Vector3 pos = a_point - m_terrainPosition;
    int posX = (int)((pos.x / m_terrainData.size.x) * m_terrainData.alphamapWidth);
    int posZ = (int)((pos.z / m_terrainData.size.z) * m_terrainData.alphamapHeight);

    posX = Mathf.Clamp(posX - m_brushSize, 0, m_heightmapResolution - m_brushSize * 2 - 1);
    posZ = Mathf.Clamp(posZ - m_brushSize, 0, m_heightmapResolution - m_brushSize * 2 - 1);

    // Get splatmap at position within brush range
    float[,,] splatMap = m_terrainData.GetAlphamaps(posX, posZ, m_brushSize * 2, m_brushSize * 2);

    // Loop through the brush size and change the slapmap values
    int diameter = m_brushSize * m_brushSize;
    for (int i = -m_brushSize; i < m_brushSize; i++)
    {
        for (int j = -m_brushSize; j < m_brushSize; j++)
        {
            if (i * i + j * j >= diameter)
                continue;

            int startX = i + m_brushSize;
            int startZ = j + m_brushSize;

            float[] splatWeights = new float[m_terrainData.alphamapLayers];

            for (int k = 0; k < splatWeights.Length; k++)
            {
                float s = splatMap[startX, startZ, k];
                if (Mathf.Approximately(s, 0))
                {
                    s = 0;
                }
                splatWeights[k] = s;
            }

            float splat = splatWeights[m_paletteLayerIndex];
            float value = Mathf.Clamp01(splat + m_brushStrength * 10);

            // if value is close to 0, make it 0
            if (Helper.Approx(value, 0, 0.05f))
            {
                value = 0;
            }

            splatWeights[m_paletteLayerIndex] = value;

            // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
            float sum = splatWeights.Sum();
            // Loop through each terrain texture
            for (int k = 0; k < m_terrainData.alphamapLayers; k++)
            {
                // Normalize so that sum of all texture weights = 1
                splatWeights[k] /= sum;

                // Assign this point to the splatmap array
                splatMap[startX, startZ, k] = splatWeights[k];
            }
        }
    }
    m_terrainData.SetAlphamaps(posX, posZ, splatMap);
}
</code></pre>
</v-container>
</template>

<script>
import Prism from 'prismjs'

export default {
    mounted() {
        Prism.highlightAll();
    }
}
</script>