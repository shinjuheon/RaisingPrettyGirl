using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class BackgroundMove : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float speed;
        [SerializeField] float range;

        public bool IsMove = true;
        
        void Update()
        {
            if (IsMove)
            {
                transform.position += -Vector3.right * (speed * Time.deltaTime);
                if(transform.position.x < -range)
                {
                    transform.position = target.position + Vector3.right * range;
                }
            }
        }
    }
}
