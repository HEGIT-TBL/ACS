using ACS.Core.Contracts.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models.Events
{
    public class ParkingLotStateChangedEvent : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("StateChangeTime")]
        public DateTime StateChangeTime { get; set; }

        public ParkingLot ChangedLot { get; set; }
    }
}
