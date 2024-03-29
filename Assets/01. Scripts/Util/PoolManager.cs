using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class PoolManager : Singletone<PoolManager>
    {
        // 프리팹들 보관할 변수
        public GameObject[] prefab;
    
        // 풀 담당을 하는 리스트들
        private List<GameObject>[] pools;

        protected override void Awake()
        {
            base.Awake();
            
            Init();
        }

        void Init()
        {
            pools = new List<GameObject>[prefab.Length];

            for (int i = 0; i < pools.Length; i++)
            {
                pools[i] = new List<GameObject>();
            }
        }
        
        /// <summary>
        /// 오브젝트 생성 함수
        /// </summary>
        /// <param name="index">배열안 오브젝트 인덱스</param>
        /// <returns>생성하는 오브젝트</returns>
        public GameObject Get(int index)
        {
            return Get(index, transform);
        }
        
        public GameObject Get(int index, Transform prt)
        {
            GameObject select = null;

            foreach (GameObject item in pools[index])
            {
                if (item == null)
                {
                    Init();
                    break;
                }
                
                if (!item.activeSelf)
                {
                    select = item;
                    select.transform.SetParent(prt);
                    select.gameObject.SetActive(true);
                    break;
                }
            }

            if (!select)
            {
                select = Instantiate(prefab[index], prt);
                pools[index].Add(select);
            }

            return select;
        }

        /// <summary>
        ///  현재 자식안에 있는 오브젝트들을 비활성화
        /// </summary>
        public void DeactivatedChileObj()
        {
            foreach (var pool in pools)
            {
                foreach (var p in pool)
                {
                    p.SetActive(false);
                }
            }
        }
    }
}
