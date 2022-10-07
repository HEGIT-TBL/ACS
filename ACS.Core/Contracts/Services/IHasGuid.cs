using System;

namespace ACS.Core.Contracts.Services
{
    public interface IHasGuid
    {
        Guid Id { get; set; }
    }
}
