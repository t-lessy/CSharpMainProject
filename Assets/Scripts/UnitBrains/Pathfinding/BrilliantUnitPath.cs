using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;
using static UnityEngine.GraphicsBuffer;

public class BrilliantUnitPath : BaseUnitPath
{
    private const int MaxLength = 500;
    private Vector2Int[] dxy = { new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1) };

    public BrilliantUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
    {
    }

    protected override void Calculate()
    {
        List<Vector2Int> result = FindPath();
        List<Vector2Int> resultNull = new List<Vector2Int> { startPoint, startPoint };
        

        path = result != null ? result.ToArray() : resultNull.ToArray();

    }


    private List<Vector2Int> FindPath()
    {
        PathNode startNode = new PathNode(startPoint);
        PathNode targetNode = new PathNode(endPoint);

        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            if (openList.Count >= MaxLength) //Защита от бесконечного цикла
            {
                //foreach (var node in openList)
                //{
                //    Debug.Log(node.Position);
                //}
                return null;
            }

            foreach (var node in openList) 
            {
                if (node.Value < currentNode.Value)
                    currentNode = node;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.Position == targetNode.Position)
            {
                List<Vector2Int> path = new List<Vector2Int>();

                while (currentNode != null)
                {
                    path.Add(currentNode.Position);
                    currentNode = currentNode.Parent;
                }

                path.Reverse();
                return path;
            }

            for (int i = 0; i < dxy.Length; i++)
            {
                Vector2Int newPos = currentNode.Position + dxy[i];

                if (runtimeModel.IsTileWalkable(newPos) || newPos == targetNode.Position)
                {
                    PathNode neighbor = new PathNode(newPos);
                    if (closedList.Contains(neighbor))
                        continue;

                    neighbor.Parent = currentNode;
                    neighbor.CalculateEstimate(targetNode.Position);
                    neighbor.CalculateValue();

                    openList.Add(neighbor);
                }
            }
        }
        return null;
    }


    private List<Vector2Int> FallbackOption()   //Скопировано с DummyUnitPath. Срабатывает только в случае, если алгоритм A* не находит путь
    {
            var currentPoint = startPoint;
            var result = new List<Vector2Int> { startPoint };
            var counter = 0;
            while (currentPoint != endPoint && counter++ < MaxLength)
            {
                var nextStep = CalcNextStepTowards(currentPoint, endPoint);
                var hasLoop = result.Contains(nextStep);
                result.Add(nextStep);
                if (hasLoop)
                    break;
                currentPoint = nextStep;
            }

            return result;

            Vector2Int CalcNextStepTowards(Vector2Int fromPos, Vector2Int toPos)
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
