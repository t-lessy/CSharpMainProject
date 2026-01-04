using System.Collections;
using System.Collections.Generic;
using UnitBrains.Player;
using UnityEngine;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    /// <summary>
    /// Имя целевой единицы. Всегда возвращает "Ironclad Behemoth".
    /// </summary>
    public string TargetUnitName => "Ironclad Behemoth";

    // Перечисление возможных состояний юнита
    private enum State
    {
        Moving,
        Attacking,
        Transitioning  // Состояние перехода между режимами
    }

    private State _currentState = State.Moving;  // Начальное состояние — движение
    private float _transitionTimer = 0f;       // Таймер перехода
    private const float TransitionDuration = 1f; // Длительность перехода (1 секунда)

    public virtual void Update()
    {
        Update();

        // Обновляем таймер перехода, если находимся в состоянии перехода
        if (_currentState == State.Transitioning)
        {
            _transitionTimer += Time.deltaTime;

            // По истечении времени перехода переключаемся в целевой режим
            if (_transitionTimer >= TransitionDuration)
            {
                _currentState = _targetState;
                _transitionTimer = 0f;
                Debug.Log($"ThirdUnitBrain: Завершён переход в режим {_currentState}");
            }
        }
    }

    /// <summary>
    /// Запускает движение юнита (если возможно).
    /// </summary>
    public void Move()
    {
        if (_currentState == State.Moving)
        {
            Debug.Log("ThirdUnitBrain: Юнит движется по уникальной логике!");
            // Здесь размещается логика движения
        }
        else if (_currentState != State.Transitioning)
        {
            StartTransition(State.Moving);
        }
    }

    /// <summary>
    /// Запускает атаку юнита (если возможно).
    /// </summary>
    public void Attack()
    {
        if (_currentState == State.Attacking)
        {
            Debug.Log("ThirdUnitBrain: Юнит атакует с особым алгоритмом!");
            // Здесь размещается логика атаки
        }
        else if (_currentState != State.Transitioning)
        {
            StartTransition(State.Attacking);
        }
    }

    /// <summary>
    /// Начинает переход в указанное состояние.
    /// </summary>
    /// <param name="targetState">Целевое состояние после перехода.</param>
    private void StartTransition(State targetState)
    {
        _targetState = targetState;
        _currentState = State.Transitioning;
        _transitionTimer = 0f;
        Debug.Log($"ThirdUnitBrain: Начался переход в режим {targetState} (длительность: {TransitionDuration}с)");
    }

    private State _targetState;  // Целевое состояние после завершения перехода

    private void SpecialAbility()
    {
        Debug.Log("ThirdUnitBrain: Активирована особая способность!");
    }
}
