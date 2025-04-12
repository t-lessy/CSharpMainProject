using Model;
using System.Collections;
using System.Collections.Generic;
using UnitBrains.Pathfinding;
using UnityEngine;

public class BrilliantUnitPath : BaseUnitPath
{
    private int[] dx = { -1, 0, 1, 0 };
    private int[] dy = { 0, 1, 0, -1 };

    public BrilliantUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
    {

    }



    protected override void Calculate()
    {
        List<PathNode> pathNodes = findPath();
        if (pathNodes == null)
            return;

        var currentPoint = startPoint;
        var result = new List<Vector2Int>();
        for (int i = 0; i < pathNodes.Count; i++)
        {
            PathNode nextPosition = pathNodes[i];
            Vector2Int nextStep = nextPosition.Position;
            if (nextStep.x == currentPoint.x && nextStep.y == currentPoint.y)
                continue;

            result.Add(nextStep);
        }

        path = result.ToArray();

    }


    public List<PathNode> findPath()
    {
        PathNode startNode = new PathNode(startPoint);
        PathNode targetNode = new PathNode(endPoint);

        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            foreach (var node in openList)
            {
                if (node.Value < currentNode.Value)
                    currentNode = node;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            
            if (currentNode.Position.x == targetNode.Position.x && currentNode.Position.y == targetNode.Position.y)
            {
                List<PathNode> path = new List<PathNode>();

                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.Parent;
                }

                path.Reverse();
                return path;
            }

            for (int i = 0; i < dx.Length; i++) // раньше тут x и y объявлялись и использовались дальше. Сейчас это не так. Возможно всё умирает из-за прибавки dy и  dx
            {
                Vector2Int newPosition = new Vector2Int(currentNode.Position.x + dx[i], currentNode.Position.y + dy[i]);

                if (!runtimeModel.IsTileWalkable(newPosition))
                {
                    PathNode neighbor = new PathNode(newPosition);

                    if (closedList.Contains(neighbor))
                        continue;

                    neighbor.Parent = currentNode;
                    neighbor.CalculateEstimate(targetNode.Position.x, targetNode.Position.y);
                    neighbor.CalculateValue();

                    openList.Add(neighbor);
                }
            }
        }
        return null;
    }
}
