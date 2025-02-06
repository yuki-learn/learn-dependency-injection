﻿using System;
using System.Collections.Generic;

namespace Ploeh.Samples.Commerce.Domain.EventHandlers
{
    public class CompositeEventHandler<TEvent> : IEventHandler<TEvent>
    {
        private readonly IEnumerable<IEventHandler<TEvent>> handlers;

        public CompositeEventHandler(IEnumerable<IEventHandler<TEvent>> handlers)
        {
            this.handlers = handlers;
        }

        public void Handle(TEvent e)
        {
            foreach (var handler in this.handlers)
            {
                handler.Handle(e);
            }
        }
    }
}