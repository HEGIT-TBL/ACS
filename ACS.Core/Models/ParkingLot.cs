using ACS.Core.Contracts.Services;
using ACS.Core.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models
{
    public class ParkingLot : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("LotNumber")]
        public int LotNumber { get; set; }

        [Required]
        [Column("State")]
        public ParkingLotState State { get; set; }

        public Car PlacedCar { get; set; }
    }
}
