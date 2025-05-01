using Model;
using System.Linq;
using UnityEngine;
using Utilities;

namespace UnitBrains.Player
{
    public class UserCoordinator : MonoBehaviour
    {
        private static UserCoordinator _instance;
        public static UserCoordinator Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(nameof(UserCoordinator));
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<UserCoordinator>();
                }
                return _instance;
            }
        }

        private IReadOnlyRuntimeModel _model;
        private TimeUtil _timeUtil;
        private float _standardAttackRange;
        public float StandardAttackRange => _standardAttackRange;

        public Vector2Int RecommendedTarget { get; private set; }
        public Vector2Int RecommendedPoint { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _model = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _timeUtil = ServiceLocator.Get<TimeUtil>();

            InitializeAttackRange();
            _timeUtil.AddFixedUpdateAction(UpdateRecommendations);
        }

        private void OnDestroy()
        {
            if (_timeUtil != null)
                _timeUtil.RemoveFixedUpdateAction(UpdateRecommendations);

            if (_instance == this)
                _instance = null;
        }

        private void InitializeAttackRange()
        {
            var myUnits = _model.RoUnits
                .Where(u => u.Config.IsPlayerUnit)
                .ToList();

            _standardAttackRange = myUnits.Count == 0
                ? 1f
                : myUnits.Min(u => u.Config.AttackRange);
        }

        private void UpdateRecommendations(float deltaTime)
        {
            var playerBase = _model.RoMap.Bases[RuntimeModel.PlayerId];
            var enemyBase = _model.RoMap.Bases[RuntimeModel.BotPlayerId];
            var enemies = _model.RoBotUnits.ToList();

            if (!enemies.Any())
            {
                RecommendNoEnemies(enemyBase);
                return;
            }

            bool enemyOnOurHalf = enemies.Any(e
                => Vector2Int.Distance(e.Pos, playerBase)
                 < Vector2Int.Distance(e.Pos, enemyBase));

            if (enemyOnOurHalf)
            {
                var closest = enemies
                    .OrderBy(e => Vector2Int.Distance(e.Pos, playerBase))
                    .First();
                RecommendEnemiesOnOurHalf(closest.Pos, playerBase);
            }
            else
            {
                var orderedByDistance = enemies
                    .OrderBy(e => Vector2Int.Distance(e.Pos, playerBase))
                    .ThenBy(e => e.Health)
                    .ToList();

                var nearest = orderedByDistance.First();

                var weakest = orderedByDistance[0];
                foreach (var e in orderedByDistance)
                    if (e.Health < weakest.Health)
                        weakest = e;

                RecommendedTarget = weakest.Pos;
                SetPointAwayFrom(nearest.Pos, playerBase);
            }
        }

        private void RecommendNoEnemies(Vector2Int enemyBase)
        {
            RecommendedTarget = enemyBase;
            RecommendedPoint = enemyBase;
        }

        private void RecommendEnemiesOnOurHalf(Vector2Int closestEnemyPos, Vector2Int playerBase)
        {
            RecommendedTarget = closestEnemyPos;
            RecommendedPoint = playerBase;
        }

        private void SetPointAwayFrom(Vector2Int enemyPos, Vector2Int playerBase)
        {
            Vector2 dir = (Vector2)(enemyPos - playerBase);
            RecommendedPoint = dir == Vector2.zero
                ? enemyPos
                : Vector2Int.RoundToInt((Vector2)enemyPos - dir.normalized * _standardAttackRange);
        }
    }
}