using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WIFramework
{
    [DefaultExecutionOrder(int.MinValue)]
    public partial class Raycaster : MonoBehaviour, ISingle
    {
        static PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
        static List<RaycastResult> results = new List<RaycastResult>();
        RaycastHit[] hitInfo = new RaycastHit[16];
        HashSet<Type> typeLayers = new HashSet<Type>();
        Dictionary<Type, Action<RaycastHit, Component>> onEnterEvent_TypeLayer = new();
        Dictionary<Type, Action<RaycastHit, Component>> onStayEvent_TypeLayer = new();
        Dictionary<Type, Action<RaycastHit, Component>> exitEvent_TypeLayer = new();
        Dictionary<Type, Action<RaycastHit, Component>> onClickEvent_TypeLayer = new();
        Dictionary<Type, Action<RaycastHit, Component>> onClickFirst = new();
        Dictionary<Type, Action<RaycastHit, Component>> onEnterFirst = new();
        Dictionary<Type, Action<RaycastHit, Component>> onStayFirst = new();
        Dictionary<Type, Action<RaycastHit, Component>> onExitFirst = new();

        public static bool isOverUI
        {
            get
            {
                pointerEvent.position = Input.mousePosition;
                EventSystem.current.RaycastAll(pointerEvent, results);
                return results.Count > 0;
            }
        }

        int hitCount;
        void Rayfire()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Physics.Raycast(ray, out singleHit, Mathf.Infinity);
            hitCount = Physics.RaycastNonAlloc(ray, hitInfo, Mathf.Infinity);
        }

        bool onClick;
        void Update()
        {
            onClick = Input.GetMouseButtonDown(0);
            Rayfire();
            SingleCasting();
            MultiCasting();
        }
        Dictionary<Type, RaycastHit> firstHit = new();
        Dictionary<Type, RaycastHit> tempFirstHit = new();
        Dictionary<Transform, RaycastHit> fth = new();
        void SingleCasting(RaycastHit hitInfo)
        {
            fth.TryAdd(hitInfo.transform, hitInfo);

            foreach (var tl in typeLayers)
            {
                SingleCasting(hitInfo, tl);
            }
        }

        void SingleCasting(RaycastHit hitInfo, Type tl)
        {
            if (tempFirstHit.ContainsKey(tl))
            {
                return;
            }

            if (!hitInfo.transform.TryGetComponent(tl, out var value))
                return;

            if (firstHit.Remove(tl, out var prev))
            {
                if (prev.transform == hitInfo.transform)
                {
                    //Debug.Log($"OnStayFirst : {prev.transform.name}");
                    EventInvoke(onStayFirst, tl, hitInfo, value);
                }
                else
                {
                    //Debug.Log($"OnExitFirst : {prev.transform.name}");
                    EventInvoke(onExitFirst, tl, prev, prev.transform.GetComponent(tl));

                    //Debug.Log($"OnEnterFirst : {hitInfo.transform.name}");
                    EventInvoke(onEnterFirst, tl, hitInfo, value);
                    fth.Remove(prev.transform);
                }
            }
            else
            {
                //Debug.Log($"OnEnterFirst : {hitInfo.transform.name}");

                EventInvoke(onEnterFirst, tl, hitInfo, value);
            }
            fth[hitInfo.transform] = hitInfo;
            tempFirstHit.Add(tl, hitInfo);
        }

        void SingleCasting()
        {
            //fth.Clear();
            tempFirstHit.Clear();
            for (int i = 0; i < hitCount; ++i)
            {
                SingleCasting(hitInfo[i]);
            }

            foreach (var f in firstHit)
            {
                //Debug.Log($"OnExitFirst :{f.Value.transform.name}");
                EventInvoke(onExitFirst, f.Key, f.Value, f.Value.transform.GetComponent(f.Key));
                fth.Remove(f.Value.transform);
                //tempFirstHit.Remove(f.Key);
            }
            firstHit.Clear();

            foreach (var p in tempFirstHit)
            {
                firstHit.Add(p.Key, p.Value);

                if (onClick)
                {
                    EventInvoke(onClickFirst, p.Key, p.Value, p.Value.transform.GetComponent(p.Key));
                }
            }

        }
        HashSet<Transform> hitTransform = new();
        HashSet<Transform> tempHit = new();
        Dictionary<Transform, RaycastHit> transformToHitinfo = new();
        void MultiCasting()
        {
            tempHit.Clear();
            for (int i = 0; i < hitCount; ++i)
            {
                var ht = hitInfo[i].transform;
                tempHit.Add(ht);
                transformToHitinfo.TryAdd(ht, hitInfo[i]);
                bool isStay = hitTransform.Remove(ht);

                foreach (var tl in typeLayers)
                {
                    if (!ht.TryGetComponent(tl, out var value))
                        continue;

                    if (onClick)
                    {
                        EventInvoke(onClickEvent_TypeLayer, tl, hitInfo[i], value);
                        //Debug.Log($"OnClick {tl} {value}");
                    }

                    if (!isStay)
                    {
                        //Debug.Log($"OnEnter {tl} {value}");
                        EventInvoke(onEnterEvent_TypeLayer, tl, hitInfo[i], value);
                    }
                    else
                    {
                        EventInvoke(onStayEvent_TypeLayer, tl, hitInfo[i], value);
                        //Debug.Log($"OnStay {tl} {value}");
                    }
                }
            }

            foreach (var h in hitTransform)
            {
                foreach (var tl in typeLayers)
                {
                    if (!h.TryGetComponent(tl, out var value))
                        continue;

                    EventInvoke(exitEvent_TypeLayer, tl, transformToHitinfo[h], value);

                    //Debug.Log($"OnExit {tl} {value}");
                }
                transformToHitinfo.Remove(h);
            }

            hitTransform.Clear();
            foreach (var p in tempHit)
            {
                hitTransform.Add(p);
            }
        }

        void EventInvoke(Dictionary<Type, Action<RaycastHit, Component>> eventTable, Type layer, RaycastHit hitInfo, Component value)
        {
            if (eventTable.TryGetValue(layer, out var action))
            {
                action?.Invoke(hitInfo, value);
            }
        }

        public bool IsFirstHit(Transform target, Type layer)
        {
            if (hitInfo.Length == 0)
                return false;
            return hitInfo[0].transform == target;
        }

        public void AddTypeLayer(Type t)
        {
            typeLayers.Add(t);
            typeLayers.RemoveWhere(t => t == null);
        }

        public void RemoveTypeLayer<T>()
        {
            typeLayers.Remove(typeof(T));
        }

        public void AddFirstEnter(Type layer, Action<RaycastHit, Component> action)
        {
            onEnterFirst.TryAdd(layer, null);

            onEnterFirst[layer] += action;
        }

        public void AddFirstExit(Type layer, Action<RaycastHit, Component> action)
        {
            onExitFirst.TryAdd(layer, null);

            onExitFirst[layer] += action;
        }

        public void AddFirstStay(Type layer, Action<RaycastHit, Component> action)
        {
            onStayFirst.TryAdd(layer, null);

            onStayFirst[layer] += action;
        }

        public void AddFirstClick(Type layer, Action<RaycastHit, Component> action)
        {
            onClickFirst.TryAdd(layer, null);

            onClickFirst[layer] += action;
        }

        public void AddEnterEvent(Type layer, Action<RaycastHit, Component> action)
        {
            onEnterEvent_TypeLayer.TryAdd(layer, null);

            onEnterEvent_TypeLayer[layer] += action;
        }

        public void AddExitEvent(Type layer, Action<RaycastHit, Component> action)
        {
            exitEvent_TypeLayer.TryAdd(layer, null);

            exitEvent_TypeLayer[layer] += action;
        }

        public void AddStayEvent(Type layer, Action<RaycastHit, Component> action)
        {
            onStayEvent_TypeLayer.TryAdd(layer, null);

            onStayEvent_TypeLayer[layer] += action;
        }

        public void AddClickEvent(Type layer, Action<RaycastHit, Component> action)
        {
            onClickEvent_TypeLayer.TryAdd(layer, null);

            onClickEvent_TypeLayer[layer] += action;
        }
    }
}