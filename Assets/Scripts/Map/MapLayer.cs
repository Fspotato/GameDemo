using UnityEngine;

[System.Serializable]
public class MapLayer
{
    public NodeType nodeType;
    public FloatMinMax distanceFromPreLayer;
    public float distanceFromApart;
    [Range(0f, 1f)] public float randomizePosition;
    [Range(0f, 1f)] public float randomizeNodes;
}
