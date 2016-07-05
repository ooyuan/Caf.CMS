﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CAF.Infrastructure.Core.Events;
using CAF.Infrastructure.Core;
using CAF.Infrastructure.Core.Async;
using Autofac;
using CAF.Infrastructure.Core.Logging;
namespace CAF.WebSite.Application.Services.Events
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ConcurrentDictionary<object, Timer> _queue = new ConcurrentDictionary<object, Timer>();

        public void Publish<T>(T eventMessage)
        {
            if (eventMessage != null)
            {
                // Enable event throttling by allowing the very same event to be published only all 150 ms.
                Timer timer;
                if (_queue.TryGetValue(eventMessage, out timer))
                {
                    // do nothing. The same event was published a tick ago.
                    return;
                }

                _queue[eventMessage] = new Timer(RemoveFromQueue, eventMessage, 150, Timeout.Infinite);
            }

            var consumerFactory = EngineContext.Current.Resolve<IConsumerFactory<T>>();
            if (consumerFactory == null)
                return;

            IEnumerable<IConsumer<T>> consumers = null;

            // first fire/forget all async consumers
            if (consumerFactory.HasAsyncConsumer)
            {
                AsyncRunner.Run((c, ct) =>
                {
                    // for wiring up dependencies correctly
                    var newFactory = c.Resolve<IConsumerFactory<T>>();
                    consumers = newFactory.GetConsumers(true);
                    foreach (var consumer in consumers)
                    {
                        consumer.HandleEvent(eventMessage);
                    }
                }).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        var ex = t.Exception;
                        if (ex != null)
                        {
                            ex.InnerExceptions.Each(x => LogError(x));
                        }
                    }
                });
            }

            // now execute all sync consumers
            consumers = consumerFactory.GetConsumers(false);
            foreach (var consumer in consumers)
            {
                PublishEvent(consumer, eventMessage);
            }
        }

        private void PublishEvent<T>(IConsumer<T> x, T eventMessage)
        {
            try
            {
                x.HandleEvent(eventMessage);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LogError(Exception exception)
        {
            try
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(exception.Message, exception);
            }
            catch
            {
                //do nothing
            }
        }

        private void RemoveFromQueue(object eventMessage)
        {
            Timer timer;
            if (_queue.TryRemove(eventMessage, out timer))
            {
                timer.Dispose();
            }
        }

    }
}
