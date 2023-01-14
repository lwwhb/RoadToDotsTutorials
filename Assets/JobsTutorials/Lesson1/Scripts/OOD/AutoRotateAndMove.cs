using Jobs.Common;
using Unity.Profiling;
using UnityEngine;

namespace Jobs.OOD
{
    [RequireComponent(typeof(ReturnToPool))]
    public class AutoRotateAndMove : MonoBehaviour
    {
        private const float Epsilon = 0.05f;
        public float rotateSpeed = 180.0f;
        public float moveSpeed = 5.0f;
        public Vector3 targetPos;

        static readonly ProfilerMarker profilerMarker = new ProfilerMarker("CubesMarch");

        // Update is called once per frame
        void Update()
        {
            using (profilerMarker.Auto())
            {
                transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
                Vector3 dist = targetPos - transform.position;
                if (dist.magnitude >= Epsilon)
                {
                    Vector3 moveDir = dist.normalized;
                    transform.position += moveDir * (moveSpeed * Time.deltaTime);
                }
                else
                {
                    ReturnToPool component = GetComponent<ReturnToPool>();
                    if (component)
                        component.OnDisappear();
                }
            }
        }
    }
}
