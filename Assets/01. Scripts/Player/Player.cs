using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace gunggme
{
    [Serializable]
    public enum PlayerState
    {
        Idle,
        Move,
        Attack
    }
    
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerState _playerState;

        [SerializeField] private SpriteRenderer _weaponSR;
        public Transform _firePos;

        [Header("Attack Timer")] 
        [SerializeField] private float _tempAttackTime;
        [SerializeField] private float _maxAttackTime;

        public float MaxAttackTime
        {
            get
            {
                float attSpeed = _maxAttackTime / SaveManager.Instance.CurSkin.AttackSpeed;
                attSpeed /= _playerEquipment.EquipWeapon.GetAttSpeed() > 0 ? _playerEquipment.EquipWeapon.GetAttSpeed() : 1;
                return  Mathf.Round(attSpeed * 100) / 100;
            }
        }
        public GameObject _nearEnem;
        
        // 스킬 능력 변수
        public float GodModeTime { get; private set; }
        private GameObject _godModeObject;
        private Animator _godModeAnimator;
        
        // Components
        private Animator _animator;
        [HideInInspector] public PlayerStat _playerStat;
        private Damageable _damageable;
        private PlayerEquipment _playerEquipment;
        
        // 애니메이션 관련 변수
        private readonly int _moveAnimHash = Animator.StringToHash("Move");
        private readonly int _attackAnimHash = Animator.StringToHash("Attack");
        
        // 매니저들
        private BackgroundManager _bgManager;
        private MonsterManager _monsterManager;
        private UIManager _uiManager;
        [SerializeField]private StageManager _stageManager;

        private void Awake()
        {
            _playerEquipment = GetComponent<PlayerEquipment>();
            _stageManager = FindObjectOfType<StageManager>();
            _bgManager = FindObjectOfType<BackgroundManager>();
            _monsterManager = FindObjectOfType<MonsterManager>();
            _uiManager = FindObjectOfType<UIManager>();
            _godModeObject = transform.Find("God").gameObject;
            _godModeAnimator = _godModeObject.GetComponent<Animator>();
            
            _playerStat = this.GetComponent<PlayerStat>();
            _animator = GetComponent<Animator>();
            _damageable = this.GetComponent<Damageable>();
        }

        private void Start()
        {
            ChangeState(PlayerState.Move);
        }

        public void ChangeWeaponSprite(Sprite weaponSprite)
        {
            _weaponSR.sprite = weaponSprite;
        }

        private void Update()
        {
            if (_nearEnem && !_nearEnem.activeSelf)
            {
                _nearEnem = null;
            }
            
            switch (_playerState)
            {
                case PlayerState.Attack:
                    FindEnemy();
                    Attack();
                    break;
            }

            if (GodModeTime > 0)
            {
                GodModeTime -= Time.deltaTime;
                if (GodModeTime <= 0f)
                {
                    _godModeAnimator.SetBool("isGod", false);
                    _godModeObject.SetActive(false);
                }
            }
        }

        public void ChangeState(PlayerState state)
        {
            _playerState = state;
            switch (_playerState)
            {
                case PlayerState.Idle:
                    break;
                case PlayerState.Move:
                    StartCoroutine(Coroutine_Move());
                    break;
            }
        }

        void Attack()
        {
            if (_nearEnem == null) return;
            
            if (_tempAttackTime > 0)
            {
                _tempAttackTime -= Time.deltaTime;
            }
            else
            {
                _animator.SetTrigger(_attackAnimHash);
                _tempAttackTime = MaxAttackTime;
            }
        }

        /// <summary>
        /// 총알 발사
        /// </summary>
        void Shot()
        {
            if (_nearEnem == null)
            {
                _animator.SetTrigger("Idle");
                return;
            }

            float ranCri = Random.Range(0f, 100f);
            float dmg = _playerStat.Dmg / Random.Range(1.00f, 2.00f);
            if (ranCri <= _playerStat.Cri)
            {
                dmg *= 2;
            }
            SoundManager.Instance.PlaySound(SoundType.VFX, 0);
            GameObject temp = PoolManager.Instance.Get(0);
            temp.GetComponent<Bullet>().SetTarget(_nearEnem.transform, Mathf.RoundToInt(dmg));
            temp.GetComponent<Bullet>().SetSprite(_playerEquipment.EquipWeapon.Rarity);
            temp.transform.position = _firePos.position;
        }

        void FindEnemy()
        {
            if (!_monsterManager.IsAllDone())
            {
                _stageManager.PlayerClearConditionMet();
                Debug.Log("스테이지 클리어");
                ChangeState(PlayerState.Move);
                return;
            }
            if (_monsterManager.RemainEnemy.Count == 0)
                return;

            Func<Func<GameObject, float>, IOrderedEnumerable<GameObject>> orderBy = _monsterManager.RemainEnemy.OrderBy;
            _monsterManager.RemainEnemy = orderBy(
                x => Math.Abs(transform.position.x - x.transform.position.x)
                ).ToList();

            _nearEnem = _monsterManager.RemainEnemy[0];
        }

        private readonly WaitForSeconds _wait1Sec = new(1f);

        IEnumerator Coroutine_Move()
        {
            _bgManager.MoveBackground();
            _animator.SetBool(_moveAnimHash, true);
            yield return _wait1Sec;
            _bgManager.StopBackground();
            _animator.SetBool(_moveAnimHash, false);
            _damageable.HealHp(_playerStat.Hp/*Mathf.RoundToInt((_playerStat.Hp / 3 +
                                                 (SaveManager.Instance.CurPet == null ? 0 : SaveManager.Instance.CurPet.HealBonus))
                                                * (SaveManager.Instance.CurPet == null ? 1 : SaveManager.Instance.CurPet.HealBonusPer))*/);
            _uiManager.UpdateHpSlider();
            
            if (_stageManager.CurrentFloor == 100) // dragon
            {
                _monsterManager.SpawnEnemy("dragon");
            }
            else if (_stageManager.CurrentFloor % 10 == 0) // boss
            {
                _monsterManager.SpawnEnemy("boss");
            }
            else
            {
                  _monsterManager.SpawnEnemy("normal");
            }

            ChangeState(PlayerState.Attack);
        }

        public void SetHP()
        {
            // Debug.Log(_playerStat.Hp);
            _damageable.SetHP(_playerStat.Hp);
            _uiManager.UpdateHpSlider();
        }

        public void GetHeal(int heal)
        {
            _damageable.HealHp(heal);
        }

        public void GetDamage(int dmg, Enemy enemy)
        {
            // 방어스탯 1당 0.5% 피해감소
            // Debug.Log($"{dmg} 피해감소: {(_playerStat.MagicDef * 0.5f) * 0.01f}");
            if (GodModeTime > 0)
                dmg = 0;

            dmg -= Mathf.RoundToInt((_playerStat.MagicDef * 0.5f) * 0.01f * dmg);
            
            if (_damageable.IsDamage(dmg))
            {
                Debug.Log("Player Dead");
                 _damageable.SetHP(_playerStat.Hp);
                _monsterManager.DeSpawnEnem();
                _stageManager.SetStage(_stageManager.CurrentStage, 1);
                ChangeState(PlayerState.Move);
            }
            _uiManager.UpdateHpSlider();
        }

        public void SetGodModTime()
        {
            GodModeTime = 10f;
            _godModeObject.SetActive(true);
            _godModeAnimator.SetBool("isGod", true);
        }
    }
}
