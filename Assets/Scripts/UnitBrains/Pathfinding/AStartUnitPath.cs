using Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;

public class AStartUnitPath : BaseUnitPath
{
    //private const int MaxLength = 100;
    private Vector2Int[] _shift = 
    {
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
    };
    public AStartUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
    {
    }

    protected override void Calculate()
    {
        Debug.Log("ALE BLYAT");
        path = FindPath().ToArray();
    }

    private List<Vector2Int> FindPath()
    {
        Debug.Log("ALE BLYAT");
        CellClass startCell = new CellClass(startPoint);
        CellClass targetCell = new CellClass(endPoint);

        List<CellClass> openList = new List<CellClass> { startCell };
        List<CellClass> closeList = new List<CellClass>();

        while (openList.Count > 0)
        {
            CellClass currentCell = openList[0];
            foreach (CellClass cell in openList)
            {
                if (cell.Value < currentCell.Value)
                    currentCell = cell;
            }
            openList.Add(currentCell);
            closeList.Add(currentCell);

            if (currentCell._cords == targetCell._cords)
            {
                //Vector2Int[] path
                List<Vector2Int> path = new List<Vector2Int>();

                while (currentCell != null)
                {
                    path.Add(currentCell._cords);
                    currentCell = currentCell.Parent;
                }
                path.Reverse();
                return path;
            }
            for (int i = 1; i < _shift.Length; i++)
            {
                Vector2Int newCords = currentCell._cords + _shift[i];

                if (runtimeModel.IsTileWalkable(newCords) || newCords == targetCell._cords 
                    || runtimeModel.RoUnits.Any(u => u.Pos == newCords))
                {
                    CellClass nei = new CellClass(newCords);

                    if (closeList.Contains(nei) || openList.Contains(nei))
                        continue;

                    nei.Parent = currentCell;
                    nei.CalculateEstimate(targetCell._cords);
                    nei.CalculateValue();

                    openList.Add(nei);
                }
            }
        }
        return null;
    }

    //protected override void Calculate()
    //{
    //    var currentPoint = startPoint;
    //    var result = new List<Vector2Int> { startPoint };
    //    var counter = 0;
    //    while (currentPoint != endPoint && counter++ < 100)
    //    {
    //        var nextStep = CalcNextStepTowards(currentPoint, endPoint);
    //        var hasLoop = result.Contains(nextStep);
    //        result.Add(nextStep);
    //        if (hasLoop)
    //            break;
    //        currentPoint = nextStep;
    //    }

    //    path = result.ToArray();
    //}
    //private Vector2Int CalcNextStepTowards(Vector2Int fromPos, Vector2Int toPos)
    //{
    //    var diff = toPos - fromPos;
    //    var stepDiff = diff.SignOrZero();
    //    var nextStep = fromPos + stepDiff;

    //    if (runtimeModel.IsTileWalkable(nextStep))
    //        return nextStep;

    //    if (stepDiff.sqrMagnitude > 1)
    //    {
    //        var partStep0 = fromPos + new Vector2Int(stepDiff.x, 0);
    //        if (runtimeModel.IsTileWalkable(partStep0))
    //            return partStep0;

    //        var partStep1 = fromPos + new Vector2Int(0, stepDiff.y);
    //        if (runtimeModel.IsTileWalkable(partStep1))
    //            return partStep1;
    //    }

    //    var sideStep0 = fromPos + new Vector2Int(stepDiff.y, -stepDiff.x);
    //    var shiftedStep0 = fromPos + (sideStep0 + stepDiff).SignOrZero();
    //    if (runtimeModel.IsTileWalkable(shiftedStep0))
    //        return shiftedStep0;

    //    var sideStep1 = fromPos + new Vector2Int(-stepDiff.y, stepDiff.x);
    //    var shiftedStep1 = fromPos + (sideStep1 + stepDiff).SignOrZero();
    //    if (runtimeModel.IsTileWalkable(shiftedStep1))
    //        return shiftedStep1;

    //    if (runtimeModel.IsTileWalkable(sideStep0))
    //        return sideStep0;

    //    if (runtimeModel.IsTileWalkable(sideStep1))
    //        return sideStep1;

    //    return fromPos;
    //}
}
