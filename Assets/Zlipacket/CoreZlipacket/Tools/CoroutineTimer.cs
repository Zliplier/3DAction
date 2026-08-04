using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Zlipacket.CoreZlipacket.Tools
{
    public class CoroutineTimer
    {
        protected MonoBehaviour owner;
        protected Coroutine co_Timer;
        public bool isRunning => co_Timer != null;
        public bool isPaused { get; protected set; } = true;

        public UnityAction onRunning;
        
        public CoroutineTimer(MonoBehaviour owner)
        {
            this.owner = owner;
        }
        
        public virtual Coroutine StartTimer(float seconds, UnityAction callback = null)
        {
            if (isRunning)
                StopTimer();
            
            isPaused = false;
            co_Timer = owner.StartCoroutine(Running(seconds, callback));
            return co_Timer;
        }

        public virtual void StopTimer()
        {
            if (!isRunning)
                return;
                
            owner.StopCoroutine(co_Timer);
            isPaused = true;
            co_Timer = null;
        }
        
        public virtual void Pause()
        {
            if (isRunning)
                isPaused = true;
        }

        public virtual void Resume()
        {
            if (isRunning)
                isPaused = false;
        }

        protected virtual IEnumerator Running(float seconds, UnityAction callback = null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < seconds)
            {
                yield return null;
                elapsedTime += Time.deltaTime;

                while (isPaused)
                    yield return null;
                
                onRunning?.Invoke();
            }
            
            co_Timer = null;
            callback?.Invoke();
        }
    }
}