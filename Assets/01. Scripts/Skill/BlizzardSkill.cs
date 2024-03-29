using System;
using System.Collections;
using System.Collections.Generic;
using gunggme;
using UnityEngine;

public class BlizzardSkill : MonoBehaviour
{
    public float fallTime;
    public Vector2 startPos;
    public Vector2 endPos;

    public IEnumerator DoSkill(MonsterManager mm, int dmg)
    {
        transform.position = startPos;
        gameObject.SetActive(true);
        
        float percent = 0f;
        while (percent < fallTime)
        {
            percent += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, endPos, percent / fallTime);
            yield return null;
        }

        for (var index = 0; index < mm.RemainEnemy.Count; index++)
        {
            if (mm.RemainEnemy[index].GetComponent<Enemy>().GetDamage(dmg))
            {
                index--;
            }
        }

        yield return new WaitForSeconds(3f);

        gameObject.SetActive(false);
    }
}
