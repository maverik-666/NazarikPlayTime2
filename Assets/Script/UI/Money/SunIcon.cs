namespace Project
{
    using UnityEngine;

    public class SunIcon : MonoBehaviour
    {
        [SerializeField] 
        private Transform _aura;

        [SerializeField]
        private float _rotationSpeed = 20;

        private void Update()
        {
            _aura.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);

        }
    }
}