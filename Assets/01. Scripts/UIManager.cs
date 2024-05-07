using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace gunggme
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private ItemInformation _itemInformation;
        [SerializeField] private EnforceInventoryUI _eiUI;

        [SerializeField] private GameObject[] _contentsButtons;
        [SerializeField] private GameObject[] _menuButtons;
        [SerializeField] private GameObject[] _shopCategoryButtons;

        [Header("Stage")] 
        [SerializeField] private TMP_Text _stageText;

        [Header("StatText")] [SerializeField] private TMP_Text _statPointText;
        [SerializeField] private TMP_Text[] _statTexts;
        private PlayerStat _playerStat;

        [Header("Combat")]
        [SerializeField] GameObject oneToOne;
        [SerializeField] TextMeshProUGUI oneToOneText;

        [Header("Hp")]
        [SerializeField] private Damageable _playerDamageable;
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private TMP_Text _hpText;
        [Header("Level/EXP")]
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _expText;
        [SerializeField] private Slider _expSlide;

        [Header("Shop")] 
        [SerializeField] private ShopItemPannel _shopPanel;

        [Header("Goods")] 
        private GoodsManager _goodsManager;
        [SerializeField] private TMP_Text[] _goldTexts;
        [SerializeField] private TMP_Text[] _diamondTexts;
        [SerializeField] private TMP_Text[] _scrollTexts;

        [Header("ChangeSkin")]
        [SerializeField] private SkinPreview _skinPreview;

        [Header("Equipment")] 
        [SerializeField] private PlayerEquipment _playerEquipment;
        [SerializeField] private EquipmentInfoWindow _equipmentInfoWindow;
        [SerializeField] private EquipSlotUI _weaaponSlotUI;
        [SerializeField] private EquipSlotUI _NecklaceSlotUI;
        [SerializeField] private EquipSlotUI[] _ringSlotUis;
        [SerializeField] private EquipSlotUI[] _earringSlotUIs;

        [Header("ETC")] [SerializeField] private GameObject _dontUseContent;
        [SerializeField] private GameObject _quitObj;
        [SerializeField] private GameObject _dontHavePetTxt;

        private void Awake()
        {
            _goodsManager = GameObject.Find("GoodsManager").GetComponent<GoodsManager>();
            _playerStat = GameObject.Find("Player").GetComponent<PlayerStat>();
            _playerEquipment = GameObject.Find("Player").GetComponent<PlayerEquipment>();
            _playerDamageable = GameObject.Find("Player").GetComponent<Damageable>();
            _playerStat.OnUpdateEXP -= UpdateLevel;
            _playerStat.OnUpdateEXP += UpdateLevel;
            _goodsManager.UpdateGoods -= UpdateGoods;
            _goodsManager.UpdateGoods += UpdateGoods;
            
            _playerStat.OnUpdateEXP?.Invoke();
            _goodsManager.UpdateGoods?.Invoke();
            // Invoke(nameof(_goodsManager.UpdateGoods), 3.0f); 이게 되네
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _quitObj.SetActive(!_quitObj.activeSelf);
            }
        }

        public void Open_ItemInformation(Item item)
        {
            _itemInformation.Information(item);
        }

        public void Btn_OpenEnforce()
        {
            _eiUI.gameObject.SetActive(true);    
        }

        public void Btn_OpenUIs(int num)
        {
            switch (num)
            {
                case 1:
                    UpdateStatUI();
                    break;
                case 3:
                    OpenPetWindow();
                    
                    return;
                    break;
            }
            for (int i = 0; i < _contentsButtons.Length; i++)
            {
                _contentsButtons[i].SetActive(i == num);
            }
        }

        public void Btn_UpStat(int statType)
        {
            _playerStat.UpStat(statType);
            UpdateStatUI();
        }

        private void UpdateStatUI()
        {
            _statPointText.text = $"AP : {_playerStat.StatPoint}";
            _statTexts[0].text = $"LV {_playerStat.DmgLevel:D4}";
            _statTexts[1].text = $"LV {_playerStat.Dex:D4}";
            _statTexts[2].text = $"LV {_playerStat.MagicDef:D4}";
            _statTexts[3].text = $"LV {_playerStat.HPLv:D4}";
        }

        private void OpenPetWindow()
        {
            int petCount = SaveManager.Instance.havePets.Sum(t => t.PetDatas.Count);

            if (petCount == 0)
            {
                Debug.Log("Pet 0!");
                if (!_dontHavePetTxt.activeSelf)
                {
                    _dontHavePetTxt.SetActive(true);
                }
                return;
            }
            
            _contentsButtons[3].SetActive(true);
        }

        public void UpdateHpSlider()
        {
            _hpSlider.value = (float)_playerDamageable.CurHP / _playerDamageable.MaxHP;
            _hpText.text = $"{_playerDamageable.CurHP} / {_playerDamageable.MaxHP}";
        }

        public void OpenEquipment()
        {
            foreach (var menu in _contentsButtons)
                menu.SetActive(false);

            _weaaponSlotUI.transform.parent.gameObject.SetActive(true);
            _weaaponSlotUI.SetImage(_playerEquipment.EquipWeapon);
            _NecklaceSlotUI.SetImage(_playerEquipment.EquipNecklace);
            for (int i = 0; i < 2; i++)
            {
                _ringSlotUis[i].SetImage(_playerEquipment.EquipRingItems[i]);
                _earringSlotUIs[i].SetImage(_playerEquipment.EquipEarringItems[i]);
            }
        }

        public void ReLoadEquipment()
        {
            _weaaponSlotUI.SetImage(_playerEquipment.EquipWeapon);
            _NecklaceSlotUI.SetImage(_playerEquipment.EquipNecklace);
            for (int i = 0; i < 2; i++)
            {
                _ringSlotUis[i].SetImage(_playerEquipment.EquipRingItems[i]);
                _earringSlotUIs[i].SetImage(_playerEquipment.EquipEarringItems[i]);
            }
        }

        public void OpenEquipInformation(Item item)
        {
            _equipmentInfoWindow.OpenItemInfo(item);
        }

        public void OpenEquipInformation(Item item, int num)
        {
            _equipmentInfoWindow.OpenItemInfo(item, num);
        }
        
        public void Btn_OpenMenuUIs(int num)
        {
            for (int i = 0; i < _menuButtons.Length; i++)
            {
                _menuButtons[i].SetActive(i == num);
            }
        }

        public void Btn_CloseMenuUIs(int num)
        {
            for (int i = 0; i < _menuButtons.Length; i++)
            {
                if (i != num) continue;
                _menuButtons[i].SetActive(false);
                break;
            }
        }

        public void Btn_ShopUIButton(int num)
        {
            for (int i = 0; i < _shopCategoryButtons.Length; i++)
            {
                _shopCategoryButtons[i].SetActive(i == num);
            }
        }

        public void SetStageText(int stage, int floor)
        {
            _stageText.text = $"Stage {stage}-{floor}";
        }

        public void UpdateLevel()
        {
            _statPointText.text = $"AP : {_playerStat.StatPoint}";
            _levelText.text = $"LV.{_playerStat.Level}";
            _expText.text = $"{_playerStat.CurEXP:N0}/{_playerStat.MaxEXP:N0}";
            if (_playerStat.MaxEXP != 0)
            {
                _expSlide.value = _playerStat.CurEXP / _playerStat.MaxEXP;
            }
        }
        
        public void UpdateGoods()
        {
            foreach (var goldText in _goldTexts)
            {
                goldText.text = $"{_goodsManager.GoldString} 골드";
            }

            foreach (var diamondText in _diamondTexts)
            {
                diamondText.text = $"{_goodsManager.Diamond} 다이아";
            }

            foreach (var scrollText in _scrollTexts)
            {
                scrollText.text = $"{_goodsManager.Scroll} 강화서";
            }
        }

        public void DontUseContent()
        {
            if (!_dontUseContent.activeSelf)
            {
                _dontUseContent.gameObject.SetActive(true);
            }
        }
        
        public void OpenShopPanel(Sprite sprite, string itemName, string itemPrice, string itemInformation, ShopItem shopItem)
        {
            _shopPanel.OpenPannel(sprite, itemName, itemPrice, itemInformation, shopItem);
        }

        //스킨 변경 좌우버튼
        //index = -1 : 좌 / idnex = 1 : 우
        public void NextPreview(int index)
        {
            _skinPreview.NextPreview(index);
        }

        public void Btn_ChangeSkin()
        {
            GameObject.Find("Player").GetComponent<PlayerSkinSystem>().ChangeSkin(_skinPreview.curRarity, _skinPreview.curSkinIdx);
            _skinPreview.transform.parent.gameObject.SetActive(false);
        }

        public void OneToOneBtn()
        {
            oneToOne.SetActive(true);
            _playerStat.UpdateCombat();
            oneToOneText.text = _playerStat.Combat.ToString();
        }
    }
}
