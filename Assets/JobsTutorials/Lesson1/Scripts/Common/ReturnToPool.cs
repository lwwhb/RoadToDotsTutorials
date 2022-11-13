using UnityEngine;
using UnityEngine.Pool;

namespace Jobs.Common
{
    public class ReturnToPool : MonoBehaviour
    {
        public ObjectPool<GameObject> pool = null;

        public void OnDisappear()
        {
            if (pool != null)
                pool.Release(gameObject);
        }
    }
}

