using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Connection
{
    public Point point1;
    public Point point2;
    public Connection(Point point1, Point point2)
    {
        this.point1 = point1;
        this.point2 = point2;
    }
}

// Wrapper Class to be able to ensure List<List<Connection>> is Serializable
[Serializable]
public class ConnectionsList
{
    public List<Connection> connections;
}