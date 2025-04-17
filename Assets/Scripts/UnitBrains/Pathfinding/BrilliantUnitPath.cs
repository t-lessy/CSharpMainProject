using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;

public class BrilliantUnitPath : BaseUnitPath
{
    private int[] dx = { -1, 0, 1, 0 };
    private int[] dy = { 0, 1, 0, -1 };
    

    private int DebugCounter = 0;
    private int MaxDebugCounter = 3000;

    public BrilliantUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
    {

    }



    protected override void Calculate()
    {
        List<Vector2Int> result = startPoint == endPoint || findPath() == null? new List<Vector2Int> {startPoint} : findPath();

        //if (pathNodes == null)
        //    return;

        path = result.ToArray();

    }


    public List<Vector2Int> findPath()  //И тут был лист нод, а не vector2int
    {
        PathNode startNode = new PathNode(startPoint);
        PathNode targetNode = new PathNode(endPoint);

        var diff = endPoint - startPoint;
        var stepDiff = diff.SignOrZero();
        Vector2Int[] allStepDiff = { stepDiff, new Vector2Int(0, stepDiff.y), new Vector2Int(stepDiff.x, 0), new Vector2Int(stepDiff.x, -stepDiff.y), new Vector2Int(-stepDiff.x, stepDiff.y) };

        List<PathNode> openList = new List<PathNode> { startNode };
        List<PathNode> closedList = new List<PathNode>();

        while (openList.Count > 0)
        {
            DebugCounter++;                            //Дебажная херня
            if (DebugCounter >= MaxDebugCounter)      //
            {                                        //
                Debug.Log("БЕСКОНЕЧНЫЙ ЦИКЛ");      //
                Debug.Log($"openList.Count = {openList.Count}");      //
                Debug.Log($"closedList.Count = {closedList.Count}");
                return null;                       //
            }                                     //

            


            PathNode currentNode = openList[0];

            foreach (PathNode node in openList)
            {
                if (node.Value < currentNode.Value)
                    currentNode = node;  //Это точно работает
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            
            //if (currentNode.Equals(targetNode)) 
            if (currentNode.Position == targetNode.Position)
            {
                List<Vector2Int> path = new List<Vector2Int>(); //Раньше тут был лист нод
                Debug.Log("Главный ИФ РАБОТАЕТ!!!!!"); //Убрать
                while (currentNode != null)
                {
                    Vector2Int currentPosition = currentNode.Position;
                    path.Add(currentPosition); //И тут добавлял текущую ноду, а не позицию //Было currentNode.Position
                    currentNode = currentNode.Parent;        //
                }

                path.Reverse();
                return path;
            }

            for (int i = 0; i < allStepDiff.Length; i++) // раньше тут длинна dx была
            {

                PathNode newPosition = new(currentNode.Position + allStepDiff[i]);   //тут был Vector2Int

                if (runtimeModel.IsTileWalkable(newPosition.Position))  //Пока тут бесконечный цикл из-за этой херни. Разобраться /// newPosition было
                {
                    Debug.Log("ИсТайлВалкабле работает");
                    PathNode neighbor = new PathNode(newPosition.Position);   // newPosition было

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
}
