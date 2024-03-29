using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class EquipSlotUI : MonoBehaviour, IUse
    {
        [SerializeField] private Sprite _nullSprite;
        [SerializeField] private Image _slotImage;
        public Item CurrentItem;
        [field: SerializeField] public int CurSlot { get; private set; }

        private UIManager _uiManager;
        private Button _button;

        private void Start()
        {
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Use);
        }

        public void SetImage(Item changeItem)
        {
            CurrentItem = changeItem;
            
            if (CurrentItem == null || CurrentItem.ItemID == 0)
            {
                _slotImage.sprite = _nullSprite;
            }
            else
            {
                _slotImage.sprite = CurrentItem.ToSprite();
            }
        }

        public void Use()
        {
            if (CurrentItem is WeaponItem or NecklaceItem)
            {
                _uiManager.OpenEquipInformation(CurrentItem);
            }
            else if (CurrentItem is RingItem or EarringItem)
            {
                _uiManager.OpenEquipInformation(CurrentItem, CurSlot);
            }
        }
    }
}
