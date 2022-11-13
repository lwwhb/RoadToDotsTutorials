using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Jobs.DOD
{
    public struct AutoRotateAndMoveJob : IJobParallelForTransform
    {
        public float deltaTime;
        public float moveSpeed;
        public float rotateSpeed;
        public NativeArray<Vector3> randTargetPosArray;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 moveDir = (randTargetPosArray[index] - transform.position).normalized;
            transform.position += moveDir * moveSpeed * deltaTime;
            Vector3 localEulerAngles = transform.localRotation.eulerAngles;
            localEulerAngles.y += rotateSpeed * deltaTime;
            Quaternion localRotation = Quaternion.Euler(localEulerAngles);
            transform.localRotation = localRotation;
        }
    }

    [BurstCompile]
    public struct AutoRotateAndMoveJobOptimize0 : IJobParallelForTransform
    {
        public float deltaTime;
        public float moveSpeed;
        public float rotateSpeed;
        public NativeArray<Vector3> randTargetPosArray;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 moveDir = (randTargetPosArray[index] - transform.position).normalized;
            transform.position += moveDir * moveSpeed * deltaTime;
            Vector3 localEulerAngles = transform.localRotation.eulerAngles;
            localEulerAngles.y += rotateSpeed * deltaTime;
            Quaternion localRotation = Quaternion.Euler(localEulerAngles);
            transform.localRotation = localRotation;
        }
    }

    [BurstCompile]
    public struct AutoRotateAndMoveJobOptimize1 : IJobParallelForTransform
    {
        public float deltaTime;
        public float moveSpeed;
        public float rotateSpeed;
        [ReadOnly] public NativeArray<Vector3> randTargetPosArray;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 moveDir = (randTargetPosArray[index] - transform.position).normalized;
            transform.position += moveDir * moveSpeed * deltaTime;
            Vector3 localEulerAngles = transform.localRotation.eulerAngles;
            localEulerAngles.y += rotateSpeed * deltaTime;
            Quaternion localRotation = Quaternion.Euler(localEulerAngles);
            transform.localRotation = localRotation;
        }
    }

    [BurstCompile]
    public struct AutoRotateAndMoveJobOptimize2 : IJobParallelForTransform
    {
        public float deltaTime;
        public float moveSpeed;
        public float rotateSpeed;
        [ReadOnly] public NativeArray<float3> randTargetPosArray;

        public void Execute(int index, TransformAccess transform)
        {
            float3 moveDir = math.normalize(randTargetPosArray[index] - (float3)transform.position);
            float3 delta = moveDir * moveSpeed * deltaTime;
            transform.position += new Vector3(delta.x, delta.y, delta.z);
            Vector3 localEulerAngles = transform.localRotation.eulerAngles;
            localEulerAngles.y += rotateSpeed * deltaTime;
            Quaternion localRotation = Quaternion.Euler(localEulerAngles);
            transform.localRotation = localRotation;
        }
    }
}


