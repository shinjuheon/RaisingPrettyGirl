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
        
        private void Awake()
        {
            _playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
            _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
            _slotImage.sprite = _shopItem.itemSprite;
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
