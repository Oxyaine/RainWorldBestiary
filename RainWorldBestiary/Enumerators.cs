using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RainWorldBestiary
{
    internal static class Enumerators
    {
        private static readonly Dictionary<int, Iterator> RunningEnumerators = new Dictionary<int, Iterator>();
        private static int CurrentFreshID = 0;
        internal static void Update()
        {
            int[] keys = new int[RunningEnumerators.Count];
            RunningEnumerators.Keys.CopyTo(keys, 0);

            foreach (int value in keys)
                if (!RunningEnumerators[value].Progress())
                    RunningEnumerators.Remove(value);
        }

        internal static void ForceProgressEnumerators()
        {
            int[] keys = new int[RunningEnumerators.Count];
            RunningEnumerators.Keys.CopyTo(keys, 0);

            foreach (int value in keys)
                if (!RunningEnumerators[value].ForceProgress())
                    RunningEnumerators.Remove(value);
        }
        internal static void ForceProgressEnumerators(params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
                ForceProgressEnumerator(ids[i]);
        }
        internal static void ForceProgressEnumerator(int id)
        {
            if (id.Equals(-1))
                return;

            if (RunningEnumerators.TryGetValue(id, out Iterator enumerator))
                if (!enumerator.ForceProgress())
                    RunningEnumerators.Remove(id);
        }

        internal static void ForceCompleteEnumerator(int id)
        {
            if (id.Equals(-1))
                return;

            if (RunningEnumerators.TryGetValue(id, out Iterator enumerator))
            {
                while (enumerator.ForceProgress()) { }
                RunningEnumerators.Remove(id);
            }
        }
        internal static void ForceCompleteEnumerators(params int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
                ForceCompleteEnumerator(ids[i]);
        }

        internal static void CompleteEnumerator(IEnumerator enumerator)
        {
            while (enumerator.MoveNext()) ;
        }

        internal static int StartEnumerator(IEnumerator enumerator)
        {
            if (enumerator.MoveNext())
            {
                ++CurrentFreshID;
                RunningEnumerators.Add(CurrentFreshID, new Iterator(enumerator));
                return CurrentFreshID;
            }
            else return -1;
        }
        internal static void StopEnumerator(int id)
        {
            if (id.Equals(-1))
                return;

            RunningEnumerators.Remove(id);
        }
    }

    internal class Iterator
    {
        public IEnumerator Enumerator;
        public float HoldTimeSeconds = 0;

        public Iterator(IEnumerator enumerator)
        {
            Enumerator = enumerator;
        }
        public Iterator(IEnumerator enumerator, float holdTimeSeconds) : this(enumerator)
        {
            HoldTimeSeconds = holdTimeSeconds;
        }

        public bool Progress()
        {
            if (HoldTimeSeconds > 0f)
                HoldTimeSeconds -= Time.deltaTime;

            if (HoldTimeSeconds <= 0)
            {
                if (Enumerator.MoveNext())
                {
                    switch (Enumerator.Current)
                    {
                        case WaitTime waitTime:
                            HoldTimeSeconds = waitTime.HoldTimeSeconds;
                            break;
                    }
                    return true;
                }
                else return false;
            }

            return true;
        }
        public bool ForceProgress() => Enumerator.MoveNext();
    }

    internal struct WaitTime
    {
        public float HoldTimeSeconds;
        public WaitTime(float holdTimeSeconds)
        {
            HoldTimeSeconds = holdTimeSeconds;
        }
    }
}
