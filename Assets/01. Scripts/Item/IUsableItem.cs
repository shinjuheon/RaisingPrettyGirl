using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public interface IUsableItem
    {
        /// <summary>
        /// 아이템 사용
        /// </summary>
        /// <returns>아이템 사용 여부</returns>
        bool Use();

        /// <summary>
        /// 장착하기
        /// </summary>
        /// <returns>장착 여부</returns>
        bool Equip();
    }
}
