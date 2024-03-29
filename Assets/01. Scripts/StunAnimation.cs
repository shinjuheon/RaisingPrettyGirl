using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunAnimation : MonoBehaviour
{
    public float rotateTime = 1f;
    public List<Transform> positionList;
    public int _curPos;

    private int CurPos
    {
        get => _curPos;
        set
        {
            _curPos = value;
            if (_curPos >= positionList.Count)
            {
                _curPos = 0;
            }
        }
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        for (int i = 0; i < positionList.Count; i++)
        {
            if (transform.position == positionList[i].position)
            {
                CurPos = i;
                break;
            }
        }

        Debug.Log(CurPos);
        transform.position = positionList[CurPos].position;
        StartCoroutine(TurnAnim());
    }

    private IEnumerator TurnAnim()
    {
        while (true)
        {
            Vector2 startPos = positionList[CurPos].position;
            CurPos++;
            Vector2 endPos = positionList[CurPos].position;
            float percent = 0f;

            while (percent < rotateTime)
            {
                percent += Time.deltaTime;
                transform.position = Vector2.Lerp(startPos, endPos, percent / rotateTime);
                
                yield return null;
            }
        }
    }
}
