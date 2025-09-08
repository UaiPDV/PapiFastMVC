using Microsoft.EntityFrameworkCore;

namespace BixWeb.Models
{
    public class DbPrint : DbContext
    {
        public DbPrint(DbContextOptions<DbPrint> options)
            : base(options)
        {
        }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioFilial> UsuarioFiliais { get; set; }
        public DbSet<Filial> Filiais { get; set; }
        public DbSet<LoginAPI> LoginAPI { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Preco> Precos { get; set; }
        public DbSet<Campanha> Campanhas { get; set; }
        public DbSet<ProdutoCampanha> ProdutosCampanha { get; set; }
        public DbSet<Modificador> Modificadores { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Convidado> Convidados { get; set; }
        public DbSet<Convite> Convites { get; set; }
        public DbSet<Cupom> Cupons { get; set; }    
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<GenerativeIA> GenerativeIAs { get; set; }
        public DbSet<RegistroLogin> RegistroLogins { get; set; }
        public DbSet<Dispositivo> Dispositivos { get; set; }
        public DbSet<WhatsApp> WhatsApps { get; set; }
        public DbSet<ListaPresente> ListaPresentes { get; set; }
        public DbSet<ReciboPresente> RecibosPresentes { get; set; }
        public DbSet<Presente> Presentes { get; set; }
        public DbSet<ReciboIngresso> ReciboIngressos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<UsuarioFilial>()
            .HasKey(sc => new { sc.codUsuario, sc.codFilial });

            modelBuilder.Entity<Usuario>()
            .HasOne(p => p.loginAPI)
            .WithOne(p => p.usuario)
            .HasForeignKey<LoginAPI>(p => p.codUsuario)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Presente>()
            .HasOne(uf => uf.listaPresente)
            .WithMany(u => u.presentes)
            .HasForeignKey(uf => uf.codListaPresente);

            modelBuilder.Entity<ReciboPresente>()
            .HasOne(rp => rp.usuario)
            .WithMany(u => u.recibosPresentes)
            .HasForeignKey(rp => rp.codUsuario)
            .IsRequired(false) // torna a relação opcional
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ingresso>()
            .HasOne(rp => rp.ReciboIngresso)
            .WithMany(u => u.ingressos)
            .HasForeignKey(rp => rp.codReciboIngresso)
            .IsRequired(false) // torna a relação opcional
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReciboPresente>()
            .HasOne(uf => uf.convite)
            .WithMany(u => u.recibosPresentes)
            .HasForeignKey(uf => uf.codConvite)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Convite>()
            .HasOne(c => c.ListaPresente)
            .WithOne(lp => lp.convite)
            .HasForeignKey<ListaPresente>(lp => lp.codConvite)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ListaPresente>()
            .HasOne(uf => uf.usuario)
            .WithMany(u => u.listaPresentes)
            .HasForeignKey(uf => uf.codUsuario);

            modelBuilder.Entity<RegistroLogin>()
            .HasOne(p => p.UsuarioFilial)
            .WithOne(p => p.RegistroLogin)
            .HasForeignKey<UsuarioFilial>(p => p.codRegistro);

            modelBuilder.Entity<RegistroLogin>()
            .HasOne(uf => uf.Dispositivo)
            .WithMany(u => u.RegistroLogins)
            .HasForeignKey(uf => uf.codDispositivo);

            modelBuilder.Entity<Dispositivo>()
            .HasOne(uf => uf.Filial)
            .WithMany(u => u.Dispositivos)
            .HasForeignKey(uf => uf.codFilial);

            modelBuilder.Entity<Convite>()
			.HasOne(uf => uf.Usuario)
			.WithMany(u => u.Convites)
			.HasForeignKey(uf => uf.codCliente);

			modelBuilder.Entity<UsuarioFilial>()
            .HasOne(uf => uf.Usuario)
            .WithMany(u => u.filiais)
            .HasForeignKey(uf => uf.codUsuario);

            modelBuilder.Entity<UsuarioFilial>()
            .HasOne(uf => uf.Filial)
            .WithMany(f => f.Usuarios)
            .HasForeignKey(uf => uf.codFilial);

            modelBuilder.Entity<Evento>()
            .HasOne(uf => uf.UsuarioFilial)
            .WithMany(u => u.Eventos)
            .HasForeignKey(c => new { c.codUsuario, c.codFilial })
            .IsRequired(false)  // Isso torna o relacionamento opcional
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Campanha>()
            .HasOne(uf => uf.Usuariofilial)
            .WithMany(u => u.Campanhas)
            .HasForeignKey(c => new { c.codUsuario, c.codFilial });

            modelBuilder.Entity<Convite>()
			.HasOne(uf => uf.UsuarioFilial)
			.WithMany(u => u.Convites)
			.HasForeignKey(c => new { c.codCriador, c.codFilial })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict); 

			modelBuilder.Entity<Evento>()
            .HasOne(uf => uf.Endereco)
            .WithMany(u => u.Eventos)
            .HasForeignKey(uf => uf.codEndereco);

            modelBuilder.Entity<Endereco>()
            .HasOne(p => p.Filial)
            .WithOne(p => p.Endereco)
            .HasForeignKey<Filial>(p => p.codEndereco);

            modelBuilder.Entity<Lote>()
            .HasOne(uf => uf.Evento)
            .WithMany(u => u.Lotes)
            .HasForeignKey(uf => uf.codEvento);

            modelBuilder.Entity<Ingresso>()
            .HasOne(uf => uf.Lote)
            .WithMany(u => u.Ingressos)
            .HasForeignKey(uf => uf.codLote);

            modelBuilder.Entity<Ingresso>()
            .HasOne(p => p.Convite)
            .WithMany(u => u.Ingressos)
            .HasForeignKey(p => p.codConvite);

            modelBuilder.Entity<Produto>()
            .HasOne(uf => uf.Categoria)
            .WithMany(u => u.Produtos)
            .HasForeignKey(uf => uf.codCategoria);

            modelBuilder.Entity<Modificador>()
            .HasOne(uf => uf.Produto)
            .WithMany(u => u.Modificadores)
            .HasForeignKey(uf => uf.codproduto);

            modelBuilder.Entity<Preco>()
            .HasOne(uf => uf.Produto)
            .WithMany(u => u.Precos)
            .HasForeignKey(uf => uf.codProduto);

            modelBuilder.Entity<ProdutoCampanha>()
            .HasOne(uf => uf.Produto)
            .WithMany(u => u.ProdutosCampanha)
            .HasForeignKey(uf => uf.codProduto);

            modelBuilder.Entity<ProdutoCampanha>()
            .HasOne(uf => uf.Campanha)
            .WithMany(u => u.produtosCampanha)
            .HasForeignKey(uf => uf.codCampanha);

            modelBuilder.Entity<ProdutoCampanha>()
            .HasOne(uf => uf.Evento)
            .WithMany(u => u.ProdutosEvento)
            .HasForeignKey(uf => uf.codEvento);

            modelBuilder.Entity<Voucher>()
            .HasOne(uf => uf.Campanha)
            .WithMany(u => u.Vouchers)
            .HasForeignKey(uf => uf.codCampanha)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Voucher>()
            .HasOne(v => v.UsuarioFilial)
            .WithMany(u => u.Vouchers)
            .HasForeignKey(c => new { c.codCriador , c.codFilial})
            .OnDelete(DeleteBehavior.Restrict); // Para evitar deleção em cascata

            modelBuilder.Entity<Voucher>()
            .HasOne(v => v.Cliente)
            .WithMany(u => u.Vouchers)
            .HasForeignKey(v => v.codCliente)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cupom>()
            .HasOne(v => v.UsuarioFilial)
            .WithMany(u => u.Cupons)
            .HasForeignKey(c => new { c.codCriador, c.codFilial });

            modelBuilder.Entity<Cupom>()
            .HasOne(v => v.Campanha)
            .WithMany(u => u.Cupons)
            .HasForeignKey(v => v.codCampanha);

            modelBuilder.Entity<Cupom>()
            .HasOne(v => v.usuario)
            .WithMany(u => u.Cupons)
            .HasForeignKey(v => v.codcliente);

            modelBuilder.Entity<Convite>()
            .HasOne(v => v.Evento)
            .WithMany(u => u.Convites)
            .HasForeignKey(v => v.codEvento);

            modelBuilder.Entity<Convidado>()
            .HasOne(v => v.Convite)
            .WithMany(u => u.Convidados)
            .HasForeignKey(v => v.codConvite);

            modelBuilder.Entity<WhatsApp>()
                .HasOne(c => c.Usuario)
                .WithOne(u => u.WhatsApp)
                .HasForeignKey<WhatsApp>(c => c.codFilial)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Usuario>()
                .HasOne(p => p.WhatsApp)
                .WithOne(p => p.Usuario)
                .HasForeignKey<WhatsApp>(p => p.codUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
