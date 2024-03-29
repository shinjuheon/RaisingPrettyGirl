using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private BackgroundMove[] _bgMoves;
        
        public void StopBackground()
        {
            foreach (var bg in _bgMoves)
            {
                bg.IsMove = false; 
            }
        }

        public void MoveBackground()
        {
            foreach (var bg in _bgMoves)
            {
                bg.IsMove = true;
            }
        }
    }
}
