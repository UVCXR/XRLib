using UnityEngine;

namespace WIFramework.Util
{
    public static partial class DebugExtractor
    {
        static uint call;
        static uint callCounter
        {
            get
            {
                if (call == uint.MaxValue)
                    call = 0;
                return call++;
            }
        }

        public static void LogError(this Debug d, string v)
        {
            Debug.LogError($"{callCounter} : {v}");
        }

        public static void Log(string format)
        {
            Debug.Log($"{callCounter} : {format}");
        }
    }
}