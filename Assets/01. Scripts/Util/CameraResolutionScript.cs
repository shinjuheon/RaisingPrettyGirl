using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gunggme
{
    public class CameraResolutionScript : MonoBehaviour
    {
        void Start()
        {
            var camera = GetComponent<Camera>();
            var r = camera.rect;
            var scaleheight = ((float)Screen.width / Screen.height) / (9f / 16f);
            var scalewidht = 1f / scaleheight;
        
            if (scaleheight < 1f)
            {
                r.height = scaleheight;
                r.y = (1f - scaleheight) / 2f;
            }
            else
            {
                r.width = scalewidht;
                r.x = (1f - scalewidht) / 2f;
            }
        
            camera.rect = r;
        }
        
        void OnPreCull() => GL.Clear(true, true, Color.black);
    }
}
