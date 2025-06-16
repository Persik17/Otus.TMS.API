﻿using Microsoft.EntityFrameworkCore;
using TMS.Infrastructure.DataAccess.DataModels;

namespace TMS.Infrastructure.DataAccess.Contexts
{
    public class PostgreSqlTmsContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BoardUser> BoardUsers { get; set; }
        public DbSet<BoardUserRole> BoardUserRoles { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Credential> Credentials { get; set; }
        public DbSet<CredentialHistory> CredentialHistories { get; set; }
        public DbSet<NotificationSetting> NotificationSettings { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<DataModels.Task> Tasks { get; set; }
        public DbSet<TaskType> TaskTypes { get; set; }
        public DbSet<TelegramAccount> TelegramAccounts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UserDepartment> UserDepartments { get; set; }
        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<RegistrationVerification> RegistrationVerifications { get; set; }

        public PostgreSqlTmsContext(DbContextOptions<PostgreSqlTmsContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
