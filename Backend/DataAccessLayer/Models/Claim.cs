﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Claim
    {
        public Claim()
        {
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey("Service")]
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }
        [Required]
        [Column(TypeName = "datetime2")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
    }
}
