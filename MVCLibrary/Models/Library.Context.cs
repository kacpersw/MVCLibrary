﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCLibrary.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LibraryEntities : DbContext
    {
        public LibraryEntities()
            : base("name=LibraryEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<AdminMessage> AdminMessage { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookSpecimen> BookSpecimen { get; set; }
        public virtual DbSet<Borrow> Borrow { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Limit> Limit { get; set; }
    }
}
