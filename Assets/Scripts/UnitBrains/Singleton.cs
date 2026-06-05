using Model;
using Model.Runtime.ReadOnly;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PriorityActions
{
    private static PriorityActions instance;
    private IReadOnlyRuntimeModel _runtimemodel;
    private TimeUtil _timeUtil;
    private IReadOnlyUnit PriorityPlayerTarget;
    private Vector2Int PriorityPlayerStep;
    private IReadOnlyUnit PriorityEnemyTarget;
    private Vector2Int PriorityEnemyStep;
    private int _middleMap;

    private PriorityActions()
    {
        _runtimemodel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
        _timeUtil = ServiceLocator.Get<TimeUtil>();
        _timeUtil.AddFixedUpdateAction(CalcPriorityTarget);
        _timeUtil.AddFixedUpdateAction(CalcPriorityStep);
        _middleMap = _runtimemodel.RoMap.Width / 2;
    }

    public static PriorityActions GetInstance()
    {
        if (instance == null)
            instance = new PriorityActions();

        return instance;
    }

    public IReadOnlyUnit GetPriorityTarget(bool IsPlayerUnitBrain)
    {
        return IsPlayerUnitBrain ? PriorityPlayerTarget : PriorityEnemyTarget;
    }

    public Vector2Int GetPriorityStep(bool IsPlayerUnitBrain)
    {
        return IsPlayerUnitBrain ? PriorityPlayerStep : PriorityEnemyStep;
    }

    private void CalcPriorityTarget(float fixedDeltaTime)
    {
        PriorityPlayerTarget = CalcTarget(_runtimemodel.RoMap.Bases[RuntimeModel.PlayerId], _runtimemodel.RoBotUnits);
        PriorityEnemyTarget = CalcTarget(_runtimemodel.RoMap.Bases[RuntimeModel.BotPlayerId], _runtimemodel.RoPlayerUnits);
    }

    private void CalcPriorityStep(float fixedDeltaTime)
    {
        PriorityPlayerStep = CalcStep(_runtimemodel.RoMap.Bases[RuntimeModel.BotPlayerId], _runtimemodel.RoPlayerUnits, _runtimemodel.RoMap.Bases[RuntimeModel.PlayerId]);
        PriorityEnemyStep = CalcStep(_runtimemodel.RoMap.Bases[RuntimeModel.PlayerId], _runtimemodel.RoBotUnits, _runtimemodel.RoMap.Bases[RuntimeModel.BotPlayerId]);
    }
    

    private Vector2Int CalcStep(Vector2Int defBase, IEnumerable<IReadOnlyUnit> targets, Vector2Int attackBase)
    {
        IReadOnlyUnit nearToBaseEnemy = null;
        foreach (var target in targets)
        {
            if (defBase.x > _middleMap && target.Pos.x > _middleMap)
            {
                return defBase + Vector2Int.right;
            }

            if (defBase.x < _middleMap && target.Pos.x < _middleMap)
            {
                return defBase + Vector2Int.left;
            }

            if (nearToBaseEnemy == null || Vector2Int.Distance(nearToBaseEnemy.Pos, defBase) > Vector2Int.Distance(target.Pos, defBase))
            {
                nearToBaseEnemy = target;
            }
        }
        return nearToBaseEnemy == null ? attackBase : nearToBaseEnemy.Pos;
    }

    private IReadOnlyUnit CalcTarget(Vector2Int defBase, IEnumerable<IReadOnlyUnit> targets)
    {
        IReadOnlyUnit LowHPTarget = null;
        IReadOnlyUnit nearToBaseUnit = null;
        foreach (var target in targets)
        {
            if (LowHPTarget == null || LowHPTarget.Health > target.Health)
            {
                LowHPTarget = target;
            }

            if (defBase.x > _middleMap && target.Pos.x > _middleMap || defBase.x < _middleMap && target.Pos.x < _middleMap)
            {
                if (nearToBaseUnit == null)
                {
                    nearToBaseUnit = target;
                }
                else
                {
                    nearToBaseUnit = Vector2Int.Distance(nearToBaseUnit.Pos, defBase) > Vector2Int.Distance(target.Pos, defBase) ? target : nearToBaseUnit;
                }
            }
        }
        return nearToBaseUnit == null ? LowHPTarget : nearToBaseUnit;
    }
}
