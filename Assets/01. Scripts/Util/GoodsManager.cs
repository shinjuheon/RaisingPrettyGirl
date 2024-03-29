using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class GoodsManager : MonoBehaviour
    {
        public delegate void OnUpdateGoods();

        public OnUpdateGoods UpdateGoods;
        
        [SerializeField] private int _gold;
        [SerializeField] private int _diamond;
        [SerializeField] private int _scroll;

        public int ScrollCnt => _scroll;
        public int GoldCnt => _gold;
        public int DiaCnt => _diamond;

        public string GoldString // Gold 값을 문자열로 변환하여 반환
        {
            get
            {
                if (_gold >= 100000000)
                    return (_gold / 100000000) + "억";
                if (_gold >= 10000)
                    return (_gold / 10000) + "만";
                
                return _gold.ToString();
            }
        }

        public string Diamond
        {
            get
            {
                if (_diamond >= 100000000)
                    return (_diamond / 100000000) + "억";
                if (_diamond >= 10000)
                    return (_diamond / 10000) + "만";
                
                return _diamond.ToString();
            }
        }

        public string Scroll
        {
            get
            {
                if (_scroll >= 100000000)
                    return (_scroll / 100000000) + "억";
                if (_scroll >= 10000)
                    return (_scroll / 10000) + "만";
                
                return _scroll.ToString();
            }
        }

        private void Start()
        {
            GetDiamond(SaveManager.Instance.ETCData.Diamond);
            GetGold(SaveManager.Instance.ETCData.Gold);
            GetScroll(SaveManager.Instance.ETCData.Scroll);
        }

        public bool UseGold(int use)
        {
            if (_gold < use)
            {
                return false;
            }

            _gold -= use;
            
            UpdateGoods?.Invoke();
            return true;
        }

        public void GetGold(int get)
        {
            _gold += get;
            UpdateGoods?.Invoke();
        }

        public bool UseDiamond(int use)
        {
            if (_diamond < use)
            {
                return false;
            }

            _diamond -= use;
            UpdateGoods?.Invoke();
            return true;
        }

        public void GetDiamond(int get)
        {
            _diamond += get;
            UpdateGoods?.Invoke();
        }

        public bool UseScroll(int use)
        {
            if (_scroll < use)
            {
                return false;
            }

            _scroll -= use;
            UpdateGoods?.Invoke();
            return true;
        }
        public void GetScroll(int get)
        {
            _scroll += get;
            UpdateGoods?.Invoke();
        }
    }
}
