﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace pEasyPrint
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class pEasyPrintEntities : DbContext
    {
        public pEasyPrintEntities()
            : base("name=pEasyPrintEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<MstOrderStatu> MstOrderStatus { get; set; }
        public DbSet<tblCity> tblCities { get; set; }
        public DbSet<tblCountry> tblCountries { get; set; }
        public DbSet<tblCustomerOrderReview> tblCustomerOrderReviews { get; set; }
        public DbSet<tblOrderStage> tblOrderStages { get; set; }
        public DbSet<tblState> tblStates { get; set; }
        public DbSet<webpages_Membership> webpages_Membership { get; set; }
        public DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<tblOrderContent> tblOrderContents { get; set; }
        public DbSet<OrderAssignment> OrderAssignments { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<tblOrderProof> tblOrderProofs { get; set; }
        public DbSet<tblContentWriter> tblContentWriters { get; set; }
        public DbSet<tblDesigner> tblDesigners { get; set; }
        public DbSet<tblOrder> tblOrders { get; set; }
        public DbSet<tblOrderItem> tblOrderItems { get; set; }
        public DbSet<tblShippingAddress> tblShippingAddresses { get; set; }
    }
}
