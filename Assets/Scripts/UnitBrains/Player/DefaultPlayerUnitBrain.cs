using Model;
using System.Collections.Generic;
using UnitBrains.Pathfinding;
using UnityEngine;

namespace UnitBrains.Player
{
    public class DefaultPlayerUnitBrain : BaseUnitBrain
    {
        private const float AttackRangeMultiplier = 2f;

        protected float DistanceToOwnBase(Vector2Int fromPos) =>
            Vector2Int.Distance(fromPos, runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);

        protected void SortByDistanceToOwnBase(List<Vector2Int> list)
        {
            list.Sort(CompareByDistanceToOwnBase);
        }

        private int CompareByDistanceToOwnBase(Vector2Int a, Vector2Int b)
        {
            var distanceA = DistanceToOwnBase(a);
            var distanceB = DistanceToOwnBase(b);
            return distanceA.CompareTo(distanceB);
        }
        public override Vector2Int GetNextStep()
        {
            var coord = UserCoordinator.Instance;
            var target = coord.RecommendedTarget;
            var center = coord.RecommendedPoint;

            Vector2Int point = GetDistributedPointAround(center, unit.Pos);
            float distToTarget = Vector2Int.Distance(unit.Pos, target);
            bool canAttack = distToTarget <= coord.StandardAttackRange * AttackRangeMultiplier;

            Vector2Int nextStep = canAttack ? target : point;

            if (!IsTargetInRange(nextStep))
            {
                var path = new AStarUnitPath(runtimeModel, unit.Pos, nextStep);
                VisualizePath(path);
                return path.GetNextStepFrom(unit.Pos);
            }

            return unit.Pos;
        }

        private static readonly Vector2Int[] DistributionOffsets =
        {
            new(0, 0),
            new(1, 0),  new(-1, 0),
            new(0, 1),  new(0, -1),
            new(1, 1),  new(-1, -1),
            new(1, -1), new(-1, 1)
        };

        protected Vector2Int GetDistributedPointAround(Vector2Int center, Vector2Int unitPos)
        {
            int hash = Mathf.Abs(unitPos.x ^ unitPos.y);
            int idx = hash % DistributionOffsets.Length;
            return center + DistributionOffsets[idx];
        }
    }
}