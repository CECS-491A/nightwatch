﻿using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace DataAccessLayer.Database
{
    public class DatabaseContext : DbContext
    {
        const string LOCAL_SQL_SERVER = "localdb";
        const string LOCAL_DB_NAME = "PointMapDB";
        public DatabaseContext()
        {
            this.Database.Connection.ConnectionString = string.Format(
                "Data Source={0};Initial Catalog={1};Integrated Security=True",
                LOCAL_SQL_SERVER, LOCAL_DB_NAME
                );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
