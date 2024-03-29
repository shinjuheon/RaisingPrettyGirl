using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace gunggme
{

    public class InventoryUI : MonoBehaviour
    {
        [FormerlySerializedAs("slotPrefab")] public GameObject SlotPrefab; // 인벤토리 슬롯 프리팹
        [FormerlySerializedAs("slotsParent")] public Transform SlotsParent; // 슬롯들이 속할 부모 Transform
        [FormerlySerializedAs("capacityText")] public TMP_Text CapacityText; // 용량 텍스트 UI

        private Inventory _inventory; // 실제 인벤토리 객체
        private UIManager _uiManager;

        private void Start()
        {
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            Initialize(_inventory);
        }

        // 인벤토리 UI를 초기화합니다.
        public void Initialize(Inventory inventory)
        {
            this._inventory = inventory;
            inventory.OnItemChangedCallback += UpdateUI; // 인벤토리 아이템 변경 시 UI 업데이트를 위한 콜백 등록

            UpdateUI(); // 초기 UI 설정
        }

        // 인벤토리 UI를 업데이트합니다.
        private void UpdateUI()
        {
            // 기존의 슬롯들을 모두 제거
            foreach (Transform child in SlotsParent)
            {
                Destroy(child.gameObject);
            }

            // 인벤토리에 있는 각 아이템에 대해 슬롯을 생성하여 UI에 추가
            foreach (Item item in _inventory.GetItems())
            {
                GameObject slotGO = PoolManager.Instance.Get(2, SlotsParent);
                InventorySlotUI slot = slotGO.GetComponent<InventorySlotUI>();
                if (slot != null)
                {
                    slot.AddItem(item, SlotUIType.Inventory, _uiManager); // 슬롯에 아이템 추가
                }
            }

            // 용량 텍스트 업데이트
            CapacityText.text = "용량: " + _inventory.GetItems().Count + "/" + _inventory.InventorySpace;
        }
    }
}
