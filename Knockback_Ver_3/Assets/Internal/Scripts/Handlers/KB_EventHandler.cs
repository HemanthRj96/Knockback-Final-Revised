using Knockback.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Handlers
{
    public static class KB_EventHandler
    {   
        /// <summary>
        /// Internal class for data injection
        /// </summary>
        internal class Message : IMessage
        {
            public Message(object data = null, GameObject source = null, float timeUntilActivation = 43)
            {
                this.data = data;
                this.source = source;
                this.timeUntilActivation = timeUntilActivation;
            }
            public object data { get; set; }
            public GameObject source { get; set; }
            public float timeUntilActivation { get; set; }
        }


        private static IMessage message = null;
        private static Dictionary<string, Action<IMessage>> _eventCollection = new Dictionary<string, Action<IMessage>>();

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="_message"></param>
        private static void Construct(IMessage _message)
        {
            message = _message;
        }

        /// <summary>
        /// Method to add events; If the method tag already exists then the method is automatically subscribed to the listener
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="_event"></param>
        public static void AddEvent(string tag, Action<IMessage> _event)
        {
            if (_event == null)
                return;

            if (_eventCollection.ContainsKey(tag))
            {
                _eventCollection[tag] += _event;
            }
            else
            {
                _eventCollection.Add(tag, _event);
            }
        }

        /// <summary>
        /// Removes the event itself from the event collection
        /// </summary>
        /// <param name="tag"></param>
        public static void RemoveEvent(string tag)
        {
            if (!_eventCollection.ContainsKey(tag))
                return;

            _eventCollection.Remove(tag);
        }

        /// <summary>
        /// Removes the only one of the listener from the event; If the listener becomes null then the tag is 
        /// automatically removed from the event collection
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="_event"></param>
        public static void RemoveListener(string tag, Action<IMessage> _event)
        {
            if (!_eventCollection.ContainsKey(tag))
                return;
            _eventCollection[tag] -= _event;
            if (_eventCollection[tag] == null)
                _eventCollection.Remove(tag);
        }

        /// <summary>
        /// Method to invoke an event from the collection
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="_data"></param>
        /// <param name="_source"></param>
        /// <param name="_timeUntilActivation"></param>
        public static void Invoke(string tag, object _data = null, GameObject _source = null, float _timeUntilActivation = 0)
        {            
            if (!_eventCollection.ContainsKey(tag))
                return;
            Construct(new Message(_data, _source, _timeUntilActivation));
            _eventCollection[tag]?.Invoke(message);
        }
    }
}