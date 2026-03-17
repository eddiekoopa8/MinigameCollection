using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Core
{
    public class Timer
    {
        float cur = 0.0f;
        float max;

        public float CurrentTick { get { return cur; } }
        public bool Reached { get { return cur >= max; } }

        bool start = true;

        public Timer(float Maximum = 0.0f)
        {
            max = Maximum;
        }

        public void SetMaximumInMilliseconds(int Maximum)
        {
            max = Maximum;
        }

        public void SetMaximumInSeconds(int Maximum)
        {
            max = Maximum * 1000;
        }

        public void Reset()
        {
            cur = 0.0f;
        }

        public void limit()
        {
            if (cur > max)
            {
                cur = max;
            }
        }
        public void Tick()
        {
            if (!start)
            {
                return;
            }
            cur += Time.deltaTime * 1000;
            limit();
        }
        public float GetMilliseconds()
        {
            return cur;
        }
        public int GetSeconds()
        {
            return Mathf.RoundToInt(cur / 1000);
        }
        public float GetCountdownMilliseconds()
        {
            return max - GetMilliseconds();
        }
        public int GetCountdownSeconds()
        {
            return Mathf.RoundToInt((max - GetMilliseconds()) / 1000);
        }
        public void FixedTick()
        {
            if (!start)
            {
                return;
            }
            cur += Time.fixedDeltaTime * 1000;
            limit();
        }
    }
}