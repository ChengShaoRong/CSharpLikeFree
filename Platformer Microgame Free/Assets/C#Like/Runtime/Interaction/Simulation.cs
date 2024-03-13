/*
 * Source from 'Platformer Microgame' : https://assetstore.unity.com/packages/templates/platformer-microgame-151055
 * I convert those classes into source can be used in hot update script.
 * Usage:
 * 1 Define a event class in hot update script:
        public class TestEvent
        {
            /// <summary>
            /// (Optional, if not exist this function, default precondition is true)
            /// Precondition for mothed 'Execute'
            /// </summary>
            public bool Precondition()
            {
                return true;
            }
            /// <summary>
            /// (Optional, if you don't need this, remve this function)
            /// This method is generally used to set references to null when required.
            /// It is automatically called by the Simulation when an event has completed.
            /// </summary>
            void Cleanup()
            {
                return true;
            }
            public void Execute()
            {
                UnityEngine.Debug.Log("Test");
            }
        }
 * 2 Raise a event in hot update script:
        TestEvent testEvent = Simulation.Schedule(typeof(TestEvent), 2f) as TestEvent;
 */
using CSharpLike.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace CSharpLike
{
    /// <summary>
    /// The Simulation class implements the discrete event simulator pattern.
    /// Events are pooled, with a default capacity of 4 instances.
    /// </summary>
    public static class Simulation
    {
        static HeapQueue<Event> eventQueue = new HeapQueue<Event>();
        static Dictionary<string, Stack<Event>> eventPools = new Dictionary<string, Stack<Event>>();

        /// <summary>
        /// Create a new event of type T and return it, but do not schedule it.
        /// </summary>
        static public Event New(Type type)
        {
            Stack<Event> pool;
            if (!eventPools.TryGetValue(type.FullName, out pool))
            {
                pool = new Stack<Event>(4);
                var e = new Event(type);
                pool.Push(e);
                eventPools[type.FullName] = pool;
            }
            if (pool.Count > 0)
                return pool.Pop();
            else
                return new Event(type);
        }
        static public Event New(SType type)
        {
            Stack<Event> pool;
            if (!eventPools.TryGetValue(type.FullName, out pool))
            {
                pool = new Stack<Event>(4);
                var e = new Event(type);
                pool.Push(e);
                eventPools[type.FullName] = pool;
            }
            if (pool.Count > 0)
                return pool.Pop();
            else
                return new Event(type);
        }

        /// <summary>
        /// Clear all pending events and reset the tick to 0.
        /// </summary>
        public static void Clear(bool bAll = false)
        {
            eventQueue.Clear();
            if (bAll)
            {
                eventPools.Clear();
                events.Clear();
                onExecutes.Clear();
            }
        }

        static public object Schedule(Type type, float tick = 0f)
        {
            //Debug.LogError($"Schedule Type {type.FullName}");
            Event ev = New(type);
            ev.tick = Time.time + tick;
            eventQueue.Push(ev);
            return ev.obj;
        }

        static public object Schedule(SType type, float tick = 0f)
        {
            //Debug.LogError($"Schedule SType {type.FullName}");
            Event ev = New(type);
            ev.tick = Time.time + tick;
            eventQueue.Push(ev);
            return ev.objS;
        }

        /// <summary>
        /// Reschedule an existing event for a future tick, and return it.
        /// </summary>
        /// <returns>The event.</returns>
        /// <param name="tick">Tick.</param>
        static public object Reschedule(object obj, float tick = 0)
        {
            if (events.TryGetValue(obj, out Event ev))
            {
                ev.tick = Time.time + tick;
                eventQueue.Push(ev);
            }
            return obj;
        }
        /// <summary>
        /// Whether enabled
        /// </summary>
        public static bool enabled = true;
        /// <summary>
        /// It was called by Update function in 'HotUpdateManager.cs', you can remove it if you want to call it by yourself.
        /// Tick the simulation. Returns the count of remaining events.
        /// If remaining events is zero, the simulation is finished unless events are
        /// injected from an external system via a Schedule() call.
        /// </summary>
        /// <returns></returns>
        static public int Tick()
        {
            if (enabled)
            {
                var time = Time.time;
                var executedEventCount = 0;
                while (eventQueue.Count > 0 && eventQueue.Peek().tick <= time)
                {
                    var ev = eventQueue.Pop();
                    var tick = ev.tick;
                    ev.ExecuteEvent();
                    if (ev.tick > tick)
                    {
                        //event was rescheduled, so do not return it to the pool.
                    }
                    else
                    {
                        // Debug.Log($"<color=green>{ev.tick} {ev.GetType().Name}</color>");
                        ev.Cleanup();
                        try
                        {
                            eventPools[ev.fullName].Push(ev);
                        }
                        catch (KeyNotFoundException)
                        {
                            //This really should never happen inside a production build.
                            Debug.LogError($"No Pool for: {ev.GetType()}");
                        }
                    }
                    executedEventCount++;
                }
            }
            return eventQueue.Count;
        }
        static Dictionary<object, Event> events = new Dictionary<object, Event>();
        /// <summary>
        /// An event is something that happens at a point in time in a simulation.
        /// The Precondition method is used to check if the event should be executed,
        /// as conditions may have changed in the simulation since the event was 
        /// originally scheduled.
        /// </summary>
        /// <typeparam name="Event"></typeparam>
        public class Event : IComparable<Event>
        {
            internal float tick;
            internal object obj;
            private Type type;
            internal SInstance objS;
            internal string fullName;
            public Event(Type type)
            {
                this.type = type;
                obj = type.Assembly.CreateInstance(type.FullName);
                events[obj] = this;
                fullName = type.FullName;
            }
            public Event(SType type)
            {
                objS = type.New().value as SInstance;
                events[objS] = this;
                fullName = type.FullName;
            }

            public int CompareTo(Event other)
            {
                return tick.CompareTo(other.tick);
            }

            public void Execute()
            {
                if (Exist("Execute"))
                {
                    MemberCall("Execute");
                    if (onExecutes.TryGetValue(fullName, out Action<object> _action))
                    {
                        if (obj != null)
                            _action?.Invoke(obj);
                        else
                            _action?.Invoke(objS);
                    }
                }
            }

            bool Exist(string funcName)
            {
                if (objS != null)
                {
                    return objS.Exist(funcName);
                }
                else if (obj != null)
                {
                    return type.GetMethod(funcName) != null;
                }
                return false;
            }
            object MemberCall(string funcName)
            {
                if (objS != null)
                {
                    return objS.MemberCall(funcName);
                }
                else if (obj != null)
                {
                    MethodInfo mi = type.GetMethod(funcName);
                    if (mi != null)
                        return mi.Invoke(obj, null);
                }
                return null;
            }

            public bool Precondition()
            {
                if (Exist("Precondition"))
                    return (bool)MemberCall("Precondition");
                return true;
            }

            internal void ExecuteEvent()
            {
                if (Precondition())
                    Execute();
            }

            /// <summary>
            /// This method is generally used to set references to null when required.
            /// It is automatically called by the Simulation when an event has completed.
            /// </summary>
            internal void Cleanup()
            {
                if (Exist("Cleanup"))
                    MemberCall("Cleanup");
            }
        }
        static Dictionary<string, Action<object>> onExecutes = new Dictionary<string, Action<object>>();
        public static void OnExecute(Type type, Action<object> action)
        {
            if (onExecutes.TryGetValue(type.FullName, out Action<object> _action))
            {
                _action += action;
            }
            else
            {
                onExecutes[type.FullName] = action;
            }
        }
        public static void OnExecute(SType type, Action<object> action)
        {
            OnExecute(type.FullName, action);
        }
        public static void OnExecute(string fullName, Action<object> action)
        {
            if (onExecutes.TryGetValue(fullName, out Action<object> _action))
            {
                _action += action;
            }
            else
            {
                onExecutes[fullName] = action;
            }
        }
    }
    /// <summary>
    /// This class just copy from 'Platformer Microgame'.
    /// HeapQueue provides a queue collection that is always ordered.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HeapQueue<T> where T : IComparable<T>
    {
        List<T> items;

        public int Count { get { return items.Count; } }

        public bool IsEmpty { get { return items.Count == 0; } }

        public T First { get { return items[0]; } }

        public void Clear() => items.Clear();

        public bool Contains(T item) => items.Contains(item);

        public void Remove(T item) => items.Remove(item);

        public T Peek() => items[0];

        public HeapQueue()
        {
            items = new List<T>();
        }

        public void Push(T item)
        {
            //add item to end of tree to extend the list
            items.Add(item);
            //find correct position for new item.
            SiftDown(0, items.Count - 1);
        }

        public T Pop()
        {

            //if there are more than 1 items, returned item will be first in tree.
            //then, add last item to front of tree, shrink the list
            //and find correct index in tree for first item.
            T item;
            var last = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            if (items.Count > 0)
            {
                item = items[0];
                items[0] = last;
                SiftUp();
            }
            else
            {
                item = last;
            }
            return item;
        }


        int Compare(T A, T B) => A.CompareTo(B);

        void SiftDown(int startpos, int pos)
        {
            //preserve the newly added item.
            var newitem = items[pos];
            while (pos > startpos)
            {
                //find parent index in binary tree
                var parentpos = (pos - 1) >> 1;
                var parent = items[parentpos];
                //if new item precedes or equal to parent, pos is new item position.
                if (Compare(parent, newitem) <= 0)
                    break;
                //else move parent into pos, then repeat for grand parent.
                items[pos] = parent;
                pos = parentpos;
            }
            items[pos] = newitem;
        }

        void SiftUp()
        {
            var endpos = items.Count;
            var startpos = 0;
            //preserve the inserted item
            var newitem = items[0];
            var childpos = 1;
            var pos = 0;
            //find child position to insert into binary tree
            while (childpos < endpos)
            {
                //get right branch
                var rightpos = childpos + 1;
                //if right branch should precede left branch, move right branch up the tree
                if (rightpos < endpos && Compare(items[rightpos], items[childpos]) <= 0)
                    childpos = rightpos;
                //move child up the tree
                items[pos] = items[childpos];
                pos = childpos;
                //move down the tree and repeat.
                childpos = 2 * pos + 1;
            }
            //the child position for the new item.
            items[pos] = newitem;
            SiftDown(startpos, pos);
        }
    }
}


