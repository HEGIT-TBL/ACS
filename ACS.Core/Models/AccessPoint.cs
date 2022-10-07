using ACS.Core.Contracts.Services;
using ACS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models
{
    public class AccessPoint : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [Column("AccessPointType")]
        public AccessPointTypes AccessPointType { get; set; }

        [Required]
        [Column("Location")]
        public string Location { get; set; }

        [Required]
        [Column("ControllerIP")]
        public string ControllerIP { get; set; }

        public List<Camera> Cameras { get; set; }
        public List<KeyCard> AllowedKeyCards { get; set; }
    }
}
