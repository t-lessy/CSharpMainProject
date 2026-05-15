using System;
using System.Collections.Generic;
using System.Linq;
using Model.Config;
using UnityEngine;

namespace UnitBrains
{
    public static class UnitBrainProvider
    {
        private static readonly List<BaseUnitBrain> brainsCache = new List<BaseUnitBrain>();

        private static PlayerCoordinator playerCoordinator;
        private static PlayerCoordinator botCoordinator;

        public static void SetCoordinators(
            PlayerCoordinator newPlayerCoordinator,
            PlayerCoordinator newBotCoordinator)
        {
            playerCoordinator = newPlayerCoordinator;
            botCoordinator = newBotCoordinator;
        }

        public static BaseUnitBrain GetBrain(UnitConfig forUnit)
        {
            InitBrainsCache();

            var brain = brainsCache.FirstOrDefault(b =>
                b.TargetUnitName == forUnit.Name &&
                b.IsPlayerUnitBrain == forUnit.IsPlayerUnit);

            if (brain == null)
            {
                brain = brainsCache.FirstOrDefault(b =>
                    string.IsNullOrEmpty(b.TargetUnitName) &&
                    b.IsPlayerUnitBrain == forUnit.IsPlayerUnit);
            }

            if (brain == null)
            {
                Debug.LogError($"Could not find brains for unit {forUnit.Name}");
                return null;
            }

            var createdBrain = (BaseUnitBrain)Activator.CreateInstance(brain.GetType());

            if (forUnit.IsPlayerUnit)
                createdBrain.SetCoordinator(playerCoordinator);
            else
                createdBrain.SetCoordinator(botCoordinator);

            return createdBrain;
        }

        private static void InitBrainsCache()
        {
            if (brainsCache.Count != 0)
                return;

            brainsCache.AddRange(
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !t.IsAbstract && typeof(BaseUnitBrain).IsAssignableFrom(t))
                    .Select(t => (BaseUnitBrain)Activator.CreateInstance(t)));
        }
    }
}