using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Codice.Client.Common;
using Model;
using UnitBrains.Pathfinding;
using UnitBrains.Player;
using UnityEditor;
using UnityEngine;
using Time = UnityEngine.Time;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{ 
    public enum UnitMode
    {
        Moving,
        Attacking,
        Switching,
    }

    public override string TargetUnitName => "Ironclad Behemoth";
    public override bool IsPlayerUnitBrain => true;

    private UnitMode _currentMode = UnitMode.Moving;
    private UnitMode _targetMode = UnitMode.Moving;

    private bool _isSwitching = false;
    private float _transitionTimer = 0f;
    private float TransitionDuration = 1.0f;

    private float _switchStartTime;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public override void Update(float deltaTime, float time)
    {
        if (_isSwitching)
        {
            //_transitionTimer += Time.deltaTime;
            if (Time.time - _switchStartTime >= TransitionDuration)
            {
                _currentMode = _targetMode;
                _isSwitching = false;
                Debug.Log($"Переход завершён: {_currentMode}");
            }
            else
                return;
        }

        switch (_currentMode)
        {
            case UnitMode.Switching:
                Debug.Log($"Current Mode: {_currentMode}");
                break;
            case UnitMode.Moving:
                GetNextStep();
                Debug.Log($"Current Mode: {_currentMode}");
                break;

            case UnitMode.Attacking:
                SelectTargets();
                Debug.Log($"Current Mode: {_currentMode}");
                break;
        }
    }
    protected override List<Vector2Int> SelectTargets()
    {

        if (_currentMode == UnitMode.Attacking)
        {
            return base.SelectTargets();
        }
        else
        {
            SwitchModeRequest(UnitMode.Attacking, Time.time);

            return base.SelectTargets();
        }

    }

    public override Vector2Int GetNextStep()
    {


        if (_currentMode == UnitMode.Moving)
        {
            return base.GetNextStep();
        }
        else
        {
            SwitchModeRequest(UnitMode.Moving, Time.time);
            return base.GetNextStep();
        }


    }

    public void SwitchModeRequest(UnitMode desiredMode, float currentTime)
    {
        if (_isSwitching || _currentMode == desiredMode)
            return;

        _targetMode = desiredMode;
        _transitionTimer = 0f;
        _isSwitching = true;
        _currentMode = UnitMode.Switching;
        _switchStartTime = currentTime;
    }

   
}
