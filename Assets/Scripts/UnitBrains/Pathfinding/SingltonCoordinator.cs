using Model;
using Model.Runtime.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.UnitBrains.Pathfinding
{
    public class SingltonCoordinator
    {
        // Приватное статическое поле для хранения единственного экземпляра
        private static SingltonCoordinator _instance;
        //получаем  сведения всей текущей сессии
        private IReadOnlyRuntimeModel _runtimeModel;
        private IReadOnlyUnit unitPos;
        private IReadOnlyMap _roMap;
        // Получаем нашу базу (PlayerId)
        private Vector2Int OurBase => _roMap.Bases.[RuntimeModel.PlayerId];

        // Получаем вражескую базу (BotPlayerId)
        private Vector2Int EnemyBase => _roMap.Bases[RuntimeModel.BotPlayerId];

        private TimeUtil _timeUtil;
        // Приватный конструктор - нельзя создать извне
        private SingltonCoordinator() 
        {
            Console.WriteLine("Создан экземпляр SingltonCoordinator");
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _timeUtil = ServiceLocator.Get<TimeUtil>();
        }

        // Публичный метод для получения экземпляра
        public static SingltonCoordinator GetInstance()
        {
            // Если экземпляр еще не создан - создаем
            if (_instance == null)
            {
                _instance = new SingltonCoordinator();
            }
            return _instance;
        }
        // Получаем всех вражеских юнитов
        private IEnumerable<IReadOnlyUnit> GetEnemyUnits()
        {
            return _runtimeModel.RoBotUnits;

        }
        // Получаем врагов на нашей половине карты
        private List<IReadOnlyUnit> GetEnemiesOnOurHalf()
        {
            return GetEnemyUnits()
                .Where(unit => IsOnOurHalf(unit.Pos))
                .ToList();
        }
        // Находим ближайшего врага к нашей базе
        private IReadOnlyUnit FindEnemyClosestToBase(IEnumerable<IReadOnlyUnit> enemies)
        {
            return enemies
                .OrderBy(unit => Vector2Int.Distance(unit.Pos, OurBase))
                .FirstOrDefault();
        }
        // Находим врага с наименьшим здоровьем
        private IReadOnlyUnit FindEnemyWithLowestHealth(IEnumerable<IReadOnlyUnit> enemies)
        {
            return enemies
                .OrderBy(unit => unit.Health)
                .FirstOrDefault();
        }
        
        // Проверяем, находится ли позиция на нашей половине карты
        private bool IsOnOurHalf(Vector2Int enemyPosition)
        {
            // Простая проверка: если ближе к нашей базе, чем к вражеской
            float distToOurBase = Vector2Int.Distance(enemyPosition, OurBase);
            float distToEnemyBase = Vector2Int.Distance(enemyPosition, EnemyBase);
            return distToOurBase < distToEnemyBase;
        }
        // Обычный метод класса
        public IReadOnlyUnit Coordination(IReadOnlyUnit unitPos)
        {
            var enemiesOnOurHalf = GetEnemiesOnOurHalf();
            var allEnemies = GetEnemyUnits().ToList();
            var attackRangeSqr = unitPos.Config.AttackRange * unitPos.Config.AttackRange;
            if (enemiesOnOurHalf.Count > 0)
            {
                foreach (var enemy in enemiesOnOurHalf)
                {
                    //Vector2Int vectorEnemy=new Vector2Int(enemy.Pos.x, enemy.Pos.y);

                    //если какой либо противник из  списка противников вне радиуса атаки возвращаем null
                    if(Vector2Int.Distance(enemy.Pos, unitPos.Pos)> attackRangeSqr)
                    {
                        return null;
                    }
                }
                // Если есть враги на нашей половине и находятся в радиусе атаки - атакуем ближайшего к базе
                return FindEnemyClosestToBase(enemiesOnOurHalf);
            }
            else if (allEnemies.Count > 0)
            {
                // Если врагов на нашей половине нет - атакуем самого слабого
                return FindEnemyWithLowestHealth(allEnemies);
            }

            return null; // Если врагов нет вообще
        }

        public void SubscribeToNotifications()
        {
            // Подписываемся на событие _updateAction
            _timeUtil.AddUpdateAction(Coordination(unitPos,0.5f));
            Console.WriteLine("SingltonCoordinator подписан на _updateAction");
        }
    }
}
