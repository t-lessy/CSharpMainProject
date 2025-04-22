using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;
using static UnityEngine.GraphicsBuffer;

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
        List<PathNode> pathNodes = FindPath();
        List<Vector2Int> result = new List<Vector2Int>();
        Debug.Log("Calculate работает"); //ДЕБАГ
        if (pathNodes != null)
        {
            Debug.Log("FindPath != null"); //ДЕБАГ
            foreach (var node in pathNodes)
            {
                result.Add(node.Position);
            }
        }
        else
        {
            Debug.Log("FindPath == null");  //ДЕБАГ
            result.Add(StartPoint);
        }

            path = result.ToArray();
    }


    public List<PathNode> FindPath() //Надо 2 листа сделать, открытый и закрытый
    {
        PathNode startNode = new PathNode(StartPoint);
        PathNode targetNode = new PathNode(EndPoint);

        Debug.Log($"StartPoint = {StartPoint}"); //DEBUG
        Debug.Log($"EndPoint = {EndPoint}");    //DEBUG

        //Сюда добавляются все вершины в которые можно пойти
        List<PathNode> openList = new List<PathNode> { startNode };

        //Сюда добавляются вершины по которым уже прошли, они в вычислениях не учавствуют
        List<PathNode> closedList = new List<PathNode>();

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];

            DebugCounter++; //DEBUG
            if (DebugCounter >= MaxDebugCounter) //DEBUG
            {
                Debug.Log("Бесконечный цикл");
                DebugCounter = 0; 
                return null; 
            }


            foreach (var node in openList)  //РАБОТАЕТ
            {
                if (node.Value < currentNode.Value)
                    currentNode = node;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            //Если координаты текущего Node равны координатам текущей цели, значит путь дошёл до конца. Если это случилось, то нужно сформировать список Node пути и вывести его
            if (currentNode.Position == targetNode.Position) //НЕ РАБОТАЕТ (Хотя должно)
            {
                List<PathNode> path = new List<PathNode>();

                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.Parent;
                }

                path.Reverse();
                DebugCounter = 0; //DEBUG
                Console.WriteLine("Путь нашёлся!!!"); //DEBUG
                return path;
            }

            for (int i = 0; i < dx.Length; i++)
            {
                int newX = currentNode.Position.x + dx[i];
                int newY = currentNode.Position.y + dy[i];
                Vector2Int newPos = new Vector2Int(newX, newY);

                if (runtimeModel.IsTileWalkable(newPos))
                {
                    PathNode neighbor = new PathNode(newPos);
                    if (closedList.Contains(neighbor)) //РАБОТАЕТ
                    {
                        //Debug.Log($"{neighbor} Есть в закрытом листе"); 
                        continue;
                    }

                    neighbor.Parent = currentNode;
                    neighbor.CalculateEstimate(targetNode.Position);
                    neighbor.CalculateValue();

                    openList.Add(neighbor);
                }
            }
        }
        DebugCounter = 0; //DEBUG
        return null;
    }
}
