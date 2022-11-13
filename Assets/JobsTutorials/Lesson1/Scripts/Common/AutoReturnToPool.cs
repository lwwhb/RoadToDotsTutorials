
using UnityEngine;
using UnityEngine.Pool;
namespace Jobs.Common
{
    public class AutoReturnToPool : MonoBehaviour
    {
        private const float Epsilon = 0.05f;
        public ObjectPool<GameObject> pool = null;
        public Vector3 generationPos;
        public Vector3 targetPos;

        private void OnEnable()
        {
            transform.position = generationPos;
        }

        private void Update()
        {
            if (pool != null)
            {
                if((transform.position - targetPos).magnitude < Epsilon)
                    pool.Release(gameObject);
            }
        }
    }
}
