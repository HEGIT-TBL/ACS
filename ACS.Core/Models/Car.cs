using ACS.Core.Contracts.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models
{
    public class Car:IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("CarNumberPlate")]
        public string CarNumberPlate { get; set; }

        [Column("CarModel")]
        public string CarModel { get; set; }

        [Column("Color")]
        public string Color { get; set; }
    }
}
