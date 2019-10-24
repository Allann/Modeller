using Microsoft.Extensions.Logging;

namespace Hy.Modeller
{
    public static class LoggingEvents
    {
        public static readonly EventId StartEvent = new EventId(100, "StartInfo");
        public static readonly EventId ParameterEvent = new EventId(101, "ParameterInfo");
        public static readonly EventId CompleteEvent = new EventId(200, "CompleteInfo");
        public static readonly EventId ValidationEvent = new EventId(1000, "ValidationInfo");

        public static readonly EventId ListError = new EventId(1001, "ListInfo");
        public static readonly EventId UpdateError = new EventId(1002, "UpdateInfo");
        public static readonly EventId BuildError = new EventId(1002, "BuildInfo");
    }
}
