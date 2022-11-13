using Jobs.Common;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.UI;
using Random = UnityEngine.Random;
namespace Jobs.OOD
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
        
        //开启collectionChecks时，当外部尝试销毁池内对象时，会触发异常报错
        public bool collectionChecks = true;
        // 对象池
        private ObjectPool<GameObject> pool = null;
        private float timer = 0.0f;

        void Start()
        {
            ///创建对象池
            if (pool == null)
                pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                    OnDestroyPoolObject, collectionChecks, 10, generationTotalNum);

            generatorAreaSize = GetComponent<BoxCollider>().size;
            if (targetArea != null)
                targetAreaSize = targetArea.GetComponent<BoxCollider>().size;

            timer = 0.0f;
        }

        void Update()
        {
            if (timer >= tickTime)
            {
                GenerateCubes();
                timer -= tickTime;
            }

            timer += Time.deltaTime;
        }

        private void OnDestroy()
        {
            if (pool != null)
                pool.Dispose();
        }
        
        private void GenerateCubes()
        {
            if (!cubeArchetype  || pool == null)
                return;
            for (int i = 0; i < generationNumPerTicktime; ++i)
            {
                if (pool.CountAll< generationTotalNum)
                {
                    GameObject cube = pool.Get();
                    if (cube)
                    {
                        ReturnToPool component = cube.GetComponent<ReturnToPool>();
                        component.pool = pool;
                        Vector3 randomPos = new Vector3(Random.Range(-generatorAreaSize.x * 0.5f, generatorAreaSize.x * 0.5f),
                            0,
                            Random.Range(-generatorAreaSize.z * 0.5f, generatorAreaSize.z * 0.5f));
                        cube.transform.position = transform.position + randomPos;
                        if (targetArea)
                            cube.GetComponent<AutoRotateAndMove>().targetPos = GetRandomTargetPos();
                    }
                }
                else
                {
                    timer = 0;
                    return;
                }
            }
        }
        private Vector3 GetRandomTargetPos()
        {
            return targetArea.transform.position + new Vector3(Random.Range(-targetAreaSize.x * 0.5f, targetAreaSize.x * 0.5f),
                0,
                Random.Range(-targetAreaSize.z * 0.5f, targetAreaSize.z * 0.5f));
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
