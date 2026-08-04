using System;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.UI
{
    public class Billboard : MonoBehaviour
    {
        //private Vector3 positionOffset;
        public Camera targetCam;
        
        private void Start()
        {
            //positionOffset = transform.localPosition;
            
            if (targetCam == null)
                targetCam = Camera.main;
        }

        public void Show()
        {
            enabled = true;
        }

        public void Hide()
        {
            enabled = false;
        }
        
        private void LateUpdate()
        {
            //Ignore parent rotation. :)
            //transform.position = transform.parent.position + positionOffset;
            //Or just make the canvas center of the object.
            
            //Look At Player.
            transform.LookAt(transform.position + targetCam.transform.rotation * Vector3.forward, targetCam.transform.rotation * Vector3.up);
        }
    }
}