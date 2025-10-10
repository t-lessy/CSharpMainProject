using System.Linq;
using Model;
using Model.Runtime.ReadOnly;
using UnityEngine;
using Utilities;

namespace UnitBrains.Coordinators
{
    public class UnitsCoordinator
    {
        public static UnitsCoordinator Instance { get; } = new UnitsCoordinator();

        public Vector2Int? RecommendedTarget { get; private set; }
        public Vector2Int RecommendedPosition { get; private set; }

        private readonly IReadOnlyRuntimeModel _runtimeModel;
        private readonly TimeUtil _timeUtil;

        private UnitsCoordinator()
        {
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _timeUtil = ServiceLocator.Get<TimeUtil>();
            _timeUtil.AddFixedUpdateAction(Update);
        }

        private void Update(float deltaTime)
        {
            var ourBase = _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId];
            var enemies = _runtimeModel.RoBotUnits;
            var enemyBase = _runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId];

            var enemiesOnOurHalf = enemies.Where(e => IsOnOurHalf(e.Pos)).ToList();

            if (enemiesOnOurHalf.Any())
            {
                RecommendedTarget = enemiesOnOurHalf.OrderBy(e => Vector2Int.Distance(e.Pos, ourBase)).First().Pos;
                RecommendedPosition = ourBase + new Vector2Int(0, 5);
            }
            else if (enemies.Any())
            {
                RecommendedTarget = enemies.OrderBy(e => e.Health).First().Pos;
                var closestEnemy = enemies.OrderBy(e => Vector2Int.Distance(e.Pos, ourBase)).First();
                RecommendedPosition = Vector2Int.RoundToInt(Vector2.MoveTowards(closestEnemy.Pos, ourBase, closestEnemy.Config.AttackRange - 1));
            }
            else
            {
                RecommendedTarget = enemyBase;
                RecommendedPosition = Vector2Int.RoundToInt(Vector2.MoveTowards(enemyBase, ourBase, 3f));
            }
        }

        private bool IsOnOurHalf(Vector2Int position)
        {
            var mapWidth = _runtimeModel.RoMap.Width;
            return position.x < mapWidth / 2;
        }
    }
}
