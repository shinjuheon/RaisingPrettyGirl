using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class EquipmentInfoWindow : MonoBehaviour
    {
        [SerializeField] private Item _currentItem;

        [SerializeField] private int currentNum;
        private PlayerEquipment _playerEquipment;

        [Header("UI")] 
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _itemInfo;

        private void Awake()
        {
            _playerEquipment = GameObject.Find("Player").GetComponent<PlayerEquipment>();
        }

        public void OpenItemInfo(Item item)
        {
            if (item == null || item.ItemID == 0) return;
            _currentItem = item;
            currentNum = 0;

            _itemIcon.sprite = _currentItem.ToSprite();
            _itemName.text = _currentItem.ItemName;
            _itemInfo.text = _currentItem.ToString();
            
            gameObject.SetActive(true);
        }

        public void OpenItemInfo(Item item, int slotNum)
        {
            if (item == null || item.ItemID == 0) return;
            _currentItem = item;
            currentNum = slotNum;
            Debug.Log(item.ItemID);
            _itemIcon.sprite = _currentItem.ToSprite();
            _itemName.text = _currentItem.ItemName;
            _itemInfo.text = _currentItem.ToString();
            gameObject.SetActive(true);
        }

        public void Btn_DeEquip()
        {
            Debug.Log(_currentItem.ItemType);
            if (_currentItem.ItemType == 1)
            {
                _playerEquipment.DeEquipItem(_currentItem);
            }
            else if (_currentItem is WeaponItem)
            {
                
            }
            else if (_currentItem.ItemType == 2 || _currentItem.ItemType == 3)
            {
                
                _playerEquipment.DeEquipItem(_currentItem, currentNum);
            }
            gameObject.SetActive(false);
        }
    }
}
