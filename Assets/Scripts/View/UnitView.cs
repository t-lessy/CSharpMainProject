using Model.Runtime.ReadOnly;
using UnitBrains.Pathfinding;
using UnityEngine;

namespace View
{
    public class UnitView : MonoBehaviour
    {
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private DebugPathOutput _debugPathOutput;

        public void UpdateState(IReadOnlyUnit model, Vector3 prevPosition)
        {
            _healthBar.UpdateHealth((float) model.Health / model.Config.MaxHealth);
            var deltaPos = transform.position - prevPosition;
            if (deltaPos != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(deltaPos, Vector3.up);
            }
            if (_debugPathOutput != null &&
                model.ActivePath != null &&
                model.ActivePath?.EndPoint != _debugPathOutput.Path?.EndPoint)
            {
                Debug.Log("Звёзды сошлись");
                _debugPathOutput.HighlightPath(model.ActivePath);
            }

            Debug.Log("UpdateState работает");

            if (_debugPathOutput != null)
                Debug.Log("_debugPathOutput != null");

            if (model.ActivePath != null)
                Debug.Log("model.ActivePath != null");

            if (_debugPathOutput != null &&
                model.ActivePath != null)
            {
                Debug.Log("_debugPathOutput_debugPathOutput != null && model.ActivePath != null");
            }
        }
    }
}