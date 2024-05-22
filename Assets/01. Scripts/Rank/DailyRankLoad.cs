using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using TMPro;

namespace gunggme
{
    public class DailyRankLoad : MonoBehaviour
    {
        [SerializeField]
        private GameObject rankDataPrefab; // 랭킹 정보 출력을 위한 UI 프리팹 원본
        [SerializeField]
        private Scrollbar scrollbar; // scrollbar의 value 설정(활성화될떄 1위가 보이도록)
        [SerializeField]
        private Transform rankDataParent; // scrollView의 Content 오브젝트
        [SerializeField]
        private DailyRankData myRankData; // 내 랭킹 정보를 출력하는 UI 게임오브젝트
        [SerializeField]
        public TextMeshProUGUI textOneToOne;

        private List<DailyRankData> rankDataList;

        public int rankIndex;

        private void Awake()
        {
            rankDataList = new List<DailyRankData>();

            // 1 ~ 10위 랭킹 출력을 위한 UI 오브젝트 생성
            for (int i = 0; i < Constants.MAX_RANK_LIST; i++)
            {
                GameObject clone = Instantiate(rankDataPrefab, rankDataParent);
                rankDataList.Add(clone.GetComponent<DailyRankData>());
            }
        }

        private void OnEnable()
        {
            // 1위 랭킹이 보이도록 scroll 값 설정
            scrollbar.value = 1;
            // 1위 ~ 50위 랭킹 정보 불러오기
            GetRankList();
            // 내 랭킹 정보 불러오기
            GetMyRank();
        }
        private void GetRankList()
        {
            // 1위 ~ 50위 랭킹 정보 불러오기
            Backend.URank.User.GetRankList(Constants.DAILY_RANK_UUID, Constants.MAX_RANK_LIST, callback =>
            {
                if (callback.IsSuccess())
                {
                    // Json 데이터 파싱 성공
                    try
                    {
                        Debug.Log($"랭킹 조회에 성공했습니다.{callback}");

                        LitJson.JsonData rankDataJson = callback.FlattenRows();

                        // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                        if (rankDataJson.Count <= 0)
                        {
                            // 1위 ~ 20위까지의 데이터를 빈 데이터로 설정
                            for (int i = 0; i < Constants.MAX_RANK_LIST; ++i)
                            {
                                SetRankData(rankDataList[i], i + 1, "-", 0 , 0);
                            }
                            Debug.LogWarning("랭킹 데이터가 존재하지 않습니다.");
                        }
                        else
                        {
                            int rankerCount = rankDataJson.Count;

                            // 랭킹 정보를 불러와 출력할 수 있도록 설정
                            for (int i = 0; i < rankerCount; ++i)
                            {
                                rankDataList[i].Rank = int.Parse(rankDataJson[i]["rank"].ToString());
                                rankDataList[i].Score = int.Parse(rankDataJson[i]["score"].ToString());
                                rankDataList[i].NickName = rankDataJson[i]["nickname"].ToString();
                                rankDataList[i].OneToOneObjIndex = rankDataList[i].Rank - 1;
                            }
                            // 만약 랭킹이 20위까지 존재하지 않을 경우 나머지 랭킹 정보는 빈 데이터로 설정
                            for (int i = rankerCount; i < Constants.MAX_RANK_LIST; ++i)
                            {
                                SetRankData(rankDataList[i], i + 1, "-", 0 , rankDataList[i].Rank -1);
                            }
                        }
                    }
                    // Json 데이터 파싱 실패
                    catch (System.Exception e)
                    {
                        // try - catch 에러 출력
                        Debug.LogError(e);
                    }
                }
                else
                {
                    // 서버접속에 실패했다면
                    // 1위 ~ 20위까지 데이터를 빈 데이터로 설정
                    for (int i = 0; i < Constants.MAX_RANK_LIST; ++i)
                    {
                        SetRankData(rankDataList[i], i + 1, "-", 0 , 0);
                    }

                    Debug.LogError($"랭킹 조회 중 오류가 발생했습니다.{callback}");
                }
            });

        }

        private void GetMyRank()
        {
            // 내 랭킹 정보 불러오기
            Backend.URank.User.GetMyRank(Constants.DAILY_RANK_UUID, callback =>
            {
                string nickname = SaveManager.Instance.UserInfo.nickname;
                Debug.Log($"내 랭킹 불러오기의 내 랭킹 이름 : {nickname}");
                if (callback.IsSuccess())
                {
                    // Json 데이터 파싱 성공
                    try
                    {
                        LitJson.JsonData rankDataJson = callback.FlattenRows();

                        // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                        if (rankDataJson.Count <=0)
                        {
                            // ["순위에 없음", "닉에임", 0]과 같이 출력
                            SetRankData(myRankData, 100000000, nickname, 0 , 0);

                            Debug.LogWarning("데이터가 존재하지 않습니다");
                        }
                        else
                        {
                            myRankData.Rank  = int.Parse(rankDataJson[0]["rank"].ToString());
                            myRankData.Score = int.Parse(rankDataJson[0]["score"].ToString());
                            myRankData.NickName = nickname;
                            Debug.Log($"내 랭킹을 불러오는데 성공했습니다.");
                        }
                    }
                    // 자신의 랭킹 정보 Json 데이터 파싱에 실패했을 경우
                    catch (System.Exception e)
                    {
                        // ["순위에 없음", "닉네임", 0]과 같이 출력 
                        SetRankData(myRankData, 100000000, nickname, 0 , 0);

                        Debug.LogError(e);
                    }
                }
                else
                {
                    // 자신의 랭킹 정보 데이터가 존재하지 않을 경우
                    if (callback.GetMessage().Contains("userRank"))
                    {
                        // ["순위에 없음", "닉에임", 0]과 같이 출력 
                        SetRankData(myRankData, 100000000, nickname, 0 , 0);
                    }
                }
            });
        }

        private void SetRankData(DailyRankData rankData, int rank, string nickname, int CombatScore, int objIndex)
        {
            rankData.Rank = rank;
            rankData.NickName = nickname;
            rankData.Score = CombatScore;
            rankData.OneToOneObjIndex = objIndex;
        }

        public void OneToOneButton()
        {
            textOneToOne.gameObject.SetActive(true);
            if (rankDataList[rankIndex].Score < myRankData.Score)
            {// 내 전투력이 상대의 전투력보다 높다면
                Debug.Log($"내 전투력이 더 높습니다. : 승리");
                textOneToOne.text = "전투에서 승리 했습니다.";
            }
            else if (rankDataList[rankIndex].Score == myRankData.Score)
            {// 상대와의 전투력이 똑같다면
                textOneToOne.text = "전투에서 비겼습니다.";
            }
            else
            {// 내 전투력이 상대의 전투력보다 낮다면
                Debug.Log($"내 전투력이 더 낮습니다. : 패배");
                textOneToOne.text = "전투에서 패배 했습니다.";
            }
        }
    }
}

