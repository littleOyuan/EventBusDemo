using EventTest.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace EventTest
{
    public class EventBus
    {
        private static EventBus _eventBus;
        private static readonly object Locker = new object();

        public static EventBus Default
        {
            get
            {
                if (_eventBus == null)
                {
                    lock (Locker)
                    {
                        if (_eventBus == null)
                        {
                            _eventBus = new EventBus();
                        }
                    }
                }

                MapEventToHandler();

                return _eventBus;
            }
        }

        private static readonly ConcurrentDictionary<Type, List<Type>> EventHandlerMapping = new ConcurrentDictionary<Type, List<Type>>();

        private EventBus()
        {
        }

        private static void MapEventToHandler()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IEventHandler).IsAssignableFrom(type)) continue;

                Type handlerInterface = type.GetInterface("IEventHandler`1");//获取该类实现的泛型接口

                if (handlerInterface == null) continue;

                Type eventDataType = handlerInterface.GetGenericArguments()[0]; // 获取泛型接口指定的参数类型

                if (EventHandlerMapping.ContainsKey(eventDataType))
                {
                    List<Type> handlerTypes = EventHandlerMapping[eventDataType];

                    handlerTypes.Add(type);

                    EventHandlerMapping[eventDataType] = handlerTypes;
                }
                else
                {
                    List<Type> handlerTypes = new List<Type> { type };

                    EventHandlerMapping[eventDataType] = handlerTypes;
                }
            }
        }

        /// <summary>
        /// 注册事件源与事件处理
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventHandler"></param>
        public void Register<TEventData>(Type eventHandler)
        {
            List<Type> handlerTypes = EventHandlerMapping[typeof(TEventData)];

            if (handlerTypes.Contains(eventHandler)) return;

            handlerTypes.Add(eventHandler);

            EventHandlerMapping[typeof(TEventData)] = handlerTypes;
        }

        /// <summary>
        /// 事件源与事件处理解绑
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventHandler"></param>
        public void UnRegister<TEventData>(Type eventHandler)
        {
            List<Type> handlerTypes = EventHandlerMapping[typeof(TEventData)];

            if (!handlerTypes.Contains(eventHandler)) return;

            handlerTypes.Remove(eventHandler);

            EventHandlerMapping[typeof(TEventData)] = handlerTypes;
        }


        /// <summary>
        /// 根据事件源触发绑定的事件处理
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            List<Type> handlers = EventHandlerMapping[eventData.GetType()];

            if (handlers == null || handlers.Count <= 0) return;

            foreach (var handler in handlers)
            {
                MethodInfo methodInfo = handler.GetMethod("HandleEvent");

                if (methodInfo == null) continue;

                object obj = Activator.CreateInstance(handler);

                methodInfo.Invoke(obj, new object[] { eventData });
            }
        }
    }
}
