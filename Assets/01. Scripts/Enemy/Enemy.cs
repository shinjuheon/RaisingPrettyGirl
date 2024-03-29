using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace gunggme
{
    public class Enemy : MonoBehaviour
    {
        protected GoodsManager _goodsManager;

        [SerializeField] protected Transform _playerTrans;
        [SerializeField] protected PlayerStat _playerStat;
        [SerializeField] protected Player _player;
        [SerializeField] protected GameObject _stunEffect;
        [SerializeField] protected float _moveSpeed;
        [SerializeField] protected float _stopDistance;

        [SerializeField] protected int dmg;
        [SerializeField] protected float _tempAttackTime;
        [SerializeField] protected float _maxAttackTime;
        [SerializeField] private int _rewardGold;
        [SerializeField] private int _rewardScroll;
        [SerializeField] private float _rewardEXP;

        public float RewardExp
        {
            get
            {
                /*
                 * 체력은 monstermanager참고
                 * 공격력 : 최대체력 / 4
                 * 
                 * 체력 100
                 * 공격력 25
                 *
                 * 주는 경험치 : 원 경험치 * ?
                 */
                if (_stageManager == null)
                {
                    _stageManager = FindObjectOfType<StageManager>();
                }
                float stageNum = (_stageManager.CurrentStage <= 10 ? 1 : _stageManager.CurrentStage / 10);
                Debug.Log(stageNum + " 배 증가중");
                return _rewardEXP * (stageNum) * _stageManager.CurrentFloor ;
            } 
        }

        private Damageable _damageable;
        protected Animator _animator;
        private RewardSystem rewardSystem;

        private MonsterManager _monsterManager;
        private StageManager _stageManager;

        protected float _stunTime = 0;

        private void Awake()
        {
            _playerTrans = GameObject.Find("Player").transform;
            _player = _playerTrans.GetComponent<Player>();
            _playerStat = _playerTrans.GetComponent<PlayerStat>();
            _stunEffect = transform.Find("Stun").gameObject;
            rewardSystem = _playerTrans.GetComponent<RewardSystem>();
            _stageManager = FindObjectOfType<StageManager>();
            _goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
            _animator = transform.Find("Sprite").GetComponent<Animator>();
            _damageable = this.GetComponent<Damageable>();
            
            _stunEffect.SetActive(false);
        }

        public void Init(MonsterManager mm, int hp, int dmg)
        {
            _monsterManager = mm;

            _stunTime = 0f;
            this.dmg = dmg;
            _tempAttackTime = 0.2f;
            _damageable.SetHP(hp);
        }

        public void Dead()
        {
            if (_damageable.IsDamage(9999999))
            {
                try
                {
                    _monsterManager.RemainEnemy.Remove(gameObject);
                }
                catch (Exception e)
                {
                    _monsterManager.RemainEnemy.Remove(gameObject);
                }

                gameObject.SetActive(false);
            }
        }

        public bool GetDamage(int Dmg)
        {
            if (_damageable.IsDamage(Dmg))
            {
                try
                {
                    _monsterManager.RemainEnemy.Remove(gameObject);
                }
                catch (Exception e)
                {
                    _monsterManager.RemainEnemy.Remove(gameObject);
                }
                
                _stunEffect.SetActive(false);
                gameObject.SetActive(false);
                GetReward();
                return true;
            }

            return false;
        }
        
        public void GetReward()
        {
            _goodsManager.GetGold(_rewardGold);
            _goodsManager.GetScroll(_rewardScroll);

            _playerStat.UpLevel(RewardExp);
            
            if (Random.Range(0f, 100f) <= 0.5f)
            {
                rewardSystem.GetReward();
            }
        }

        public void SetStunTime()
        {
            _stunTime = 1.5f;
            _stunEffect.SetActive(true);
        }
    }
}
