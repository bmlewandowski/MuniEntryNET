using System.Text.Json.Serialization;

namespace Munientry.Client.Shared.Models
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