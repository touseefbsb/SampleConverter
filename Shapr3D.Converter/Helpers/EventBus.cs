﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapr3D.Converter.Helpers
{
    public class EventBus
    {
        private static readonly EventBus eventBus = new EventBus();

        private EventBus()
        {
        }

        public static EventBus GetInstance()
        {
            return eventBus;
        }

        private readonly List<Action<object>> handlers = new List<Action<object>>();

        public void Subscribe(Action<object> handler)
        {
            handlers.Add(handler);
        }

        public void Publish(object e)
        {
            handlers.ForEach(handler => handler(e));
        }
    }

}
