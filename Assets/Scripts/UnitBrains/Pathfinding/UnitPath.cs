using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace UnitBrains.Pathfinding
{
    public class UnitPath : BaseUnitPath
    {
        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };

        public UnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
            Debug.Log("UnitPath.UnitPath");
        }
        protected override void Calculate()
        {
            Debug.Log("UnitPath.Calculate");
            path = FindPath().ToArray();
            Debug.Log(path);

            if (path == null)
            {
                return;
            }
        }

        public List<Vector2Int> FindPath()
        {
            Node startNode = new Node(startPoint.x, startPoint.y);
            Node targetNode = new Node(endPoint.x, endPoint.y);

            List<Node> openList = new List<Node> { startNode };
            List<Node> closedList = new List<Node>();

            while (openList.Count > 0)
            {
                Node currentNode = openList[0];

                foreach (var node in openList)
                {
                    if (node.Value < currentNode.Value)
                        currentNode = node;
                }
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y)
                {
                    List<Vector2Int> path = new List<Vector2Int> ();
                    while (currentNode != null)
                    {
                        Vector2Int curNode = new Vector2Int(currentNode.X, currentNode.Y);
                        path.Add(curNode);
                        currentNode = currentNode.Parent;
                    }
                    path.Reverse();
                    return path;
                }
                for (int i = 0; i < dx.Length; i++)
                {
                    int newX = currentNode.X + dx[i];
                    int newY = currentNode.Y + dy[i];

                    if (IsValid(newX, newY))
                    {
                        Node neighbor = new Node(newX, newY);

                        if (closedList.Contains(neighbor))
                            continue;

                        neighbor.Parent = currentNode;
                        neighbor.CalculateEstimate(targetNode.X, targetNode.Y);
                        neighbor.CalculateValue();

                        openList.Add(neighbor);
                    }
                }
            }
            return null;
        }

        private bool IsValid(int x, int y)
        {
            bool containsX = x >= 0 && x < runtimeModel.RoMap.Width;
            bool containsY = y >= 0 && y < runtimeModel.RoMap.Height;
            Vector2Int pos = new Vector2Int(x, y);
            bool isNotWall = runtimeModel.IsTileWalkable(pos);
            return containsX && containsY && isNotWall;
        }
    }
}