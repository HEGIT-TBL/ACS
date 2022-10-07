using ACS.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models
{
    public class KeyCard : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("KeyCardId")]
        public string Key { get; set; }

        [Required]
        [Column("ExpirationDate")]
        public DateTime ExpirationDate { get; set; }

        public List<AccessPoint> AvailableAccessPoints { get; set; }
        public User Owner { get; set; }
    }
}
