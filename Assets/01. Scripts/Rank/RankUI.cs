using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace gunggme
{
    public class RankUI : MonoBehaviour
    {
        public GameObject rankUiObj;
        private DailyRankRegister dailyRankRegister;
        private PlayerStat playerStat;


        private void Start()
        {
            if (dailyRankRegister == null)
            {
                dailyRankRegister = FindObjectOfType<DailyRankRegister>();
            }

            if (playerStat == null)
            {
                playerStat = FindObjectOfType<PlayerStat>();
            }
        }

        public void RankObj()
        {
            rankUiObj.SetActive(!rankUiObj.activeSelf);
            Debug.Log($"rankUIObj Activeself : {rankUiObj.activeSelf}");
            if (rankUiObj.activeSelf) // 활성화 상태라면
            {
                Debug.Log($"dailyRankRegister : {dailyRankRegister}");
                Debug.Log($"playerStat : {playerStat}");
                int combat = playerStat.UpdateCombat(); // 이 부분인데
                Debug.Log($"Combat value: {combat}");
                dailyRankRegister.Process(combat);
                Debug.Log($"Combat 값을 이용해 랭킹삽입에 성공했습니다.{combat}");
            }
        }
    }
}
