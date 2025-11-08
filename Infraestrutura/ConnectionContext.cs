using FirstAPI.Domain.Model.CarroAggregate;
using FirstAPI.Domain.Model.CompanyAggregate;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Infrastructure
{
    public class ConnectionContext : DbContext
    {
        // Este construtor é essencial para a injeção de dependência
        public ConnectionContext(DbContextOptions<ConnectionContext> options) : base(options)
        {
        }

        public DbSet<Carro> Carros { get; set; }
        public DbSet<Company> Companies { get; set; }

        // O método OnConfiguring não é mais necessário aqui,
        // pois vamos configurar a conexão no Program.cs
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //    ...
        // }
    }
}