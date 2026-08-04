using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Zlipacket.CoreZlipacket.Player.Input.InputMap;

namespace Zlipacket.CoreZlipacket.Player.Input
{
    [CreateAssetMenu(menuName = "Zlipacket/Player/Input Reader", fileName = "Input Reader")]
    public class SO_InputReader : ScriptableObject
    {
        private InputSystem_Actions inputSystem;
        
        public PlayerInputMap playerInputMap;
        public UIInputMap uiInputMap;

        private void OnEnable()
        {
            if (inputSystem == null)
                inputSystem = new InputSystem_Actions();
            
            if (playerInputMap == null)
            {
                playerInputMap = new PlayerInputMap(inputSystem);
            }
            if (uiInputMap == null)
            {
                uiInputMap = new UIInputMap(inputSystem);
            }

            playerInputMap.OnEnable();
            uiInputMap.OnEnable();
        }

        private void OnDisable()
        {
            if (inputSystem == null)
                return;

            playerInputMap?.OnDisable();
            uiInputMap?.OnDisable();
        }
    }
}