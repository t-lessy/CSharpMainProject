using System;
using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public class Node
    {
        public Vector2Int Pos;
        public int X;
        public int Y;
        public int Cost = 10;
        public int Estimate;
        public int Value;
        public Node Parent;

        public Node(int x, int y)
        {
            X = x;
            Y = y;
            Pos = new Vector2Int(x, y);
        }

        public void CalculateEstimate(int targetX, int targetY)
        {
            Estimate = Math.Abs(X - targetX) + Math.Abs(Y - targetY);
        }

        public void CalculateValue()
        {
            Value = Cost + Estimate;
        }
    }
}