using Codice.Client.BaseCommands.Merge;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnitBrains.Pathfinding;
using UnityEngine;




namespace UnitBrains.Pathfinding
{
    
    public class AStarUnitPath : BaseUnitPath
    {
        private class Node
        {
            public int X;
            public int Y;
            public int Cost;           
            public int Estimate;       
            public int Value;           
            public Node Parent;

            public Node(int x, int y)
            {
                X = x;
                Y = y;
                Cost = 0;
            }

            public void CalculateEstimate(int targetX, int targetY)
            {
                int distanceX = Math.Abs(X - targetX);
                int distanceY = Math.Abs(Y - targetY);
                Estimate = distanceX + distanceY;  
            }

            public void CalculateValue()
            {
                Value = Cost + Estimate;  
            }

            public Vector2Int ToVector() => new Vector2Int(X, Y);
        }

        private readonly int[] dx = { -1, 0, 1, 0 };
        private readonly int[] dy = { 0, 1, 0, -1 };

        private Vector2Int _currentPosition;

        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint)
            : base(runtimeModel, startPoint, endPoint)
        {
            _currentPosition = startPoint;
        }

        public override Vector2Int GetNextStepFrom(Vector2Int currentPos)
        {
            if (_currentPosition != currentPos)
            {
                _currentPosition = currentPos;
                path = null;
            }

            if (path == null || path.Length == 0)
            {
                Calculate();
            }

            if (path == null || path.Length == 0)
            {
                return _currentPosition;
            }

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == _currentPosition)
                {
                    if (i + 1 < path.Length)
                    {
                        return path[i + 1];
                    }
                    break;
                }
            }
            return _currentPosition;
        }

        protected override void Calculate()
        {
            if (_currentPosition == endPoint)
            {
                path = new[] { _currentPosition };
                return;
            }

            Node startNode = new Node(_currentPosition.x, _currentPosition.y);
            Node targetNode = new Node(endPoint.x, endPoint.y);

            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            startNode.CalculateEstimate(targetNode.X, targetNode.Y);
            startNode.CalculateValue();
            openList.Add(startNode);


            while (openList.Count > 0)
            {
                
                Node currentNode = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].Value < currentNode.Value ||
                        (openList[i].Value == currentNode.Value && openList[i].Estimate < currentNode.Estimate))
                    {
                        currentNode = openList[i];
                    }
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);


                
                if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y)
                {
                    ReconstructPath(currentNode);
                    return;
                }

                
                for (int i = 0; i < 4; i++)
                {
                    int newX = currentNode.X + dx[i];
                    int newY = currentNode.Y + dy[i];


                    if (!IsValid(newX, newY))
                    {
                        continue;
                    }

                    Node neighbor = new Node(newX, newY);
                    neighbor.Cost = currentNode.Cost + 10;  

                    neighbor.Parent = currentNode;

                    
                    bool inClosedList = closedList.Any(n => n.X == newX && n.Y == newY);
                    if (inClosedList)
                    {
                        continue;
                    }

                    
                    bool inOpenList = false;
                    for (int j = 0; j < openList.Count; j++)
                    {
                        Node openNode = openList[j];
                        if (openNode.X == newX && openNode.Y == newY)
                        {
                            inOpenList = true;
                            
                            if (neighbor.Value < openNode.Value)
                            {
                                openNode.Parent = neighbor.Parent;
                                openNode.Cost = neighbor.Cost;
                                openNode.CalculateEstimate(targetNode.X, targetNode.Y);
                                openNode.CalculateValue();
                            }
                            break;
                        }
                    }

                    
                    if (!inOpenList)
                    {
                        neighbor.CalculateEstimate(targetNode.X, targetNode.Y);
                        neighbor.CalculateValue();
                        openList.Add(neighbor);
                    }
                }
            }

            path = new Vector2Int[0];  

        }

        private void ReconstructPath(Node endNode)
        {
            List<Vector2Int> pathList = new List<Vector2Int>();
            Node current = endNode;
            while (current != null)
            {
                pathList.Add(new Vector2Int(current.X, current.Y));
                current = current.Parent;
            }
            pathList.Reverse();
            path = pathList.ToArray();
        }

        private bool IsValid(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);

            
            if (x < 0 || x >= runtimeModel.RoMap.Width ||
                y < 0 || y >= runtimeModel.RoMap.Height)
            {
                return false;
            }

            
            if (runtimeModel.RoMap[pos])
            {
                return false;
            }

            
            foreach (var unit in runtimeModel.RoUnits)
            {
                if (unit.Pos == pos)
                {
                    return false;
                }
            }

            return true;
            
        }
    }

}
