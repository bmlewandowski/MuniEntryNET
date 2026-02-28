using System.Text.Json.Serialization;

namespace Munientry.Poc.Client.Shared.Models
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