using Franquias.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opcoes) : base(opcoes)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Franqueadora> Franqueadoras { get; set; }
        public DbSet<UnidadeFranqueada> Unidades { get; set; }
        public DbSet<Responsavel> Responsaveis { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<ProdutoServico> Produtos { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Royalty> Royalties { get; set; }
        public DbSet<ChamadoSuporte> Chamados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UnidadeFranqueada>()
                .HasIndex(u => u.Cnpj)
                .IsUnique();

            modelBuilder.Entity<Fornecedor>()
                .HasIndex(f => f.Cnpj)
                .IsUnique();

            modelBuilder.Entity<Estoque>()
                .HasIndex(e => new { e.UnidadeFranqueadaId, e.ProdutoServicoId })
                .IsUnique();

            modelBuilder.Entity<Royalty>()
                .HasIndex(r => new { r.UnidadeFranqueadaId, r.Ano, r.Mes })
                .IsUnique();

            modelBuilder.Entity<Franqueadora>()
                .Property(f => f.PercentualRoyaltyPadrao)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<UnidadeFranqueada>()
                .Property(u => u.PercentualRoyalty)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<ProdutoServico>()
                .Property(p => p.PrecoBase)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Venda>()
                .Property(v => v.ValorTotal)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.PrecoUnitario)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<ItemVenda>()
                .Property(i => i.Subtotal)
                .HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Royalty>()
                .Property(r => r.FaturamentoPeriodo)
                .HasColumnType("decimal(12,2)");

            modelBuilder.Entity<Royalty>()
                .Property(r => r.ValorDevido)
                .HasColumnType("decimal(12,2)");

            modelBuilder.Entity<Royalty>()
                .Property(r => r.PercentualAplicado)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.UnidadeFranqueada)
                .WithMany()
                .HasForeignKey(u => u.UnidadeFranqueadaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProdutoServico>()
                .HasOne(p => p.Fornecedor)
                .WithMany(f => f.Produtos)
                .HasForeignKey(p => p.FornecedorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
