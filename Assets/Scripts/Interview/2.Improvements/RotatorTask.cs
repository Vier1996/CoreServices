using UnityEngine;

namespace Interview._2.Improvements
{
    public class RotatorTask : MonoBehaviour
    {
        [Range(0f, 5f)] [SerializeField] private float _rotationSpeed;

        private Transform _selfTransform;
        private float _rotationDelta;
        private void Awake()
        {
            _selfTransform = transform;
            _rotationDelta = _rotationSpeed * Time.fixedDeltaTime;
        }

        private void FixedUpdate() => _selfTransform.Rotate(0, _rotationDelta, 0);
    }
    
    /*public class RotatorTask : MonoBehaviour
    {
        [Range(0f, 5f)] [SerializeField] private float _rotationSpeed;

        private void FixedUpdate() => 
            transform.Rotate(0, _rotationSpeed * Time.fixedDeltaTime, 0);
    }*/
}