using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace gunggme
{
    public class Damageable : MonoBehaviour
    {
        [SerializeField] private int _curHP;
        [SerializeField] private int _maxHP;
        
        [SerializeField] private Transform _canvasParent;
        
        public int MaxHP => _maxHP;
        public int CurHP => _curHP;

        private void Awake()
        {
            _canvasParent = GameObject.Find("Damageed").transform;
        }

        private void OnEnable()
        {
            
        }

        public void SetHP(int hp)
        {
            _maxHP = hp;
            _curHP = hp;
            // Debug.Log("hpset");
        }

        public void HealHp(int heal)
        {
            _curHP += heal;
            if (_curHP > _maxHP)
                _curHP = _maxHP;
        }

        public bool IsDamage(int dmg)
        {
            _curHP -= dmg;
            TMP_Text damagedText = PoolManager.Instance.Get(5, _canvasParent).GetComponent<TMP_Text>();
            damagedText.text = $"{dmg}";
            // 현재 객체의 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));

// 스크린 좌표를 캔버스 내의 로컬 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasParent.GetComponent<RectTransform>(), screenPosition, Camera.main, out Vector2 anchoredPosition);

// 새로운 anchoredPosition 설정
            damagedText.rectTransform.anchoredPosition = anchoredPosition;
            if (_curHP <= 0)
            {
                return true;
            }
            return false;
        }
    }
}
