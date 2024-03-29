using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace gunggme
{
    public enum SlotUIType 
    {
        Inventory,
        EnforceInventory,
        Enforce,
        EquipEnforceInventory
    }
    
    public class InventorySlotUI : MonoBehaviour
    {
        public Image _icon; // 아이콘 이미지 표시를 위한 Image 컴포넌트

        private Item _item; // 슬롯에 표시할 아이템 객체

        private UIManager _uiManager;
        private EnforceUI _enforce;
        public SlotUIType _type;
        [SerializeField] private TMP_Text _enforceText;
        [SerializeField] private TMP_Text _equipmentText;

        // 슬롯에 아이템을 추가합니다.
        public void AddItem(Item newItem, SlotUIType type, UIManager uiManager)
        {
            _item = newItem;
            _icon.sprite = _item.ToSprite(); // 아이콘 이미지 업데이트
            _uiManager = uiManager;
            _enforceText.text = $"+{_item.Enforce}";
            _type = type;
            _icon.enabled = true; // 아이콘 이미지 활성화
            if(type != SlotUIType.EquipEnforceInventory)
            {
                _equipmentText.gameObject.SetActive(false);
            }
        }

        public void AddItem(Item newItem, SlotUIType type)
        {
            _item = newItem;
            _icon.sprite = _item.ToSprite();
            _enforceText.text = $"+{_item.Enforce}";
            _type = type;
            if(type != SlotUIType.EquipEnforceInventory)
            {
                _equipmentText.gameObject.SetActive(false);
            }
        }
        
        public void AddItem(Item newItem, SlotUIType type, EnforceUI uiManager)
        {
            _item = newItem;
            _icon.sprite = _item.ToSprite(); // 아이콘 이미지 업데이트
            _enforce = uiManager;
            _enforceText.text = $"+{_item.Enforce}";
            _type = type;
            _icon.enabled = true; // 아이콘 이미지 활성화
            _equipmentText.gameObject.SetActive(SlotUIType.EquipEnforceInventory == type);
        }
        


        public void Btn_Click()
        {
            switch (_type)
            {
                case SlotUIType.Inventory:
                    _uiManager.Open_ItemInformation(_item);
                    break;
                case SlotUIType.EquipEnforceInventory:
                case SlotUIType.EnforceInventory:
                    _enforce.AddItem(_item);
                    break;
                case SlotUIType.Enforce:
                    _enforce.RemoveItem(_item);
                    break;
            }
        }
    }
}
