using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class Singletone<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        Debug.Log($"현재 씬에서 {typeof(T)} 가 존재하지 않습니다.");
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                this.CreateThisInstance();
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != null)
            {
                Debug.Log("현재 instance가 중복됩니다.\n" + _instance.name);
                Destroy(gameObject);
            }
        }

        public void CreateThisInstance()
        {
            Debug.Log(Instance.gameObject.name);
        }
    }
}

