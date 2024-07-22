using System.Collections.Generic;
using Model;
using Model.Runtime.Projectiles;
using UnitBrains.Pathfinding;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class DefaultPlayerUnitBrain : BaseUnitBrain
    {
        protected float DistanceToOwnBase(Vector2Int fromPos) =>
        Vector2Int.Distance(fromPos, runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);

        public static bool IsPlayerUnitBrain => true;
        public AStarUnitPath ActivePath => _activePath;

        private readonly TimeUtil _timeUtil = ServiceLocator.Get<TimeUtil>();

        private readonly UnitCoordinator _ts = ServiceLocator.Get<UnitCoordinator>();

        private AStarUnitPath _activePath = null;

        void Awake()
        {
        }
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
            if (HasTargetsInRange())
                return unit.Pos;
            var target = _ts.GetTargetPosRecommendation();

            _activePath = new AStarUnitPath(runtimeModel, unit.Pos, target);
            return _activePath.GetNextStepFrom(unit.Pos);
        }

    }
}