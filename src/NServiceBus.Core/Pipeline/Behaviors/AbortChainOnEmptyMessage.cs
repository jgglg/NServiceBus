﻿namespace NServiceBus.Pipeline.Behaviors
{
    using System.Reflection;
    using Logging;
    using Pipeline;
    using Unicast.Transport;

    public class AbortChainOnEmptyMessage : IBehavior
    {
        static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IBehavior Next { get; set; }
        
        public void Invoke(IBehaviorContext context)
        {
            var transportMessage = context.TransportMessage;

            if (!transportMessage.IsControlMessage() && LogicalMessageCount(context) == 0)
            {
                context.Trace("Ignoring empty message with ID {0}", transportMessage.Id);
                Log.Warn("Received an empty message - ignoring.");
                return;
            }

            Next.Invoke(context);
        }

        static int LogicalMessageCount(IBehaviorContext context)
        {
            return context.Messages == null
                       ? 0
                       : context.Messages.Length;
        }
    }
}