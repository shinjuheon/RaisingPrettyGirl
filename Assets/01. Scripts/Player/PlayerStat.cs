using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace gunggme
{
    public class 
        PlayerStat : MonoBehaviour
    {
        public delegate void OnEXPUp();
        public OnEXPUp OnUpdateEXP;

        [Header("EXP/Level")]
        [SerializeField] private float _curEXP;
        [SerializeField] private float _maxEXP = 100;
        [SerializeField] private int _level;
        [SerializeField] private int _statPoint;
        [Header("Stat")]
        [SerializeField] private int _dmg;
        [SerializeField] private int _dex;
        [SerializeField] private int _magicDef;
        [SerializeField] private int _hp;


        [Header("Components")]
        private PlayerEquipment _playerEquipment;
        private PetSystem _petSystem;
        private Player _player;

        // ex[/;eve;
        public float CurEXP => _curEXP;
        public float MaxEXP => _maxEXP;
        public int Level => _level;
        
        public float Cri => _playerEquipment.GetCriV();
        
        // Stat
        public int Dmg => (_dmg + _playerEquipment.GetDamage() + (_petSystem.CurrentPet ? _petSystem.CurrentPet.AtkBonus : 0)) * (_player.GodModeTime > 0 ? 10 : 1);
        public int DmgLevel => _dmg;
        
        public int Dex => _dex;

        public int Combat = 0;
        
        public int StatPoint => _statPoint;

        public int MagicDef
        {
            get
            {
                int mdef = _magicDef;
                if (_playerEquipment.EquipNecklace != null && _playerEquipment.EquipNecklace.ItemID != 0)
                {
                    mdef += _playerEquipment.EquipNecklace.Enforce;
                }
                foreach (var ring in _playerEquipment.EquipRingItems)
                {
                    if (ring != null && ring.ItemID != 0)
                    {
                        mdef += ring.Enforce;
                    }
                }
                foreach (var earring in _playerEquipment.EquipEarringItems)
                {
                    if (earring != null && earring.ItemID != 0)
                    {
                        mdef += earring.Enforce;
                    }
                }

                return mdef;
            }
        }

        public int MDefLevel => _magicDef;

        public int Hp
        {
            get
            {
                var tempHP = _petSystem.CurrentPet == null
                    ? (_hp * 50) + 80
                    : _petSystem.CurrentPet.HpBonus +
                      ((_hp * 50) + 80);
                // Debug.Log((_hp * 500) + 2000);
                return tempHP + (20 * _level);
            }
        }

        public int HPLv => _hp;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _playerEquipment = this.GetComponent<PlayerEquipment>();
            _petSystem = this.GetComponent<PetSystem>();
        }

        private void Start()
        {
            Debug.Log($"SaveManager.Instance.LevelStatData.CurLv : {SaveManager.Instance.LevelStatData.CurLv}");
            Debug.Log($"SaveManager.Instance.LevelStatData.CurEXP : {SaveManager.Instance.LevelStatData.CurEXP}");
            Debug.Log($"SaveManager.Instance.LevelStatData.SP : {SaveManager.Instance.LevelStatData.SP}");
            Debug.Log($"SaveManager.Instance.LevelStatData.Dmg : {SaveManager.Instance.LevelStatData.Dmg}");
            Debug.Log($"SaveManager.Instance.LevelStatData.Dex : {SaveManager.Instance.LevelStatData.Dex}");
            Debug.Log($"SaveManager.Instance.LevelStatData.MDef : {SaveManager.Instance.LevelStatData.MDef}");
            Debug.Log($"SaveManager.Instance.LevelStatData.HP : {SaveManager.Instance.LevelStatData.HP}");

            LoadData(SaveManager.Instance.LevelStatData.CurLv, SaveManager.Instance.LevelStatData.CurEXP,
                SaveManager.Instance.LevelStatData.SP, SaveManager.Instance.LevelStatData.Dmg,
                SaveManager.Instance.LevelStatData.Dex, SaveManager.Instance.LevelStatData.MDef,
                SaveManager.Instance.LevelStatData.HP);

            Debug.Log("PlayerStatLoadData 완료");
            
            _player.SetHP();
            Debug.Log("_player.SetHP();");
            UpLevel(0);
            Debug.Log("UpLevel(0);");

        }

        public int UpdateCombat()
        {
            Combat = Dex + MagicDef + Hp + Dmg;

            return Combat;
        }

        public bool UpLevel(float exp)
        {
            if(_level == 1000) return true;
            
            _maxEXP = CalculateExperienceForLevel(Level);

            _curEXP += exp * (_petSystem.CurrentPet ? _petSystem.CurrentPet.ExpBonusPer : 1);
            if (_curEXP >= _maxEXP)
            {
                _level++;
                _curEXP -= _maxEXP;
                _statPoint++;
                _maxEXP = CalculateExperienceForLevel(Level);
                OnUpdateEXP?.Invoke();
                return true;
            }
            
            OnUpdateEXP?.Invoke();

            return false;
        }
        
        // 필요한 기본 경험치
        private int baseExperience = 100;
        // 경험치 증가율
        private float experienceMultiplier = 1.25f;
        
        int CalculateExperienceForLevel(int level)
        {
            // 경험치 공식 사용하여 직접 계산
            int experienceNeeded = Mathf.RoundToInt(baseExperience * Mathf.Pow(experienceMultiplier, level - 1));
            return experienceNeeded;
        }

        public void UpStat(int num)
        {
            if (_statPoint == 0) return;
            
            switch (num)
            {
                case 0:
                    if (_dmg == 100) return;
                    _dmg++;
                    break;
                case 1:
                    if (_dex == 100) return;
                    _dex++;
                    break;
                case 2:
                    if (_magicDef == 100) return;
                    _magicDef++;
                    break;
                case 3:
                    if (_hp == 100) return;
                    _hp++;
                    _player.SetHP();
                    break;
            }

            _statPoint--;
        }
        public void LoadData(int level, float exp, int SP, int dmg, int dex, int mdef, int hp)
        {
            _level = level;
            _curEXP = exp;
            _maxEXP = 100 * (_level * 1.25f);
            _statPoint = SP;
            _dmg = dmg;
            _dex = dex;
            _magicDef = mdef;
            _hp = hp;
            OnUpdateEXP?.Invoke();
        }
    }
    
    
}
