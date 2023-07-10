using System;
using System.Collections.Generic;
using UnityEngine;

namespace WIFramework
{
    public partial class Hooker
    {
        static Array codes;
        internal static void Initialize()
        {
            //codes = Enum.GetValues(typeof(KeyCode));
            PlayerLoopInterface.InsertSystemAfter(typeof(DataBinder), DataBinder.Update, typeof(UnityEngine.PlayerLoop.EarlyUpdate.UpdatePreloading));
            PlayerLoopInterface.InsertSystemAfter(typeof(Hooker), MonoTracking, typeof(UnityEngine.PlayerLoop.EarlyUpdate.UpdatePreloading));
            //PlayerLoopInterface.InsertSystemBefore(typeof(Hooker), DetectingKey, typeof(UnityEngine.PlayerLoop.PreUpdate.NewInputUpdate));
            //SetHook();
        }

        static Queue<MonoBehaviour> initializerList = new Queue<MonoBehaviour>();
        static Queue<MonoBehaviour> afterInitializerList = new Queue<MonoBehaviour>();
        static Queue<MonoBehaviour> registWatingQueue = new Queue<MonoBehaviour>();
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void MonoTracking()
        {
            while (registWatingQueue.Count > 0)
            {
                var curr = registWatingQueue.Dequeue();
                if (curr != null)
                {
                    if (WIManager.Regist(curr))
                        initializerList.Enqueue(curr);
                }
            }

            Initializing();
        }

        static void Initializing()
        {
            while (initializerList.Count > 0)
            {
                var mono = initializerList.Dequeue();
                mono.Initialize();
                //Debug.Log($"{mono.GetType().Name} Initialized");
                afterInitializerList.Enqueue(mono);
            }

            while (afterInitializerList.Count > 0)
            {
                var curr = afterInitializerList.Dequeue();
                curr.AfterInitialize();
                // Debug.Log($"{curr.GetType().Name} AfterInitialized");
            }
        }

        public static void RegistReady(MonoBehaviour target)
        {
            registWatingQueue.Enqueue(target);
        }

        static void DetectingKey()
        {
            foreach (KeyCode k in codes)
            {
                if (Input.GetKey(k))
                {
                    Posting(k, WIManager.getKeyActors);
                }
                if (Input.GetKeyDown(k))
                {
                    Posting(k, WIManager.getKeyDownActors);
                }
                if (Input.GetKeyUp(k))
                {
                    Posting(k, WIManager.getKeyUpActors);
                }
            }
        }
        static void Posting(KeyCode key, HashSet<IGetKey> actorList)


        {
            foreach (var actor in actorList)
            {
                if (actor is null)
                {
                    continue;
                }
                actor.GetKey(key);
            }
        }
        static void Posting(KeyCode key, HashSet<IGetKeyDown> actorList)
        {
            foreach (var actor in actorList)
            {
                if (actor is null)
                {
                    continue;
                }
                actor.GetKeyDown(key);
            }
        }
        static void Posting(KeyCode key, HashSet<IGetKeyUp> actorList)
        {
            foreach (var actor in actorList)
            {
                if (actor is null)
                {
                    continue;
                }
                actor.GetKeyUp(key);
            }
        }

    }
}
