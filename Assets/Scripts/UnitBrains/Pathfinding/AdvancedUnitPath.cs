using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Pathfinding;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;
using static UnityEngine.GraphicsBuffer;

public class AdvancedUnitPath : BaseUnitPath
{
    IReadOnlyRuntimeModel _runtimeModel;
    Vector2Int _startPoint;
    Vector2Int _endPoint;

    public AdvancedUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
    {
        _runtimeModel = runtimeModel;
        _startPoint = startPoint;
        _endPoint = endPoint;
    }

    

    protected override void Calculate()
    {
        List<Node> nodePath = FindPath();
        if (nodePath == null || nodePath.Count == 0)
        {
            path = new[] { startPoint };
            return;
        }


        //var currentPoint = _startPoint;
        //var result = new List<Vector2Int> { _startPoint };

        //var counter = 0;
        //while (currentPoint != _endPoint && counter++ < 200)
        //{
        //    var nextStep = GetNextStepFrom(currentPoint);
        //    var hasLoop = result.Contains(nextStep);
        //    result.Add(nextStep);
        //    if (hasLoop)
        //        break;
        //    currentPoint = nextStep;
        //}

        path = nodePath
                .Select(n => n.Pos)
                .ToArray();
    }

    public List<Node> FindPath()
    {
        // 1. Используем актуальные startPoint и endPoint из базового класса
        Node startNode = new Node(startPoint.x, startPoint.y);
        Node targetNode = new Node(endPoint.x, endPoint.y);

        // 2. Инициализируем стартовый узел
        startNode.Cost = 0; // Путь до самого себя равен 0[cite: 3]
        startNode.CalculateEstimate(targetNode.X, targetNode.Y);
        startNode.CalculateValue();

        List<Node> openList = new List<Node> { startNode };
        List<Node> closedList = new List<Node>();

        // Переменные для поиска "лучшего из возможного", если путь заблокирован
        Node bestNodeSoFar = startNode;
        float bestDistanceSoFar = float.MaxValue;

        int counter = 0;
        int maxIterations = 150; // Защита от бесконечных циклов

        while (openList.Count > 0 && counter < maxIterations)
        {
            counter++;

            // Ищем узел с минимальным f-value (Value)
            Node currentNode = openList[0];
            foreach (var node in openList)
            {
                if (node.Value < currentNode.Value)
                    currentNode = node;
            }

            // Обновляем лучший найденный узел по прямой дистанции до цели[cite: 4]
            float distToTarget = Vector2Int.Distance(currentNode.Pos, endPoint);
            if (distToTarget < bestDistanceSoFar)
            {
                bestDistanceSoFar = distToTarget;
                bestNodeSoFar = currentNode;
            }

            // Если дошли до цели — возвращаем путь[cite: 1, 4]
            if (currentNode.X == targetNode.X && currentNode.Y == targetNode.Y)
            {
                return BuildPathList(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, 1, 0, -1 };

            for (int i = 0; i < dx.Length; i++)
            {
                int newX = currentNode.X + dx[i];
                int newY = currentNode.Y + dy[i];

                // Проверяем, не обрабатывали ли мы уже эту клетку[cite: 4]
                if (closedList.Any(n => n.X == newX && n.Y == newY))
                    continue;

                Node neighbor = new Node(newX, newY);

                // Проверяем проходимость клетки[cite: 1, 4]
                if (IsValid(neighbor, endPoint))
                {
                    int newCost = currentNode.Cost + 1; // Шаг стоит 1[cite: 4]

                    // Ищем, есть ли этот сосед уже в списке на проверку[cite: 1, 4]
                    var existing = openList.FirstOrDefault(n => n.X == newX && n.Y == newY);

                    if (existing != null)
                    {
                        if (newCost < existing.Cost)
                        {
                            existing.Cost = newCost;
                            existing.Parent = currentNode;
                            existing.CalculateValue();
                        }
                    }
                    else
                    {
                        neighbor.Cost = newCost;
                        neighbor.Parent = currentNode;
                        neighbor.CalculateEstimate(targetNode.X, targetNode.Y);
                        neighbor.CalculateValue();
                        openList.Add(neighbor);
                    }
                }
            }
        }

        // Если путь не найден или кончились итерации, идем к ближайшей возможной точке[cite: 4]
        return BuildPathList(bestNodeSoFar);
    }

    private List<Node> BuildPathList(Node node)
    {
        List<Node> result = new List<Node>();
        while (node != null)
        {
            result.Add(node);
            node = node.Parent;
        }
        result.Reverse();
        return result;
    }
    private bool IsValid(Node node, Vector2Int targetPos)
    {
        if(node.X < 0 || node.X >= _runtimeModel.RoMap.Width ||
        node.Y < 0 || node.Y >= _runtimeModel.RoMap.Height)
        return false;

        if (node.X == targetPos.x && node.Y == targetPos.y)
            return true;

        return _runtimeModel.IsTileWalkable(node.Pos);
    }
}