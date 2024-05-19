using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapInfo", menuName = "MapInfo")]
public class MapInfo : ScriptableObject
{
    public List<MapNodeType> allNodes;
    public List<int> nodeRows;
}
