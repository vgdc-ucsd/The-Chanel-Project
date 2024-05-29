using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapInfo", menuName = "MapInfo")]
public class MapInfo : ScriptableObject
{
    public List<MapNodeType> nodeTypes;
    public List<Point> nodePoints;
    public List<ConnectionsList> nodeConnections;
    public List<bool> nodeVisited;
    public Point lastVisitedNode;
}
