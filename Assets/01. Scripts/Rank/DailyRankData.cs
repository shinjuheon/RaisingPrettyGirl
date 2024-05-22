using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


namespace gunggme
{
    public class DailyRankData : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textRank;
        [SerializeField]
        private TextMeshProUGUI textNickName;
        [SerializeField]
        private TextMeshProUGUI textCombat;
        [SerializeField]
        private TextMeshProUGUI textIndex;

        int rank;
        string nickName;
        int combatScore;
        int oneToOneObjIndex;
        DailyRankLoad dailyRankLoad;

        private void Awake()
        {
            dailyRankLoad = FindObjectOfType<DailyRankLoad>();
        }

        public int OneToOneObjIndex
        {
            set
            {
                if (value <= Constants.MAX_RANK_LIST)
                {
                    oneToOneObjIndex = value;
                    textIndex.text = oneToOneObjIndex.ToString();
                }
            }
            get => oneToOneObjIndex;
        }
        public int Rank
        {
            set
            {
                if (value <= Constants.MAX_RANK_LIST)
                {
                    rank = value;
                    textRank.text = rank.ToString();
                }
                else
                {
                    textRank.text = "순위에 없음";
                }
            }
            get => rank;
        }

        public string NickName
        {
            set
            {
                nickName = value;
                textNickName.text = nickName;
            }
            get => nickName;
        }

        public int Score
        {
            set
            {
                combatScore = value;
                textCombat.text = combatScore.ToString();
            }
            get => combatScore;
        }

        public void SetObjIndex()
        {
            dailyRankLoad.rankIndex = OneToOneObjIndex;
            Debug.Log($"dailyRankLoad.rankIndex : {dailyRankLoad.rankIndex}");
        }
    }
}

