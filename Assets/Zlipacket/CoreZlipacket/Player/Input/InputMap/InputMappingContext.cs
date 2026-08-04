using System;
using UnityEngine;

namespace Zlipacket.CoreZlipacket.Player.Input.InputMap
{
    public abstract class InputMappingContext
    {
        protected InputSystem_Actions inputSystem;
        public InputMapType mapType;

        protected InputMappingContext(InputSystem_Actions inputSystem, InputMapType mapType)
        {
            this.inputSystem = inputSystem;
            this.mapType = mapType;
        }
        
        public abstract void OnEnable();
        public abstract void OnDisable();
        
        public abstract void SetMapEnable(bool enable);
    }

    [Serializable]
    public enum InputMapType
    {
        None, 
        Player, 
        UI
    }
}