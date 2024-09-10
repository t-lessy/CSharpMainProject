using Model;
using Model.Runtime.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using UnitBrains;
using UnityEngine;
using Utilities;

namespace Assets.Scripts.Utilities
{
    public class UnitCoordinator
    {
        private static UnitCoordinator _instance;

        private readonly IReadOnlyRuntimeModel _runtimeModel;
        private readonly TimeUtil _timeUtil;
        public IReadOnlyUnit recommendTarget = null;


        private UnitCoordinator()
        {
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _timeUtil = ServiceLocator.Get<TimeUtil>();

            _timeUtil.AddUpdateAction(GetRecommendedTarget);
        }

        public static UnitCoordinator GetInstance()
        {
            if (_instance == null)
                _instance = new UnitCoordinator();
            return _instance;
        }


        private void GetRecommendedTarget(float deltaTime)
        {
            recommendTarget = null;

            int firstHalfMapX = _runtimeModel.RoMap.Width / 2;
            float closedDist = float.MaxValue;
            List<IReadOnlyUnit> Targets = _runtimeModel.RoBotUnits.ToList();

            foreach (IReadOnlyUnit target in Targets)
            {
                if (target.Pos.x < firstHalfMapX && closedDist > (target.Pos - _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]).magnitude)
                {
                    closedDist = (target.Pos - _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]).magnitude;
                    recommendTarget = target;
                }

            }

            if (recommendTarget != null)
            {
                return;
            }

            int minHP = int.MaxValue;
            foreach (IReadOnlyUnit target in Targets)
            {
                if (minHP > target.Health)
                {
                    minHP = target.Health;
                    recommendTarget = target;
                }
            }
        }
    }
}