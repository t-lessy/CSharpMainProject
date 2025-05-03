using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Codice.Client.Common;
using Model;
using UnitBrains.Pathfinding;
using UnitBrains.Player;
using UnityEngine;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Ironclad Behemoth";
    private BaseUnitPath _activePath = null;
    public override BaseUnitPath ActivePath => _activePath;
   
    private bool _isMoving = false;
    private bool _isAttacking = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update(float deltaTime, float time)
    {
        float timeCount = 0f;

        do
        {
            timeCount += deltaTime;
        }
        while (timeCount <= time);
    }
    protected override List<Vector2Int> SelectTargets()
    {
        _isAttacking = true;
        var result = GetReachableTargets();

        if (_isMoving)
        {
            result.Clear();
        }
        else
        {
            Update(UnityEngine.Time.deltaTime, 1000);
            while (result.Count > 1)
                result.RemoveAt(result.Count - 1);
        }
        _isAttacking = false;
        return result;
    }

    public override Vector2Int GetNextStep()
    {
        _isMoving = true;

        if (_isAttacking)
        {
            _isMoving = false;
            return unit.Pos;
        }
        else
        {
            Update(UnityEngine.Time.deltaTime, 1000);
            if (HasTargetsInRange())
                return unit.Pos;

            var target = runtimeModel.RoMap.Bases[
                IsPlayerUnitBrain ? RuntimeModel.BotPlayerId : RuntimeModel.PlayerId];

            _activePath = new DummyUnitPath(runtimeModel, unit.Pos, target);
            _isMoving = false;
            //Thread.Sleep(10);
            return _activePath.GetNextStepFrom(unit.Pos);
        }

    }
    //public void DelateCount(float time)
    //{
    //    float timeCount = 0f;

    //    do
    //    {
    //        timeCount += Time.deltaTime;
    //    }
    //    while (timeCount <= time); 
    //}
}
