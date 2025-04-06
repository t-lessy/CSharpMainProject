using System.Linq;
using Model;
using Model.Runtime.ReadOnly;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class UnitCoordinator
    {
        private static readonly UnitCoordinator s_instance = new();

        private readonly IReadOnlyRuntimeModel _runtimeModel;

        private readonly int _halfMapWidth;
        private readonly bool _isPlayerOnLeftSide;

        public Vector2Int SuggestedTarget { get; private set; }

        private UnitCoordinator()
        {
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();

            _halfMapWidth = _runtimeModel.RoMap.Width / 2;
            _isPlayerOnLeftSide = _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId].x < _halfMapWidth;

            ServiceLocator.Get<TimeUtil>().AddFixedUpdateAction(Update);
        }

        public static UnitCoordinator Instance() => s_instance;

        private void Update(float deltaTime)
        {
            SuggestedTarget = _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];

            var enemies = _runtimeModel.RoBotUnits.ToList();
            if (!enemies.Any())
                return;
            enemies.Sort(CompareByHealth);
            SuggestedTarget = enemies.First().Pos;

            var enemiesOnPlayerSide = enemies.Where(IsTargetOnPlayerSide).ToList();
            if (!enemiesOnPlayerSide.Any())
                return;
            enemiesOnPlayerSide.Sort(CompareByDistanceToPlayerBase);
            SuggestedTarget = enemiesOnPlayerSide.First().Pos;
        }

        private int CompareByHealth(IReadOnlyUnit a, IReadOnlyUnit b) =>
            a.Health.CompareTo(b.Health);

        private bool IsTargetOnPlayerSide(IReadOnlyUnit target) =>
            target.Pos.x < _halfMapWidth && _isPlayerOnLeftSide
            || target.Pos.x >= _halfMapWidth && !_isPlayerOnLeftSide;

        private int CompareByDistanceToPlayerBase(IReadOnlyUnit a, IReadOnlyUnit b)
        {
            var distanceA = DistanceToPlayerBase(a);
            var distanceB = DistanceToPlayerBase(b);
            return distanceA.CompareTo(distanceB);

            float DistanceToPlayerBase(IReadOnlyUnit target) =>
                Vector2Int.Distance(target.Pos, _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
        }
    }
}