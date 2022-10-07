using ACS.Core.Contracts.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models
{
    public class Identifier : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Photo")]
        public byte[] Photo { get; set; }

        [Column("FacePoints")]
        public float[] FacePoints { get; set; }
    }
}
