using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DatingApp.Api.Models;

namespace DatingApp.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) :  base(options){}
        public DbSet<Value> Values {get;set;}
    }
}