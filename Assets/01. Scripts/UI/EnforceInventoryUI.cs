using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class EnforceInventoryUI : MonoBehaviour
    {
        private Inventory _inventory;
        private UIManager _uiManager;

        [SerializeField] private Transform _slotPrt;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private EnforceUI _enforceUI;
        private PlayerEquipment _playerEquipment;

        private void Awake()
        {
            _playerEquipment = GameObject.Find("Player").GetComponent<PlayerEquipment>();
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _enforceUI = GetComponent<EnforceUI>();
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            Initialize();
        }

        private void OnEnable()
        {
            _uiManager.Btn_OpenUIs(0);
            UpdateEnforceUI();
        }

        // 인벤토리 UI를 초기화합니다.
        public void Initialize()
        {
            _inventory.OnItemChangedCallback -= UpdateEnforceUI; // 인벤토리 아이템 변경 시 UI 업데이트를 위한 콜백 등록
            _inventory.OnItemChangedCallback += UpdateEnforceUI; // 인벤토리 아이템 변경 시 UI 업데이트를 위한 콜백 등록
            _enforceUI.OnEnforce -= UpdateEnforceUI;
            _enforceUI.OnEnforce += UpdateEnforceUI;
            Debug.Log("초기화");
            Debug.Log(_playerEquipment != null);
            UpdateEnforceUI(); // 초기 UI 설정
        }

        // 인벤토리 UI를 업데이트합니다.
        public void UpdateEnforceUI()
        {
            // 기존의 슬롯들을 모두 제거
            foreach (Transform child in _slotPrt)
            {
                Destroy(child.gameObject);
            }
            
            if (!_inventory)
            {
                _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            }
            
            // 장비창에 있는 아이템에 대해 슬롯을 생성
            if (_playerEquipment.EquipNecklace != null && _playerEquipment.EquipNecklace.ItemID != 0)
            {
                GameObject slotGO = PoolManager.Instance.Get(2, _slotPrt);
                InventorySlotUI slot = slotGO.GetComponent<InventorySlotUI>();
                if (slot != null)
                {
                    slot.AddItem(_playerEquipment.EquipNecklace, SlotUIType.EquipEnforceInventory, _enforceUI); // 슬롯에 아이템 추가
                }
            }
            foreach (EarringItem item in _playerEquipment.EquipEarringItems)
            {
                if (item != null && item.ItemID != 0)
                {
                    GameObject slotGO = PoolManager.Instance.Get(2, _slotPrt);
                    InventorySlotUI slot = slotGO.GetComponent<InventorySlotUI>();
                    if (slot != null)
                    {
                        slot.AddItem(item, SlotUIType.EquipEnforceInventory, _enforceUI); // 슬롯에 아이템 추가
                    }
                }
            }
            foreach (RingItem item in _playerEquipment.EquipRingItems)
            {
                if (item != null && item.ItemID != 0)
                {
                    GameObject slotGo = PoolManager.Instance.Get(2, _slotPrt);
                    InventorySlotUI slot = slotGo.GetComponent<InventorySlotUI>();
                    if (slot != null)
                    {
                        slot.AddItem(item, SlotUIType.EquipEnforceInventory, _enforceUI); // 슬롯에 아이템 추가
                    }
                }
            }

            // 인벤토리에 있는 각 아이템에 대해 슬롯을 생성하여 UI에 추가
            foreach (Item item in _inventory.GetItems())
            {
                GameObject slotGo = PoolManager.Instance.Get(2, _slotPrt);
                InventorySlotUI slot = slotGo.GetComponent<InventorySlotUI>();
                if (slot != null)
                {
                    slot.AddItem(item, SlotUIType.EnforceInventory, _enforceUI); // 슬롯에 아이템 추가
                }
            }
            
            
        }

        public void Btn_Close()
        {
            _inventory.OnItemChangedCallback?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
