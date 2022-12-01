using UnityEngine;

namespace DOTS.DOD.LESSON0
{
    public class RotateCube : MonoBehaviour
    {
        [Range(0, 360)] public float rotateSpeed = 360.0f;

        void Update()
        {
            transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
        }
    }
}
