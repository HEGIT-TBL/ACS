using ACS.Core.Contracts.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACS.Core.Models
{
    public class User : IHasGuid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        [Column("Id")]
        public Guid Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("Surname")]
        public string Surname { get; set; }

        [Column("Patronymic")]
        public string Patronymic { get; set; }

        [Column("ProfilePicture")]
        public byte[] ProfilePicture { get; set; }

        public List<KeyCard> KeyCards { get; set; }
        public List<Identifier> Identifiers { get; set; }
        public List<Car> OwnedCars { get; set; }
    }
}
