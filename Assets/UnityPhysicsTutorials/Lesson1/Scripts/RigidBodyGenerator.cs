using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DOTS.PHYSICS.LESSON0
{
    public class RigidBodyGenerator : MonoBehaviour
    {
        public GameObject rigidbodyBox;
        public Vector3 gridNum;
        public float space = 0.5f;
        // Start is called before the first frame update
        void Start()
        {
            if (rigidbodyBox != null)
            {
                for (int z = 0; z < gridNum.z; z++)
                {
                    for (int y = 0; y < gridNum.y; y++)
                    {
                        for (int x = 0; x < gridNum.x; x++)
                        {
                            GameObject go = Instantiate(rigidbodyBox);
                            go.transform.position = new Vector3(x, y + 50, z) * space - gridNum * space * 0.5f;
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
