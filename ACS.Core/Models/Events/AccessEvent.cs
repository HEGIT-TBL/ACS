using ACS.Core.Contracts.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models.Events
{
    public class AccessEvent : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("CaptureTime")]
        public DateTime AccessTime { get; set; }

        [Required]
        [Column("IsPermissionGranted")]
        public bool IsPermissionGranted { get; set; }

        public User User { get; set; }
        public AccessPoint AccessPoint { get; set; }
    }
}
