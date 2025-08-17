using Codice.Client.Common.GameUI;
using Model;
using Model.Config;
using Model.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using Utilities;

public class EffectsForUnits
{
    private RuntimeModel _runtimeModel;

    public EffectsForUnits(RuntimeModel runtimeModel)
    {
        _runtimeModel = runtimeModel;
    }

    public void SetAllUnitsMoveDelay(int durationInMs, float moveDelayModifier)
    {
        foreach (Unit unit in _runtimeModel.AllUnits)
        {
            SetUnitMoveDelay(unit, durationInMs, moveDelayModifier);
        }
    }
    public void SetUnitMoveDelay(Unit unit, int durationInMs, float moveDelayModifier)
    {
        SetTimerForMove(unit, durationInMs);
        unit.MoveDelay = unit.Config.MoveDelay * moveDelayModifier;
    }

    public void SetAllUnitsAttackDelay(int durationInMs, float attackDelayModifier)
    {
        foreach (Unit unit in _runtimeModel.AllUnits)
        {
            SetUnitAttackDelay(unit, durationInMs, attackDelayModifier);
        }
    }
    public void SetUnitAttackDelay(Unit unit, int durationInMs, float attackDelayModifier)
    {
        SetTimerForAttack(unit, durationInMs);
        unit.AttackDelay = unit.Config.AttackDelay * attackDelayModifier;
    }


    private void SetTimerForMove(Unit unit, int durationInMs)
    {
        System.Timers.Timer timer = new System.Timers.Timer(durationInMs);
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = false;
        timer.Enabled = true;

        void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            unit.MoveDelay = unit.Config.MoveDelay;

            timer.Stop();
            timer.Dispose();
        }
    }

    private void SetTimerForAttack(Unit unit, int durationInMs)
    {
        System.Timers.Timer timer = new System.Timers.Timer(durationInMs);
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = false;
        timer.Enabled = true;

        void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            unit.AttackDelay = unit.Config.AttackDelay;

            timer.Stop();
            timer.Dispose();
        }
    }
    




}

