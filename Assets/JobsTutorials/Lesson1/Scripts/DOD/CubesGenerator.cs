using Jobs.Common;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Jobs.DOD
{
    [RequireComponent(typeof(BoxCollider))]
    public class CubesGenerator : MonoBehaviour
    {
        public GameObject cubeArchetype = null;
        public GameObject targetArea = null;
        [Range(10, 10000)] public int generationTotalNum = 500;
        [Range(1, 60)] public int generationNumPerTicktime = 5;
        [Range(0.1f, 1.0f)] public float tickTime = 0.2f;
        [HideInInspector]
        public Vector3 generatorAreaSize;
        [HideInInspector]
        public Vector3 targetAreaSize;
        
        public float rotateSpeed = 180.0f;
        public float moveSpeed = 5.0f;
        private TransformAccessArray transformsAccessArray;
        //job
        //private NativeArray<Vector3> randTargetPosArray;
        
        //optimize2
        private NativeArray<float3> randTargetPosArray;

        //开启collectionChecks时，当外部尝试销毁池内对象时，会触发异常报错
        public bool collectionChecks = true;
        // 对象池
        private ObjectPool<GameObject> pool = null;
        private Transform[] transforms;
        
        private float timer = 0.0f;

        void Start()
        {
            ///创建对象池
            if (pool == null)
                pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                    OnDestroyPoolObject, collectionChecks, 10, generationTotalNum);

            generatorAreaSize = GetComponent<BoxCollider>().size;
            targetAreaSize = targetArea.GetComponent<BoxCollider>().size;
            
            // job
            //randTargetPosArray = new NativeArray<Vector3>(generationTotalNum, Allocator.Persistent);
            //optimize2
            randTargetPosArray = new NativeArray<float3>(generationTotalNum, Allocator.Persistent);

            transforms = new Transform[generationTotalNum];
            for (int i = 0; i < generationTotalNum; i++)
            {
                GameObject cube = pool.Get();
                var component = cube.AddComponent<AutoReturnToPool>();
                component.pool = pool;

                Vector3 randGenerationPos =  transform.position + new Vector3(Random.Range(-generatorAreaSize.x * 0.5f, generatorAreaSize.x * 0.5f),
                    0, Random.Range(-generatorAreaSize.z * 0.5f, generatorAreaSize.z * 0.5f));
                component.generationPos = randGenerationPos;
                cube.transform.position = randGenerationPos;
                
                Vector3 randTargetPos = targetArea.transform.position + new Vector3(Random.Range(-targetAreaSize.x * 0.5f, targetAreaSize.x * 0.5f),
                    0, Random.Range(-targetAreaSize.z * 0.5f, targetAreaSize.z * 0.5f));
                randTargetPosArray[i] =  randTargetPos;
                component.targetPos = randTargetPos;
                
                transforms[i] = cube.transform;
            }
            transformsAccessArray = new TransformAccessArray(transforms);
            for (int i = generationTotalNum-1; i >=0; i--)
            {
                pool.Release(transforms[i].gameObject);
            }
            timer = 0.0f;
        }

        void Update()
        {
            //job
            //var autoRotateAndMoveJob = new AutoRotateAndMoveJob();
            //optimize2
            var autoRotateAndMoveJob = new AutoRotateAndMoveJobOptimize2();
            autoRotateAndMoveJob.randTargetPosArray = randTargetPosArray;
            autoRotateAndMoveJob.deltaTime = Time.deltaTime;
            autoRotateAndMoveJob.moveSpeed = moveSpeed;
            autoRotateAndMoveJob.rotateSpeed = rotateSpeed;
            JobHandle autoRotateAndMoveJobJobHandle =
                autoRotateAndMoveJob.Schedule(transformsAccessArray);
            autoRotateAndMoveJobJobHandle.Complete();
            
            if (timer >= tickTime)
            {
                GenerateCubes();
                timer -= tickTime;
            }
            timer += Time.deltaTime;
        }

        private void OnDestroy()
        {
            if(transformsAccessArray.isCreated)
                transformsAccessArray.Dispose();
            randTargetPosArray.Dispose();

            if (pool != null)
                pool.Dispose();
        }
        
        private void GenerateCubes()
        {
            if (!cubeArchetype  || pool == null)
                return;
            for (int i = 0; i < generationNumPerTicktime; ++i)
            {
                if (pool.CountActive < generationTotalNum)
                {
                    pool.Get();
                }
                else
                {
                    timer = 0;
                    return;
                }
            }
        }

        GameObject CreatePooledItem()
        {
            return Instantiate(cubeArchetype, transform);
        }

        void OnReturnedToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        void OnTakeFromPool(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }

        void OnDestroyPoolObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }
    }
}
