using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class ItemInformation : MonoBehaviour
    {
        private Inventory _inventory;
        private PlayerEquipment _playerEquipment;
        private UIManager _uiManager;
        [SerializeField] private Item _curItem;

        [SerializeField] private Image _itemImage;
        [SerializeField] private TMP_Text _itemInfor;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private GameObject _slotObj;

        private void Start()
        {
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            _inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
            _playerEquipment = GameObject.Find("Player").GetComponent<PlayerEquipment>();
        }

        private void OnEnable()
        {
            _slotObj.SetActive(false);
        }

        public void Information(Item item)
        {
            _curItem = item;
            _itemName.text = $"{_curItem.ItemName}";
            _itemImage.sprite = item.ToSprite();
            _itemInfor.text = item.ToString();
            gameObject.SetActive(true);
        }

        public void Btn_Enforce()
        {
            _uiManager.Btn_OpenEnforce();
            SetNull();
            gameObject.SetActive(false);
        }

        public void Btn_Equipment()
        {
            Debug.Log(_curItem.GetType());
            
            if (_curItem is RingItem or EarringItem)
            {
                _slotObj.SetActive(true);
            }
            else if (_curItem is WeaponItem or NecklaceItem)
            {
                _playerEquipment.EquipItem(_curItem);
                SetNull();
                gameObject.SetActive(false);
            }
        }

        public void Btn_EquipmentRingOrEarring(int slot)
        {
            _playerEquipment.EquipItem(_curItem, slot);
            gameObject.SetActive(false);
        }

        void SetNull()
        {
            _curItem = null;
        }
    }
}
