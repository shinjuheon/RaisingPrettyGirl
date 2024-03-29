using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace gunggme
{
    public class MonsterManager : MonoBehaviour
    {
        public bool IsLive = false;

        [SerializeField] private Transform[] _spawnTrans;
        
        [SerializeField] private int _remainEnemCount;
        public List<GameObject> RemainEnemy;

        [Header("Spawn")] 
        [SerializeField] private float _tempSpawnTime;
        [SerializeField] private float _maxSpawnTime;

        private Player _player;
        private StageManager _stageManager;

        private void Start()
        {
            _stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
            _player = GameObject.Find("Player").GetComponent<Player>();
        }

        public void SpawnEnemy(string enemyType)
        {
            IsLive = true;
            _tempSpawnTime = _maxSpawnTime;

#if UNITY_EDITOR
            _remainEnemCount = 7;
#endif
#if !UNITY_EDITOR
            _remainEnemCount = enemyType == "normal" ? Random.Range(3, 7) : 1;
#endif
        }
        

        public bool IsAllDone()
        {
            return IsLive;
        }

        private void Update()
        {
            if (IsLive && _remainEnemCount != 0)
            {
                SpawningEnemy();
            }
            
            if (_remainEnemCount == 0 && RemainEnemy.Count ==  0)
            {
                IsLive = false;
            }
        }

        void SpawningEnemy()
        {
            if (_tempSpawnTime > 0)
            {
                _tempSpawnTime -= Time.deltaTime;
                return;
            }
            _tempSpawnTime = _maxSpawnTime;
            _remainEnemCount--;
            Spawn();
        }

        public void DeSpawnEnem()
        {
            _tempSpawnTime = _maxSpawnTime;
            foreach (Transform enem in transform)
            {
                if (enem.gameObject.activeSelf)
                {
                    if (enem.TryGetComponent(out Enemy enemy))
                    {
                        enemy.Dead();
                    }
                }
            }
            IsLive = false;
        }

        void Spawn()
        {
            int hp = 0;
            float dmg = 0;
            GameObject temp = null;
            if (_stageManager.CurrentFloor == 100) // dragon
            {
                temp = PoolManager.Instance.Get(4, transform);
                hp = ((_stageManager.CurrentStage * 5) + (_stageManager.CurrentFloor * 1)) * 10;
                dmg = hp / 4f;
            }
            else if (_stageManager.CurrentFloor % 10 == 0) // boss
            {
                temp = PoolManager.Instance.Get(3, transform);
                hp = ((_stageManager.CurrentStage * 5) + (_stageManager.CurrentFloor * 1)) * 5;
                dmg = hp / 4f;
            }
            else // normal
            {
                temp = PoolManager.Instance.Get(1, transform);
                hp = (_stageManager.CurrentStage * 5) + (_stageManager.CurrentFloor * 1);
                dmg = hp / 4f;
            }
            temp.transform.position = _spawnTrans[Random.Range(0, _spawnTrans.Length)].position;
            temp.GetComponent<Enemy>().Init(this, hp, Mathf.RoundToInt(dmg));
            RemainEnemy.Add(temp.gameObject);
        }
    }
}
