using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitBrains.Pathfinding;
using UnityEngine;
using Model.Runtime.ReadOnly;

public class Nodes
{
    public Vector2Int Pos;
    public int Cost = 1;
    public int Estimation;
    public int Value;
    public Nodes Parent;

    public Nodes(Vector2Int pos)
    {
        Pos = pos;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Nodes Nodes)
            return Pos.x == Nodes.Pos.x && Pos.y == Nodes.Pos.y;

        return false;
    }
}

public class UpdatedUnitPath : BaseUnitPath
{
    public UpdatedUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint)
            : base(runtimeModel, startPoint, endPoint)
    {
    }

    protected override void Calculate()
    {
        Nodes startNodes = new Nodes(startPoint);
        Nodes targetNodes = new Nodes(endPoint);

        int maxDistance = ManhattanDistance(startNodes, targetNodes);
        int tryDistacne = 0;

        // if the path is blocked, let's try to find at least part of the path
        while ((path == null || path.Length == 0) && tryDistacne < maxDistance)
        {
            path = CalculatePath(startNodes, targetNodes, tryDistacne)
                .Select(Nodes => Nodes.Pos)
                .ToArray();
            tryDistacne++;
        }
    }

    public List<Nodes> CalculatePath(Nodes start, Nodes target, int distance)
    {
        var openList = new List<Nodes> { start };
        var closedList = new List<Nodes>();

        while (openList.Count > 0)
        {
            var current = openList
                .OrderBy(n => n.Value)
                .First();

            if (ManhattanDistance(current, target) <= distance)
                return ReconstructPath(current);

            openList.Remove(current);
            closedList.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                bool inOpen = openList.Contains(neighbor);
                bool inClone = closedList.Contains(neighbor);
                if (inOpen || inClone || !IsValid(neighbor))
                    continue;

                neighbor.Parent = current;
                neighbor.Estimation = ManhattanDistance(neighbor, target);
                neighbor.Value = neighbor.Cost + neighbor.Estimation;
                openList.Add(neighbor);
            }
        }

        return new();
    }

    public int ManhattanDistance(Nodes a, Nodes b) =>
        Math.Abs(a.Pos.x - b.Pos.x) + Math.Abs(a.Pos.y - b.Pos.y);

    public List<Nodes> ReconstructPath(Nodes Nodes)
    {
        var path = new List<Nodes>();
        while (Nodes != null)
        {
            path.Add(Nodes);
            Nodes = Nodes.Parent;
        }
        path.Reverse();
        return path;
    }

    public List<Nodes> GetNeighbors(Nodes Nodes)
    {
        var neighbors = new List<Nodes>();
        Vector2Int[] directions =
        {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
            };

        foreach (var dir in directions)
            neighbors.Add(new Nodes(Nodes.Pos + dir));

        return neighbors;
    }

    public bool IsValid(Nodes Nodes)
    {
        bool validX = Nodes.Pos.x >= 0 && Nodes.Pos.x < runtimeModel.RoMap.Width;
        bool validY = Nodes.Pos.y >= 0 && Nodes.Pos.y < runtimeModel.RoMap.Height;
        bool walkable = runtimeModel.IsTileWalkable(Nodes.Pos);
        return validX && validY && walkable;
    }
}

