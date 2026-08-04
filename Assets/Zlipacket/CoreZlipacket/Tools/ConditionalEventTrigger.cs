using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zlipacket.CoreZlipacket.Condition;

namespace Zlipacket.CoreZlipacket.Tools
{
    public class ConditionalEventTrigger : MonoBehaviour
    {
        public bool triggerOnStart = false;
        [Space]
        public List<ConditionEvent> conditionEvents = new();

        private void Start()
        {
            if (triggerOnStart)
                Trigger();
        }

        public void Trigger()
        {
            foreach (var conditionEvent in conditionEvents)
            {
                conditionEvent.Trigger();
            }
        }
    }

    [Serializable]
    public class ConditionEvent
    {
        public List<SO_Condition> conditions;
        [Space]
        public UnityEvent onTrigger;
        
        public bool CheckCondition()
        {
            foreach (var condition in conditions)
            {
                if (!condition.IsMet())
                    return false;
            }
            
            return true;
        }

        public void Trigger()
        {
            if (CheckCondition())
                onTrigger?.Invoke();
        }
    }
}