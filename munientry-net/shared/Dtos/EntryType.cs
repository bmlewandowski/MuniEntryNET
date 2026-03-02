using System.Text.Json.Serialization;

namespace Munientry.Shared.Dtos
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EntryType
    {
        DenyDrivingPrivileges,
        PermitTest,
        PermitRenew,
        TerminatePrivileges
    }
}
