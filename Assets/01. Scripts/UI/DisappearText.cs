using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class DisappearText : MonoBehaviour
    {
        [SerializeField] private float _disappearTime;

        private void OnEnable()
        {
            Invoke(nameof(Disappear), _disappearTime);
        }

        void Disappear()
        {
            gameObject.SetActive(false);
        }
    }
}
