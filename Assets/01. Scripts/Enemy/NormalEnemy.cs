using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace gunggme
{
    public class NormalEnemy : Enemy
    {
        private void Update()
        {
            if (_stunTime > 0)
            {
                _stunTime -= Time.deltaTime;
                if (_stunTime <= 0f)
                    _stunEffect.SetActive(false);
                return;
            }

            if (Mathf.Abs(transform.position.x - _playerTrans.position.x) > _stopDistance)
            {
                Move();
            }
            else
            {
                Attack();
            }
        }

        private void Move()
        {
            // 거리가 가까워질때가지
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(_playerTrans.position.x, transform.position.y, _playerTrans.position.z),
                _moveSpeed * Time.deltaTime);
        }

        private void Attack()
        {
            if (_tempAttackTime > 0)
            {
                _tempAttackTime -= Time.deltaTime;
                return;
            }

            _tempAttackTime = _maxAttackTime;

            _animator.SetTrigger("Attack");

            Invoke("DamagePlayer", 0.35f);
        }

        private void DamagePlayer()
        {
            Debug.Log("DamagePlayer");
            _player.GetDamage(dmg, this);
        }
    }
}
