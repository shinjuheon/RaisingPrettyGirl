using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace gunggme
{
    public class EnforceUI : MonoBehaviour
    {
        public delegate void OnItemEnforce();
        public OnItemEnforce OnEnforce;
        
        [SerializeField] private List<Item> _enforeList;

        [SerializeField] private Transform _slotPrt;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private TMP_Text _needScrollText;
        [SerializeField] private TMP_Text _needGoldText;

        [SerializeField] private GoodsManager _goodsManager;
        private EnforceInventoryUI _enforceInventory;
        private Inventory _inventory;
        private PlayerEquipment _playerEquipment;

        private int _needScrolls = 0;
        private int _needGold = 0;

        private void Awake()
        {
            _playerEquipment = GameObject.Find("Player").GetComponent<PlayerEquipment>();
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _enforceInventory = GetComponent<EnforceInventoryUI>();
            _goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
            _enforeList = new List<Item>();
            SlotUpdate();
            OnEnforce += SlotUpdate;
        }

        private void OnEnable()
        {
            _enforeList.Clear();
            _needScrolls = 0;
            SlotUpdate();
        }

        public void AddItem(Item item)
        {
            if (_enforeList.Find(i => (i.HashCode == item.HashCode)) != null)
            {
                Debug.Log("이미 올라간 아이템입니다.");
                return;
            }

            _enforeList.Add(item);
            
            SlotUpdate();
        }
        
        public void RemoveItem(Item item)
        {
            _enforeList.Remove(_enforeList.Find(i => (i.HashCode == item.HashCode)));
            
            SlotUpdate();
        }

        void SlotUpdate()
        {
            foreach (Transform child in _slotPrt)
            {
                Destroy(child.gameObject);
            }

            foreach (Item item in _enforeList)
            {
                GameObject temp = PoolManager.Instance.Get(2, _slotPrt);
                InventorySlotUI slotUI = temp.GetComponent<InventorySlotUI>();
                slotUI.AddItem(item, SlotUIType.Enforce, this);
            }

            _needScrolls = _enforeList.Sum(i => i.Enforce + 1);
            _needGold = _enforeList.Sum(i => (i.Enforce * 100000) + 100000);
            _needScrollText.color = _needScrolls > _goodsManager.ScrollCnt ? Color.red : Color.black;
            _needGoldText.color = _needGold > _goodsManager.GoldCnt ? Color.red : Color.black;
            _needScrollText.text = _needScrolls + " 필요";
            _needGoldText.text = _needGold + " 필요";
        }

        public void Btn_Enforce()
        {
            if (_enforeList.Count == 0)
                return;
            if (!_goodsManager.UseScroll(_needScrolls) && !_goodsManager.UseGold(_needGold))
            {
                Debug.Log("스크롤이 부족합니다.");
                return;
            }

            for (int i = _enforeList.Count - 1; i >= 0; i--)
            {
                if (_enforeList[i].TryEnforce())
                {
                    if (_enforeList[i] != null && _inventory.GetItems().Find(im => im.HashCode == _enforeList[i].HashCode) != null)
                    {
                        _inventory.GetItems().Find(im => im.HashCode == _enforeList[i].HashCode).Enforce = _enforeList[i].Enforce;
                    }
                    else if(_enforeList[i] != null &&_playerEquipment.EquipNecklace != null && _playerEquipment.EquipNecklace.HashCode == _enforeList[i].HashCode)
                    {
                        _playerEquipment.EquipNecklace.Enforce =  _enforeList[i].Enforce;
                    }
                    else if (_enforeList[i] != null && _playerEquipment.EquipEarringItems.Find(im => im.HashCode == _enforeList[i].HashCode) != null)
                    {
                        _playerEquipment.EquipEarringItems.Find(im => im.HashCode == _enforeList[i].HashCode).Enforce = _enforeList[i].Enforce;
                    }
                    else if (_enforeList[i] != null && _playerEquipment.EquipRingItems.Find(im => im.HashCode == _enforeList[i].HashCode) != null)
                    {
                        _playerEquipment.EquipRingItems.Find(im => im.HashCode == _enforeList[i].HashCode).Enforce = _enforeList[i].Enforce;
                    }
                }
                else
                {
                    if ( _enforeList[i] != null && _inventory.GetItems().Find(im => im.HashCode == _enforeList[i].HashCode) != null)
                    {
                        _inventory.RemoveItem(_enforeList[i]);
                    }
                    else if( _enforeList[i] != null && _playerEquipment.EquipNecklace != null && _playerEquipment.EquipNecklace.HashCode == _enforeList[i].HashCode)
                    {
                        _playerEquipment.EquipNecklace = null;
                    }
                    else if ( _enforeList[i] != null && _playerEquipment.EquipEarringItems.Find(im => im.HashCode == _enforeList[i].HashCode) != null)
                    {
                        _playerEquipment.EquipEarringItems[_playerEquipment.EquipEarringItems.FindIndex(im => im.HashCode == _enforeList[i].HashCode)] = new EarringItem();
                    }
                    else if ( _enforeList[i] != null && _playerEquipment.EquipRingItems.Find(im => im.HashCode == _enforeList[i].HashCode) != null)
                    {
                        _playerEquipment.EquipRingItems[_playerEquipment.EquipRingItems.FindIndex(im => im.HashCode == _enforeList[i].HashCode)] = new RingItem();
                    }
                    
                    RemoveItem(_enforeList[i]);
                }
            }
            _enforceInventory.UpdateEnforceUI();
            OnEnforce?.Invoke();
        }
    }
}
