using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private int _dmg;
        public int Dmg => _dmg;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private Sprite[] _sprites;
        
        private Transform _target;
        private SpriteRenderer _spriteRenderer;

        [SerializeField] private int _maxPenetrate = 0;
        private int _curPenetrate = 0;
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetTarget(Transform target, int dmg)
        {
            SetTarget(target, dmg, Vector2.one * 0.3f);
        }
        public void SetTarget(Transform target, int dmg, Vector2 size)
        {
            _target = target;
            _dmg = dmg;
            _curPenetrate = 0;

            transform.localScale = size;
        }

        public void SetSprite(int spriteNum)
        {
            switch (spriteNum)
            {
                case 0:
                    _spriteRenderer.sprite = _sprites[spriteNum];
                    break;
                case 1:
                    _spriteRenderer.sprite = _sprites[spriteNum];
                    break;
                case 2:
                default:
                    _spriteRenderer.sprite = _sprites[2];
                    break;
            }
        }

        private void Update()
        {
            if (!_target || !_target.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _target.position, _moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy") || !other.TryGetComponent(out Enemy enemy) || _curPenetrate >= _maxPenetrate)
                return;
            _curPenetrate++;
            
            Debug.Log(other.name + "부딛힘");
            
            enemy.GetDamage(Dmg);

            if (_curPenetrate >= _maxPenetrate)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
