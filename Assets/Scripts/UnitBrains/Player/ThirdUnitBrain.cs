using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitBrains.Player
{
    public class ThirdUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Ironclad Behemoth";

        private static int _reloadingTimeSeconds = 1;
        private static DateTime _reloadingStartDateTime;

        private bool _isReloading;
        private bool _isBurstMode;

        public override Vector2Int GetNextStep()
        {
            if (NeedToSwitchMode()) StartModeSwitching();
            return _isReloading || _isBurstMode ? unit.Pos : base.GetNextStep();
        }

        protected override List<Vector2Int> SelectTargets()
        {
            if (NeedToSwitchMode()) StartModeSwitching();
            return _isReloading || !_isBurstMode ? new List<Vector2Int>() : base.SelectTargets();
        }

        public override void Update(float deltaTime, float time)
        {
            if (_isReloading)
            {
                int totalSwitchingTime = (DateTime.Now - _reloadingStartDateTime).Seconds;
                if (totalSwitchingTime >= _reloadingTimeSeconds) CompleteModeSwitching();
            }
        }

        private bool NeedToSwitchMode() => !_isReloading && (HasTargetsInRange() != _isBurstMode);

        private void StartModeSwitching()
        {
            _reloadingStartDateTime = DateTime.Now;
            _isReloading = true;
        }

        private void CompleteModeSwitching()
        {
            _isBurstMode = !_isBurstMode;
            _isReloading = false;
        }

    }
}