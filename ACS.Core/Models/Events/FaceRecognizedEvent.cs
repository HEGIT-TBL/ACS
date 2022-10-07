using ACS.Core.Contracts.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models.Events
{
    public class FaceRecognizedEvent : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Probability")]
        public double Probability { get; set; }

        [Required]
        [Column("CaptureTime")]
        public DateTime CaptureTime { get; set; }

        public User RecognizedUser { get; set; }
        public Camera Camera { get; set; }
    }
}
