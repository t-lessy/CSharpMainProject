using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;
using Utilities;

namespace UnitBrains.Pathfinding
{
    public class DummyUnitPath : BaseUnitPath
    {
        private const int MaxLength = 100;

        private class Node
        {
            public Vector2Int Position;
            public float GCost;
            public float HCost;
            public float FCost => GCost + HCost;
            public Node Parent;

            public Node(Vector2Int position)
            {
                Position = position;
            }
        }

        public DummyUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            Debug.Log($"Calculate: start={startPoint}, end={endPoint}");

            // Пытаемся найти путь через A*
            var pathList = FindAStarPath(startPoint, endPoint);

            if (pathList != null && pathList.Count > 1)
            {
                path = pathList.ToArray();
                Debug.Log($"A* SUCCESS! Path length: {path.Length}");
                return;
            }

            // Если A* не сработал - используем гарантированный рабочий fallback
            Debug.Log($"A* failed, using working fallback");

            var currentPoint = startPoint;
            var result = new List<Vector2Int> { startPoint };
            var counter = 0;
            var maxSteps = 200;

            while (currentPoint != endPoint && counter++ < maxSteps)
            {
                var nextStep = CalcNextStepTowards(currentPoint, endPoint);
                var hasLoop = result.Contains(nextStep);
                result.Add(nextStep);
                if (hasLoop)
                    break;
                currentPoint = nextStep;
            }

            path = result.ToArray();
            Debug.Log($"Fallback path created, length = {path.Length}");
        }

        private List<Vector2Int> FindAStarPath(Vector2Int start, Vector2Int target)
        {
            // Проверка проходимости
            if (!runtimeModel.IsTileWalkable(start))
            {
                Debug.Log($"Start {start} not walkable");
                return null;
            }

            if (!runtimeModel.IsTileWalkable(target))
            {
                Debug.Log($"Target {target} not walkable");
                return null;
            }

            var openSet = new List<Node>();
            var closedSet = new HashSet<Vector2Int>();

            var startNode = new Node(start);
            startNode.GCost = 0;
            startNode.HCost = Heuristic(start, target);
            openSet.Add(startNode);

            var iterations = 0;
            var maxIterations = 10000;

            while (openSet.Count > 0 && iterations < maxIterations)
            {
                iterations++;

                var current = openSet.OrderBy(n => n.FCost).First();

                if (current.Position == target)
                {
                    Debug.Log($"A* found path in {iterations} iterations");
                    return ReconstructPath(current);
                }

                openSet.Remove(current);
                closedSet.Add(current.Position);

                var neighbors = GetNeighbors(current.Position);

                foreach (var neighborPos in neighbors)
                {
                    if (!runtimeModel.IsTileWalkable(neighborPos))
                        continue;

                    if (closedSet.Contains(neighborPos))
                        continue;

                    var tentativeGCost = current.GCost + 1;
                    var neighborNode = openSet.FirstOrDefault(n => n.Position == neighborPos);

                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighborPos);
                        neighborNode.GCost = tentativeGCost;
                        neighborNode.HCost = Heuristic(neighborPos, target);
                        neighborNode.Parent = current;
                        openSet.Add(neighborNode);
                    }
                    else if (tentativeGCost < neighborNode.GCost)
                    {
                        neighborNode.GCost = tentativeGCost;
                        neighborNode.Parent = current;
                    }
                }
            }

            Debug.Log($"A* failed after {iterations} iterations");
            return null;
        }

        private float Heuristic(Vector2Int a, Vector2Int b)
        {
            // Манхэттенское расстояние
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        private List<Vector2Int> GetNeighbors(Vector2Int pos)
        {
            // 4 направления для простоты (работает везде)
            return new List<Vector2Int>
            {
                pos + Vector2Int.up,
                pos + Vector2Int.down,
                pos + Vector2Int.left,
                pos + Vector2Int.right
            };
        }

        private List<Vector2Int> ReconstructPath(Node endNode)
        {
            var pathList = new List<Vector2Int>();
            var current = endNode;

            while (current != null)
            {
                pathList.Add(current.Position);
                current = current.Parent;
            }

            pathList.Reverse();
            return pathList;
        }

        // Гарантированно работающий метод движения (старый добрый)
        private Vector2Int CalcNextStepTowards(Vector2Int fromPos, Vector2Int toPos)
        {
            var diff = toPos - fromPos;
            var stepDiff = diff.SignOrZero();
            var nextStep = fromPos + stepDiff;

            if (runtimeModel.IsTileWalkable(nextStep))
                return nextStep;

            if (stepDiff.sqrMagnitude > 1)
            {
                var partStep0 = fromPos + new Vector2Int(stepDiff.x, 0);
                if (runtimeModel.IsTileWalkable(partStep0))
                    return partStep0;

                var partStep1 = fromPos + new Vector2Int(0, stepDiff.y);
                if (runtimeModel.IsTileWalkable(partStep1))
                    return partStep1;
            }

            var sideStep0 = fromPos + new Vector2Int(stepDiff.y, -stepDiff.x);
            var shiftedStep0 = fromPos + (sideStep0 + stepDiff).SignOrZero();
            if (runtimeModel.IsTileWalkable(shiftedStep0))
                return shiftedStep0;

            var sideStep1 = fromPos + new Vector2Int(-stepDiff.y, stepDiff.x);
            var shiftedStep1 = fromPos + (sideStep1 + stepDiff).SignOrZero();
            if (runtimeModel.IsTileWalkable(shiftedStep1))
                return shiftedStep1;

            if (runtimeModel.IsTileWalkable(sideStep0))
                return sideStep0;

            if (runtimeModel.IsTileWalkable(sideStep1))
                return sideStep1;

            return fromPos;
        }
    }
}