
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class ShopItemPannel : MonoBehaviour
    {
        [SerializeField] private ShopItem _shopItem;
        
        [SerializeField] private Image _itemImage;
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private TMP_Text _itemPriceText;
        [SerializeField] private TMP_Text _information;

        private GoodsManager _goodsManager;
        private PlayerStat _playerStat;

        private void Start()
        {
            _playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
            _goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
        }

        public void OpenPannel(Sprite itemSprite, string itemName, string itemPrice, string itemInformation, ShopItem item)
        {
            _itemImage.sprite = itemSprite;
            _itemName.text = itemName;
            _itemPriceText.text = itemPrice;
            _information.text = itemInformation;
            _shopItem = item;
            gameObject.SetActive(true);
        }

        public void BtnBuy()
        {
            if (_shopItem is TransformCard transformCard)
            {
                // 레벨 제한이 없는 경우 또는 플레이어의 레벨이 해당 카드의 레벨 제한 내에 있으면 구매 가능
                if (!((transformCard.MaxLevel == 0 && transformCard.MinLevel == 0) || (_playerStat.Level >= transformCard.MinLevel && _playerStat.Level <= transformCard.MaxLevel)))
                {
                    Debug.Log($"레벨 부족으로 인한 구매 실패\n레벨 조건 : {(transformCard.MinLevel >= _playerStat.Level && transformCard.MaxLevel <= _playerStat.Level)}");
                    return;
                }
            }
            switch (_shopItem.Type)
            {
                case GoodsType.Coin:
                    if (!_goodsManager.UseGold(_shopItem.Price))
                    {
                        return;
                    }
                    break;
                case GoodsType.Diamond:
                    if (!_goodsManager.UseDiamond(_shopItem.Price))
                    {
                        return;
                    }
                    break;
                case GoodsType.Money:
                    break;
            }
            Debug.Log("구매완료");
            _shopItem.ApplyReward();
           gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _shopItem = null;
        }
    }
}
