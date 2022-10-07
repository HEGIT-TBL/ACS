using ACS.Core.Contracts.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models
{
    public class Camera : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("Location")]
        public string Location { get; set; }

        [Required]
        [Column("StreamLink")]
        public string StreamLink { get; set; }

        public AccessPoint? AccessPoint { get; set; }
    }
}
