using Model;
using Model.Config;
using Model.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnitBrains;
using UnityEngine;
using Utilities;
using static Codice.Client.BaseCommands.Import.Commit;

namespace Assets.Scripts.Utilities
{
    public class EffectSystem : MonoBehaviour
    {
        private Dictionary<Unit, List<Effect>> unitEffects = new Dictionary<Unit, List<Effect>>();
        private Coroutine updateCoroutine;

       

        private void Awake()
        {
            // Регистрируем систему в ServiceLocator
            ServiceLocator.Register<EffectSystem>(this);

            // Запускаем корутину для обновления эффектов
            updateCoroutine = StartCoroutine(UpdateEffectsCoroutine());
        }

        private void OnDestroy()
        {
            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);

            ServiceLocator.Unregister<EffectSystem>();
        }

        // Добавление эффекта юниту
        public void AddEffect(Unit unit, Effect effect)
        {
            if (!unitEffects.ContainsKey(unit))
            {
                unitEffects[unit] = new List<Effect>();
            }

            // Проверяем, есть ли уже такой эффект
            var existingEffect = unitEffects[unit].Find(e => e.EffectId == effect.EffectId);
            if (existingEffect != null)
            {
                // Обновляем длительность существующего эффекта
                existingEffect.Duration = effect.Duration;
            }
            else
            {
                // Добавляем новый эффект
                unitEffects[unit].Add(effect);
            }

            Debug.Log($"Добавлен эффект {effect.DisplayName} юниту {unit.Config.name}");
        }

        // Удаление эффекта
        public void RemoveEffect(Unit unit, string effectId)
        {
            if (unitEffects.ContainsKey(unit))
            {
                var effect = unitEffects[unit].Find(e => e.EffectId == effectId);
                if (effect != null)
                {
                    unitEffects[unit].Remove(effect);
                    Debug.Log($"Удален эффект {effectId} у юнита {unit.Config.name}");
                }
            }
        }

        // Получение модификатора скорости передвижения
        public float GetMoveSpeedModifier(Unit unit)
        {
            float modifier = 1f;

            if (unitEffects.ContainsKey(unit))
            {
                foreach (var effect in unitEffects[unit])
                {
                    modifier *= effect.MoveSpeedModifier;
                }
            }

            return modifier;
        }

        // Получение модификатора скорости атаки
        public float GetAttackSpeedModifier(Unit unit)
        {
            float modifier = 1f;

            if (unitEffects.ContainsKey(unit))
            {
                foreach (var effect in unitEffects[unit])
                {
                    modifier *= effect.AttackSpeedModifier;
                }
            }

            return modifier;
        }

        // Получение модификатора урона
        public float GetDamageModifier(Unit unit)
        {
            float modifier = 1f;

            if (unitEffects.ContainsKey(unit))
            {
                foreach (var effect in unitEffects[unit])
                {
                    modifier *= effect.DamageModifier;
                }
            }

            return modifier;
        }

        // Корутина для обновления эффектов
        private IEnumerator UpdateEffectsCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f); // Обновляем каждые 0.1 секунды

                UpdateEffects(0.1f);
            }
        }

        // Обновление длительности эффектов
        private void UpdateEffects(float deltaTime)
        {
            var unitsToRemove = new List<Unit>();

            foreach (var unitEntry in unitEffects)
            {
                var unit = unitEntry.Key;
                var effects = unitEntry.Value;

                // Обновляем длительность эффектов
                for (int i = effects.Count - 1; i >= 0; i--)
                {
                    effects[i].UpdateDuration(deltaTime);

                    if (effects[i].IsExpired)
                    {
                        Debug.Log($"Эффект {effects[i].DisplayName} истек у юнита {unit.Config.name}");
                        effects.RemoveAt(i);
                    }
                }

                // Если у юнита не осталось эффектов, помечаем для удаления
                if (effects.Count == 0)
                {
                    unitsToRemove.Add(unit);
                }
            }

            // Удаляем юнитов без эффектов
            foreach (var unit in unitsToRemove)
            {
                unitEffects.Remove(unit);
            }
        }

        // Получение всех активных эффектов юнита (для UI)
        public List<Effect> GetUnitEffects(Unit unit)
        {
            if (unitEffects.ContainsKey(unit))
            {
                return new List<Effect>(unitEffects[unit]);
            }
            return new List<Effect>();
        }


    }
    [System.Serializable]
    public class Effect
    {
        public string EffectId;
        public string DisplayName;
        public EffectType Type;
        public float Duration;
        public float MoveSpeedModifier = 1f;
        public float AttackSpeedModifier = 1f;
        public float DamageModifier = 1f;

        public bool IsExpired => Duration <= 0;

        public Effect(string id, string name, EffectType type, float duration,
                     float moveMod = 1f, float attackMod = 1f, float damageMod = 1f)
        {
            EffectId = id;
            DisplayName = name;
            Type = type;
            Duration = duration;
            MoveSpeedModifier = moveMod;
            AttackSpeedModifier = attackMod;
            DamageModifier = damageMod;
        }

        public void UpdateDuration(float deltaTime)
        {
            if (Duration > 0)
                Duration -= deltaTime;
        }
    }

    public enum EffectType
    {
        Buff,
        Debuff
    }
}
