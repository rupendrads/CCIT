using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestApp
{
    internal class TradesContext : DbContext    
    {
        public TradesContext() { }
        public TradesContext(DbContextOptions<TradesContext> options) : base(options) { }
        public virtual DbSet<Trade> Trades
        {
            get;
            set;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var filePath = Path.Combine(System.Environment.CurrentDirectory, @"\Trade.db");
            optionsBuilder.UseSqlite($"Data Source= Trade.db");
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trade>(entity => {
                entity.HasKey(e => e.TradeID);
                entity.Property(e => e.TradeID).HasColumnType("VARCHAR");
                entity.Property(e => e.ISIN).HasColumnType("VARCHAR");
                entity.Property(e => e.Notional).HasColumnType("VARCHAR");                
            });            
        }
    }
}
