using ACS.Core.Helpers;
using ACS.Core.Properties;

namespace ACS.Core.Models.Enums
{
    public enum ParkingLotState
    {
        [LocalizedDescription("Empty", typeof(Resources))]
        Empty,
        [LocalizedDescription("Taken", typeof(Resources))]
        Taken,
        [LocalizedDescription("Maintainance", typeof(Resources))]
        Maintainance,
    }
}
