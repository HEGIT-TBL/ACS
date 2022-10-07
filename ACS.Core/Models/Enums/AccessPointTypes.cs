using ACS.Core.Helpers;
using ACS.Core.Properties;

namespace ACS.Core.Models.Enums
{
    public enum AccessPointTypes
{
        [LocalizedDescription("Door", typeof(Resources))]
        Door,
        [LocalizedDescription("Turnstile", typeof(Resources))]
        Turnstile,
        [LocalizedDescription("Gate", typeof(Resources))]
        Gate,
    }
}
