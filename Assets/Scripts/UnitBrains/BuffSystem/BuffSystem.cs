using System.Collections.Generic;
using System.Linq;
using Controller;
using Model;
using Model.Runtime;
using Model.Runtime.ReadOnly;
using UnitBrains.Player;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UnitBrains.BuffSystem
{
    public class BuffSystem
    {
        private TimeUtil _timeUtil;
        private RuntimeModel _runtimeModel;
        private LevelController _levelController;
        private IReadOnlyList<List<Unit>> _listUnits;

        private List<Buff<Unit>> _buffs = new List<Buff<Unit>>()
        {
            new DoubleShootBuff(2),
            new MoveFasterBuff(2),
        };

        private Dictionary<Unit, Buff<Unit>> _effectedUnits = new Dictionary<Unit, Buff<Unit>>();
        
        public BuffSystem(LevelController levelController, RuntimeModel runtimeModel)
        {
            _timeUtil = ServiceLocator.Get<TimeUtil>();
            _levelController = levelController;
            _runtimeModel = runtimeModel;
            _levelController.OnGameStarted += OnGameStarted;
            Debug.Log("BuffSystem Created");
        }

        ~BuffSystem()
        {
            if (_levelController != null)
            {
                _levelController.OnGameStarted -= OnGameStarted;
            }
        }
        
        private void OnGameStarted()
        {
            _listUnits = _runtimeModel.PlayersUnits;
            SetBuffs();
        }

        public void SetBuffs()
        {
            foreach (var buff in _buffs)
            {
                _timeUtil.AddUpdateAction(buff.Update);
            }
            
            if (_listUnits == null)
            {
                Debug.LogError("Units list is not initialized.");
                return;
            }

            // Применяем баффы ко всем юнитам
            foreach (var unitList in _listUnits)
            {
                foreach (var unit in unitList)
                {
                    ApplyBuffsToUnit(unit);
                    
                }
            }
        }
        
        private void ApplyBuffsToUnit(Unit unit)
        {
            foreach (var buff in _buffs)
            {
                if (buff.CanApply(unit))
                {
                    buff.Add(unit);
                    _effectedUnits.Add(unit, buff);
                }
            }
        }
        
        // этот метод не нужен, задел на будущее
        public void AddBuff(Buff<Unit> buff)
        {
            _buffs.Add(buff);
            Debug.Log($"Added buff '{buff.Name}' to the system.");
        }
    }
}