
using System.Collections.Generic;
using UnityEngine;

namespace Jobs.OOD
{
    public class WaveCubes : MonoBehaviour
    {
        public GameObject cubeAchetype = null;
        [Range(10, 100)] public int xHalfCount = 40;
        [Range(10, 100)] public int zHalfCount = 40;
        private List<Transform> cubesList;
        void Start()
        {
            cubesList = new List<Transform>();
            for (var z = -zHalfCount; z <= zHalfCount; z++)
            {
                for (var x = -xHalfCount; x <= xHalfCount; x++)
                {
                    var cube = Instantiate(cubeAchetype);
                    cube.transform.position = new Vector3(x * 1.1f, 0, z * 1.1f);
                    cubesList.Add(cube.transform);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            for (var i = 0; i < cubesList.Count; i++)
            {
                var distance = Vector3.Distance(cubesList[i].position, Vector3.zero);
                cubesList[i].localPosition += Vector3.up * Mathf.Sin(Time.time * 3f + distance * 0.2f);
            }
        }
    }
}
