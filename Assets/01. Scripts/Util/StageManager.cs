using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private int _currentStage = 1;
        [SerializeField] private int _maximumStage = 1;
        [SerializeField] private int _currentFloor = 1;
        [SerializeField] private int _currentClearFloor = 1;
        
        public int MaximumStage => _maximumStage;
        public int CurrentStage => _currentStage;
        public int CurrentFloor => _currentFloor;
        
        [field: SerializeField] public bool IsUpside { get; private set; }

        private UIManager _uiManager;
        private MonsterManager _monsterManager;

        private void Start()
        {
            _monsterManager = GameObject.FindObjectOfType<MonsterManager>();
            _uiManager = FindObjectOfType<UIManager>();
            
            SetStage(SaveManager.Instance.StageCoupon.CurStage); // 서버에 저장된 CurStage를 넣어준다. // 맥스스테이지와 관계없이 진행하던 스테이지를 표시해준다.
            Debug.Log(SaveManager.Instance.StageCoupon.CurStage);

            _maximumStage = SaveManager.Instance.StageCoupon.MaxStage; // 최고로 달성한 스테이지

            _uiManager.SetStageText(_currentStage, _currentFloor);
        }

        // 클리어 조건 체크
        void CheckClearCondition()
        {
            if (_currentFloor > 100) // 스테이지 마지막은 100(드래곤) 꺠면 Player스크립트의 void FindEnemy()함수가 실행되고 FindEnemy()함수에 
            {                        // CheckClearCondition()함수가 실행된다. 
                if (IsUpside)
                {
                    _currentStage++;
                    _currentFloor = 1;
                    SaveManager.Instance.StageCoupon.CurFloor = 1;
                    _currentClearFloor = 0;
                    if (_currentStage >= SaveManager.Instance.StageCoupon.MaxStage)
                    {
                        SaveManager.Instance.StageCoupon.MaxStage++;
                        _maximumStage = SaveManager.Instance.StageCoupon.MaxStage;
                    }
                }
                else
                {
                    _currentFloor = 1;
                    _currentClearFloor = 0;
                }
                Debug.Log("Stage " + _currentStage + " reached!");
            }
        }

        public void SetStage(int stage) // SaveManager.Instance.StageCoupon.CurStage 값이 들어옴
        {
            _currentStage = stage;
            _currentFloor = SaveManager.Instance.StageCoupon.CurFloor;
            _uiManager.SetStageText(_currentStage, _currentFloor);
        }
        
        public void SetStage(int stage, int floor)
        {
            _maximumStage = SaveManager.Instance.StageCoupon.MaxStage;
            _currentStage = stage;
            _currentFloor = floor;
            _monsterManager.DeSpawnEnem();
            _uiManager.SetStageText(_currentStage, _currentFloor);
        }

        // 클리어한 층 수 증가
        void IncreaseClearedFloors()
        {
            _currentFloor++;
            SaveManager.Instance.StageCoupon.CurFloor++;
            CheckClearCondition();
            _uiManager.SetStageText(_currentStage, _currentFloor);
        }

        // 현재 상태 출력
        public void PrintCurrentStatus()
        {
            Debug.Log("Current Stage: " + _currentStage);
            Debug.Log("Current Floor: " + _currentFloor);
        }

        // 예시: 플레이어가 클리어 조건을 만족시키는 상황에서 호출되는 함수
        public void PlayerClearConditionMet()
        {
            IncreaseClearedFloors();
            PrintCurrentStatus();
        }
    }
}
