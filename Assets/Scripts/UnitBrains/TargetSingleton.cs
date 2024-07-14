using Model;
using Model.Config;
using Model.Runtime.ReadOnly;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Utilities;

public class TargetSingleton
{
    private static TargetSingleton _instance;
    private readonly TimeUtil _timeUtil = ServiceLocator.Get<TimeUtil>();
    protected static IReadOnlyRuntimeModel runtimeModel => ServiceLocator.Get<IReadOnlyRuntimeModel>();
    private bool _isPlayerUnitBrain;
    private int _mapLenght;
    private Vector2Int target;
    private Vector2Int targetPos;
    private Vector2Int basePoint;
    

    public TargetSingleton(bool isPlayerUnitBrain = false)
    {
        _isPlayerUnitBrain = isPlayerUnitBrain;
        basePoint = runtimeModel.RoMap.Bases[
                isPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];
        targetPos = basePoint;
        _mapLenght = runtimeModel.RoMap.Width;
    }
    
    public static TargetSingleton GetInstance(bool isPlayerUnitBrain = false, TimeUtil timeUtil = null, IReadOnlyRuntimeModel runtimeModel = null)
    {
        if (_instance == null || isPlayerUnitBrain != false)
        {
            _instance = new TargetSingleton(isPlayerUnitBrain);
        }
        return _instance;
    }


    public Vector2Int GetTargetRecommendation()
    {
        return target;
    }
    public Vector2Int GetTargetPosRecommendation()
    {
        return targetPos;
    }

    private void GetEnemies()
    {
        List<IReadOnlyUnit> enemyNearBase = new List<IReadOnlyUnit>();
        if (_isPlayerUnitBrain == true)
        {
            foreach (var enemy in runtimeModel.RoBotUnits)
            {
                if(enemy.Pos.x <= _mapLenght / 2)
                {
                     enemyNearBase.Add(enemy);
                }
            }

        }
        else
        {
            foreach (var enemy in runtimeModel.RoPlayerUnits)
            {
                if (enemy.Pos.x >= _mapLenght / 2)
                {
                    enemyNearBase.Add(enemy);
                }
            }
        }

        if (enemyNearBase.Count > 0)
        {
            Vector2Int targetEnemy = enemyNearBase[0].Pos;
            float minDistance = Vector2Int.Distance(targetEnemy, basePoint);
            for (int i = 1; i < enemyNearBase.Count; i++)
            {
                float distance = Vector2Int.Distance(enemyNearBase[i].Pos, basePoint);
                if (distance < minDistance)
                {
                    targetEnemy = enemyNearBase[i].Pos;
                    minDistance = distance;
                };
            }
            targetPos = new Vector2Int(basePoint.x + (_isPlayerUnitBrain? 1:-1), basePoint.y); //Рекомендуемая точка.
            target = targetEnemy; // Устанавливаем Рекомендуемую цель.
        }
        else
        {
            int MinHealth = int.MaxValue;
            float minDistance = float.MaxValue;
            foreach (var enemy in _isPlayerUnitBrain ? runtimeModel.RoBotUnits : runtimeModel.RoPlayerUnits)
            {
                float distance = Vector2Int.Distance(enemy.Pos, basePoint);
                if (distance < minDistance)
                {
                    
                    minDistance = distance;
                    targetPos = new Vector2Int(enemy.Pos.x + (int) (_isPlayerUnitBrain ? -enemy.Config.AttackRange : enemy.Config.AttackRange), 
                        enemy.Pos.y); //рекомендуемая точка находится на расстоянии выстрела от ближайшего к базе врага.
                }
                if (enemy.Health < MinHealth)
                {
                    target = enemy.Pos;
                    MinHealth = enemy.Health;
                }
            }
        }
    }
    public void GetRecommendationDelayed(Vector2Int unitPos, int playerId, float delay = 1)
    {
        _timeUtil.RunDelayed(delay, () => GetEnemies());
    }
}
