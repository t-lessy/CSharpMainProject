using Codice.CM.Common.Merge;
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitBrains.Pathfinding;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            Node startNode = new(startPoint.x, startPoint.y);
            Node targetNode = new(EndPoint.x, endPoint.y);

            List<Node> openList = new() { startNode };

            List<Node> closedList = new();

            List<Vector2Int> pathList = new() { };

            bool pathIsBuilt = false;

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

                if (Math.Abs(currentNode.X - targetNode.X) <= 1 && Math.Abs(currentNode.Y - targetNode.Y) <= 1)
                {
                    while (currentNode != null)
                    {
                        pathList.Add(new Vector2Int(currentNode.X, currentNode.Y));
                        currentNode = currentNode.Parent;
                    }
                    pathList.Reverse();
                    pathIsBuilt = true;
                }

                if (!pathIsBuilt)
                {
                    for (int i = 0; i < dx.Length; i++)
                    {
                        int newX = currentNode.X + dx[i];
                        int newY = currentNode.Y + dy[i];

                        if (IsValid(newX, newY))
                        {
                            Node neighbor = new(newX, newY);

                            if (closedList.Contains(neighbor))
                                continue;

                            neighbor.Parent = currentNode;
                            neighbor.CalculateEstimate(targetNode.X, targetNode.Y);
                            neighbor.CalculateValue();

                            openList.Add(neighbor);
                        }
                    }
                }
            }
            path = pathList.ToArray();
        }
        private int[] dx = { -1, 0, 1, 0 };
        private int[] dy = { 0, 1, 0, -1 };
        private bool IsValid(int x, int y)
        {
            return runtimeModel.IsTileWalkable(new Vector2Int(x, y));
        }
    }
}