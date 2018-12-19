using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Hy.Modeller.Cli
{
    public class LoggingEvents
    {
        public static readonly EventId StartEvent = new EventId(100, "StartInfo");
        public static readonly EventId ParameterEvent = new EventId(101, "ParameterInfo");
        public static readonly EventId CompleteEvent = new EventId(200, "CompleteInfo");
        public static readonly EventId ValidationEvent = new EventId(1000, "ValidationInfo");
    }
}
