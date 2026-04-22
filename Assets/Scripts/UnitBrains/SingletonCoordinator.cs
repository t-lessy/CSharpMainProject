using Model;
using Model.Runtime.ReadOnly;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using static UnityEngine.EventSystems.EventTrigger;

public class SingletonCoordinator
{
    private static SingletonCoordinator _singletonCoordinator;
    private IReadOnlyRuntimeModel _runtimeModel;
    private TimeUtil _timeUtil;
    private IReadOnlyUnit RecommendedPlayerTarget;
    private Vector2Int RecommendedPlayerPos;
    private IReadOnlyUnit RecommendedEnemyTarget;
    private Vector2Int RecommendedEnemyPos;

    private SingletonCoordinator()
    {
        _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
        _timeUtil = ServiceLocator.Get<TimeUtil>();
        _timeUtil.AddFixedUpdateAction(CalcRecomendedTarget);
        _timeUtil.AddFixedUpdateAction(CalcRecomendedPos);
    }

    public static SingletonCoordinator GetInstance()
    {
        if (_singletonCoordinator == null)
            _singletonCoordinator = new SingletonCoordinator();
        return _singletonCoordinator;
    }

    public IReadOnlyUnit GetRecomendedTarget(bool IsPlayerUnitBrain)
    {
        return IsPlayerUnitBrain ? RecommendedPlayerTarget : RecommendedEnemyTarget;
    }

    public Vector2Int GetRecomendedPos(bool IsPlayerUnitBrain)
    {
        return IsPlayerUnitBrain ? RecommendedPlayerPos : RecommendedEnemyPos;
    }

    private void CalcRecomendedTarget(float fixedDeltaTime)
    {
        RecommendedPlayerTarget = CalcTarget(_runtimeModel.RoMap.Bases[RuntimeModel.PlayerId], _runtimeModel.RoBotUnits, _runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
        RecommendedEnemyTarget = CalcTarget(_runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId], _runtimeModel.RoPlayerUnits, _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
    }

    private void CalcRecomendedPos(float fixedDeltaTime)
    {
        RecommendedPlayerPos = CalcPos(_runtimeModel.RoMap.Bases[RuntimeModel.PlayerId], _runtimeModel.RoBotUnits, _runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
        RecommendedEnemyPos = CalcPos(_runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId], _runtimeModel.RoPlayerUnits, _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
    }

    private IReadOnlyUnit CalcTarget(Vector2Int defenseBase, IEnumerable<IReadOnlyUnit> units, Vector2Int attackBase)
    {
        IReadOnlyUnit lowHealthEnemy = null;
        IReadOnlyUnit nearestEnemyToBase = null;
        foreach (var unit in units)
        {
            if (Vector2Int.Distance(unit.Pos, defenseBase) < Vector2Int.Distance(unit.Pos, attackBase))
            {
                if (nearestEnemyToBase == null)
                {
                    nearestEnemyToBase = unit;
                }
                else
                {
                    nearestEnemyToBase = Vector2Int.Distance(unit.Pos, defenseBase) < Vector2Int.Distance(nearestEnemyToBase.Pos, defenseBase) ? unit : nearestEnemyToBase;
                }
                continue;
            }

            if (lowHealthEnemy == null || unit.Health < lowHealthEnemy.Health)
            {
                lowHealthEnemy = unit;
            }
        }

        return nearestEnemyToBase == null ? lowHealthEnemy : nearestEnemyToBase;
    }

    private Vector2Int CalcPos(Vector2Int defenseBase, IEnumerable<IReadOnlyUnit> units, Vector2Int attackBase)
    {
        IReadOnlyUnit nearestEnemyToBase = null;
        foreach (var unit in units)
        {
            if (Vector2Int.Distance(unit.Pos, defenseBase) < Vector2Int.Distance(unit.Pos, attackBase))
            {
                return defenseBase;
            }

            if (nearestEnemyToBase == null || Vector2Int.Distance(unit.Pos, defenseBase) < Vector2Int.Distance(nearestEnemyToBase.Pos, defenseBase))
            {
                nearestEnemyToBase = unit;
            }
        }

        return nearestEnemyToBase == null ? attackBase : nearestEnemyToBase.Pos;
    }
}
