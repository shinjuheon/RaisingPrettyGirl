using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class ShopItemSlotUI : MonoBehaviour, IUse
    {
        [SerializeField] private Image _slotImage;
        [SerializeField] public ShopItem _shopItem;
        [SerializeField] private TMP_Text _itemPrice;

        private UIManager _uiManager;
        private PlayerStat _playerStat;
        private Button _button;

        private void Awake()
        {
            _playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            _slotImage.sprite = _shopItem.itemSprite;

            // 버튼 클릭 이벤트 자동 연결
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(Use);
            }
        }

        public void Use()
        {
            _uiManager.OpenShopPanel(_slotImage.sprite, _shopItem.ItemName, _itemPrice.text, _shopItem.GetInformation(), _shopItem);
        }

        public void Buy()
        {
            _shopItem.ApplyReward();
        }
    }
}
