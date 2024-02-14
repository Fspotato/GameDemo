using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class MapConfig : ScriptableObject
{
    public List<NodeBlueprint> nodeBlueprints;

    public IntMinMax numOfPreBossNodes;
    public IntMinMax numOfStartingNodes;

    public int GridWidth => Mathf.Max(numOfStartingNodes.max, numOfPreBossNodes.max);

    public List<MapLayer> layers;
}
