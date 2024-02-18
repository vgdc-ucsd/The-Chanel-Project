using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default MapConfig", menuName = "MapConfig")]
public class MapConfig : ScriptableObject
{
    public List<MapLayerOptions> layerOptions;
}

[Serializable]
public class MapLayerOptions
{
    public int numOfNodes;
    public MapNodeType mapNodeType;
}