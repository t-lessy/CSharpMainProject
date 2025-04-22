using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathNode          // : MonoBehaviour
{
    public Vector2Int Position;
    public int Cost = 10;
    public int Estimate;
    public int Value;
    public PathNode Parent;

    public PathNode(Vector2Int position)
    {
        Position = position;
    }

    public void CalculateEstimate(Vector2Int target)
    {
        Estimate = Math.Abs(Position.x - target.x) + Math.Abs(Position.y - target.y);
    }
    public void CalculateValue()
    {
        Value = Cost + Estimate;
    }
    public override bool Equals(object? obj)
    {
        if (obj is not PathNode node)
            return false;

        return Position.x == node.Position.x && Position.y == node.Position.y;
    }
}
