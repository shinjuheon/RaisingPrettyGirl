using UnityEngine;
using BackEnd;
namespace gunggme
{
    public class DailyRankRegister : MonoBehaviour
    {

        public void Process(int newCombat)
        {
            UpdateMyRankData(newCombat);
        }

        private void UpdateMyRankData(int newCombat)
        {
            string rowInDate = string.Empty;

            // 랭킹 데이터를 업데이트 하려면 게임 데이터에서 사용하는 데이터의 InDate 값이 필요
            Backend.GameData.GetMyData(Constants.USER_DATA_TABLE, new Where(), callback =>
            {
                if (!callback.IsSuccess())
                {
                    Debug.LogError($"데이터 조희 중 문제가 발생했습니다.{callback}");
                    return;
                }

                Debug.Log($"데이터 조회에 성공했습니다.{callback}");

                if (callback.FlattenRows().Count > 0)
                {
                    rowInDate = callback.FlattenRows()[0]["inDate"].ToString();
                }
                else
                {
                    var bro2 = Backend.GameData.Insert(Constants.USER_DATA_TABLE);
                    Debug.Log("데이터가 존재하지 않습니다.");

                    if (bro2.IsSuccess() == false)
                    {
                        Debug.LogError("데이터 삽입 중 문제가 발생했습니다 : " + bro2);
                        return;
                    }

                    Debug.Log($"데이터 삽입에 성공했습니다. : {bro2}");

                    rowInDate = bro2.GetInDate();
                }

                Param param = new Param();
                param.Add("Combat", newCombat);

                Debug.Log("랭킹 삽입을 시도합니다.");

                Backend.URank.User.UpdateUserScore(Constants.DAILY_RANK_UUID, Constants.USER_DATA_TABLE, rowInDate, param, callback =>
                {
                    if (callback.IsSuccess())
                    {
                        Debug.Log($"랭킹 등록에 성공했습니다. : {callback}");
                    }
                    else
                    {
                        Debug.LogError($"랭킹 등록에 실패했습니다. : {callback}");
                    }
                });
            });

        }
    }
}
