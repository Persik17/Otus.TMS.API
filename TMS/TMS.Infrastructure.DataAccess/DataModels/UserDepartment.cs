﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TMS.Abstractions.Models.Contracts;

namespace TMS.Infrastructure.DataAccess.DataModels
{
    public class UserDepartment : IAuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }

        public DateTime JoinDate { get; set; }

        public UserDepartment() { }
    }
}
