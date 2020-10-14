using Knockback.Utility;
using System;
using System.Collections.Generic;

namespace Knockback.Handlers
{
    public class KB_EventHandler : KB_Singleton<KB_EventHandler>
    {
        public bool doNotDestroy = true;

        public Dictionary<string, Action<int>> _eventCollection = new Dictionary<string, Action<int>>();

        protected override void Awake()
        {
            doNotDestoryOnLoad = doNotDestroy;
            base.Awake();
        }

        public void Invoke(string tag, int data)
        {
            if (!_eventCollection.ContainsKey(tag))
                return;
            _eventCollection[tag].Invoke(data);
        }

        public void CreateEvent(string tag, Action<int> _event)
        {
            if (_event != null)
            {
                Action<int> temp = null;
                _eventCollection.Add(tag, temp);
                _eventCollection[tag] += _event;
            }
        }

        public void AddListener(string tag, Action<int> _event)
        {
            if(_eventCollection.ContainsKey(tag))
            {
                _eventCollection[tag] += _event;
            }
        }

        public void RemoveListener(string tag)
        {
            if (_eventCollection.ContainsKey(tag))
                _eventCollection.Remove(tag);
        }
    }
}