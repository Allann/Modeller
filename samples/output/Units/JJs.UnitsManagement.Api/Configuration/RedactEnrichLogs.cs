using Serilog.Core;
using Serilog.Events;

namespace JJs.UnitsManagement.Api.Configuration;

public class RedactEnrichLogs(HashSet<string> fieldToRedact) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var requestProperty = logEvent.Properties.FirstOrDefault(p => p.Key == "Request").Value;

        if (requestProperty is StructureValue structureValue)
        {
            var sanitizedRequest = RedactFieldInStructure(structureValue);
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Request", sanitizedRequest));
        }
    }

    private StructureValue RedactFieldInStructure(StructureValue structureValue)
    {
        var redactedProperties = structureValue.Properties
            .Select(property =>
            {
                if (fieldToRedact.Contains(property.Name))
                {
                    return new(property.Name, new ScalarValue("REDACTED"));
                }
                else if (property.Value is StructureValue nestedStructure)
                {
                    return new(property.Name, RedactFieldInStructure(nestedStructure));
                }

                return property;
            })
            .ToList();

        return new(redactedProperties);
    }
}
