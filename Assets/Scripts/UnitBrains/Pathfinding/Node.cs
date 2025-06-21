using System;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class Node
    {
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
            Parent = null;
        }
        public void CalculateEstimate(int targetX, int targetY)
        {
            Estimate = Math.Abs(X - targetX) + Math.Abs(Y - targetY);
        }
        public void CalculateValue()
        {
            Value = Cost + Estimate;
        }
        public override bool Equals(object obj)
        {
            if (obj is not Node node)
                return false;

            return X == node.X && Y == node.Y;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}