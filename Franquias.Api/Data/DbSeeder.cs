using System;
using System.Collections.Generic;
using System.Linq;
using Franquias.Api.Models;

namespace Franquias.Api.Data
{
    // Popula o banco com dados de exemplo para facilitar os testes e a avaliacao.
    public static class DbSeeder
    {
        public static void Popular(AppDbContext contexto)
        {
            if (contexto.Usuarios.Any())
            {
                return;
            }

            // ----- Franqueadora, categorias, fornecedores e produtos -----
            var franqueadora = new Franqueadora
            {
                RazaoSocial = "Rede Cafe Bom Ltda",
                NomeFantasia = "Cafe Bom Franquias",
                Cnpj = "10203040000199",
                Email = "contato@cafebom.com.br",
                Telefone = "(11) 4000-1000",
                PercentualRoyaltyPadrao = 5m
            };
            contexto.Franqueadoras.Add(franqueadora);

            var catBebidas = new Categoria { Nome = "Bebidas" };
            var catAlimentos = new Categoria { Nome = "Alimentos" };
            var catServicos = new Categoria { Nome = "Servicos" };
            contexto.Categorias.AddRange(catBebidas, catAlimentos, catServicos);

            var fornCentral = new Fornecedor
            {
                Nome = "Distribuidora Central Ltda",
                Cnpj = "11222333000144",
                Email = "vendas@central.com.br",
                Telefone = "(11) 3222-1111"
            };
            var fornInsumos = new Fornecedor
            {
                Nome = "Insumos Rapidos ME",
                Cnpj = "55666777000188",
                Email = "comercial@insumosrapidos.com.br",
                Telefone = "(19) 3555-2222"
            };
            contexto.Fornecedores.AddRange(fornCentral, fornInsumos);

            var prodCafe = new ProdutoServico { Nome = "Cafe Expresso", Descricao = "Cafe expresso tradicional", PrecoBase = 6.50m, Categoria = catBebidas, Fornecedor = fornCentral };
            var prodCappuccino = new ProdutoServico { Nome = "Cappuccino", Descricao = "Cappuccino com canela", PrecoBase = 9.00m, Categoria = catBebidas, Fornecedor = fornCentral };
            var prodAgua = new ProdutoServico { Nome = "Agua Mineral 500ml", Descricao = "Agua sem gas", PrecoBase = 4.00m, Categoria = catBebidas, Fornecedor = fornCentral };
            var prodPaoQueijo = new ProdutoServico { Nome = "Pao de Queijo", Descricao = "Porcao com 3 unidades", PrecoBase = 5.50m, Categoria = catAlimentos, Fornecedor = fornInsumos };
            var prodBolo = new ProdutoServico { Nome = "Bolo de Chocolate", Descricao = "Fatia de bolo", PrecoBase = 8.50m, Categoria = catAlimentos, Fornecedor = fornInsumos };
            var prodSanduiche = new ProdutoServico { Nome = "Sanduiche Natural", Descricao = "Pao integral com frango", PrecoBase = 12.00m, Categoria = catAlimentos, Fornecedor = fornInsumos };
            var servConsultoria = new ProdutoServico { Nome = "Consultoria de Layout", Descricao = "Consultoria de organizacao da loja", PrecoBase = 250.00m, EhServico = true, Categoria = catServicos };
            var servTreinamento = new ProdutoServico { Nome = "Treinamento de Equipe", Descricao = "Treinamento de atendimento", PrecoBase = 400.00m, EhServico = true, Categoria = catServicos };
            contexto.Produtos.AddRange(prodCafe, prodCappuccino, prodAgua, prodPaoQueijo, prodBolo, prodSanduiche, servConsultoria, servTreinamento);

            contexto.SaveChanges();

            // ----- Unidades e responsaveis -----
            var unidadeCentro = new UnidadeFranqueada
            {
                Nome = "Franquia Centro",
                Cnpj = "00111222000101",
                Cidade = "Sao Paulo",
                Estado = "SP",
                Endereco = "Rua XV de Novembro, 100",
                Telefone = "(11) 3100-0101",
                Email = "centro@cafebom.com.br",
                DataInicio = new DateTime(2024, 2, 1),
                PercentualRoyalty = 5m,
                Ativa = true,
                FranqueadoraId = franqueadora.Id,
                Responsaveis = new List<Responsavel>
                {
                    new Responsavel { Nome = "Joao Pereira", Cpf = "111.111.111-11", Email = "joao@centro.com.br", Telefone = "(11) 99999-0001", Franqueado = true },
                    new Responsavel { Nome = "Maria Souza", Cpf = "222.222.222-22", Email = "maria@centro.com.br", Telefone = "(11) 99999-0002", Franqueado = false }
                }
            };
            var unidadeZonaSul = new UnidadeFranqueada
            {
                Nome = "Franquia Zona Sul",
                Cnpj = "00111222000102",
                Cidade = "Sao Paulo",
                Estado = "SP",
                Endereco = "Av. Santo Amaro, 2000",
                Telefone = "(11) 3100-0102",
                Email = "zonasul@cafebom.com.br",
                DataInicio = new DateTime(2024, 6, 15),
                PercentualRoyalty = 5m,
                Ativa = true,
                FranqueadoraId = franqueadora.Id,
                Responsaveis = new List<Responsavel>
                {
                    new Responsavel { Nome = "Carlos Lima", Cpf = "333.333.333-33", Email = "carlos@zonasul.com.br", Telefone = "(11) 99999-0003", Franqueado = true }
                }
            };
            var unidadeCampinas = new UnidadeFranqueada
            {
                Nome = "Franquia Campinas",
                Cnpj = "00111222000103",
                Cidade = "Campinas",
                Estado = "SP",
                Endereco = "Rua Barao de Jaguara, 500",
                Telefone = "(19) 3100-0103",
                Email = "campinas@cafebom.com.br",
                DataInicio = new DateTime(2025, 1, 10),
                PercentualRoyalty = 6m,
                Ativa = true,
                FranqueadoraId = franqueadora.Id,
                Responsaveis = new List<Responsavel>
                {
                    new Responsavel { Nome = "Ana Martins", Cpf = "444.444.444-44", Email = "ana@campinas.com.br", Telefone = "(19) 99999-0004", Franqueado = true }
                }
            };
            var unidadeLitoral = new UnidadeFranqueada
            {
                Nome = "Franquia Litoral",
                Cnpj = "00111222000104",
                Cidade = "Santos",
                Estado = "SP",
                Endereco = "Av. Ana Costa, 300",
                Telefone = "(13) 3100-0104",
                Email = "litoral@cafebom.com.br",
                DataInicio = new DateTime(2023, 9, 1),
                PercentualRoyalty = 5m,
                Ativa = false,
                FranqueadoraId = franqueadora.Id,
                Responsaveis = new List<Responsavel>
                {
                    new Responsavel { Nome = "Pedro Rocha", Cpf = "555.555.555-55", Email = "pedro@litoral.com.br", Telefone = "(13) 99999-0005", Franqueado = true }
                }
            };
            contexto.Unidades.AddRange(unidadeCentro, unidadeZonaSul, unidadeCampinas, unidadeLitoral);
            contexto.SaveChanges();

            // ----- Usuarios -----
            contexto.Usuarios.AddRange(
                new Usuario
                {
                    Nome = "Administrador Geral",
                    Email = "admin@franquias.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Perfil = Perfil.AdministradorFranqueadora,
                    Ativo = true
                },
                new Usuario
                {
                    Nome = "Gestor Centro",
                    Email = "gestor.centro@franquias.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Gestor@123"),
                    Perfil = Perfil.GestorUnidade,
                    Ativo = true,
                    UnidadeFranqueadaId = unidadeCentro.Id
                },
                new Usuario
                {
                    Nome = "Operador Centro",
                    Email = "operador.centro@franquias.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Operador@123"),
                    Perfil = Perfil.Operador,
                    Ativo = true,
                    UnidadeFranqueadaId = unidadeCentro.Id
                },
                new Usuario
                {
                    Nome = "Gestor Zona Sul",
                    Email = "gestor.zonasul@franquias.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("Gestor@123"),
                    Perfil = Perfil.GestorUnidade,
                    Ativo = true,
                    UnidadeFranqueadaId = unidadeZonaSul.Id
                });
            contexto.SaveChanges();

            // ----- Estoques -----
            var produtosEstoque = new[] { prodCafe, prodCappuccino, prodAgua, prodPaoQueijo, prodBolo, prodSanduiche };
            var unidadesEstoque = new[] { unidadeCentro, unidadeZonaSul, unidadeCampinas };
            var quantidadesIniciais = new Dictionary<string, int>
            {
                { "Cafe Expresso", 120 },
                { "Cappuccino", 90 },
                { "Agua Mineral 500ml", 200 },
                { "Pao de Queijo", 70 },
                { "Bolo de Chocolate", 12 },
                { "Sanduiche Natural", 45 }
            };
            var minimos = new Dictionary<string, int>
            {
                { "Cafe Expresso", 30 },
                { "Cappuccino", 20 },
                { "Agua Mineral 500ml", 50 },
                { "Pao de Queijo", 20 },
                { "Bolo de Chocolate", 15 },
                { "Sanduiche Natural", 10 }
            };

            var estoques = new List<Estoque>();
            foreach (var unidade in unidadesEstoque)
            {
                foreach (var produto in produtosEstoque)
                {
                    var quantidade = quantidadesIniciais[produto.Nome];
                    if (unidade == unidadeCampinas)
                    {
                        quantidade = quantidade / 4; // unidade mais nova, estoque menor
                    }

                    var estoque = new Estoque
                    {
                        UnidadeFranqueadaId = unidade.Id,
                        ProdutoServicoId = produto.Id,
                        Quantidade = quantidade,
                        QuantidadeMinima = minimos[produto.Nome],
                        Movimentacoes = new List<MovimentacaoEstoque>
                        {
                            new MovimentacaoEstoque
                            {
                                Tipo = TipoMovimentacao.Entrada,
                                Quantidade = quantidade,
                                Observacao = "Carga inicial de estoque",
                                Data = new DateTime(2026, 6, 30)
                            }
                        }
                    };
                    estoques.Add(estoque);
                }
            }
            contexto.Estoques.AddRange(estoques);
            contexto.SaveChanges();

            // ----- Vendas -----
            var administrador = contexto.Usuarios.First(u => u.Email == "admin@franquias.com");
            var operadorCentro = contexto.Usuarios.First(u => u.Email == "operador.centro@franquias.com");
            var gestorZonaSul = contexto.Usuarios.First(u => u.Email == "gestor.zonasul@franquias.com");

            RegistrarVenda(contexto, estoques, unidadeCentro.Id, operadorCentro.Id, new DateTime(2026, 7, 5),
                new[] { (prodCafe, 3), (prodPaoQueijo, 2) });
            RegistrarVenda(contexto, estoques, unidadeCentro.Id, operadorCentro.Id, new DateTime(2026, 7, 18),
                new[] { (prodCappuccino, 2), (prodBolo, 1), (prodAgua, 2) });
            RegistrarVenda(contexto, estoques, unidadeCentro.Id, operadorCentro.Id, new DateTime(2026, 8, 2),
                new[] { (prodCafe, 5), (prodSanduiche, 2) });
            RegistrarVenda(contexto, estoques, unidadeCentro.Id, operadorCentro.Id, new DateTime(2026, 8, 20),
                new[] { (prodCafe, 4), (prodCappuccino, 3), (prodPaoQueijo, 4) });

            RegistrarVenda(contexto, estoques, unidadeZonaSul.Id, gestorZonaSul.Id, new DateTime(2026, 7, 10),
                new[] { (prodCafe, 6), (prodAgua, 4) });
            RegistrarVenda(contexto, estoques, unidadeZonaSul.Id, gestorZonaSul.Id, new DateTime(2026, 8, 12),
                new[] { (prodSanduiche, 3), (prodCappuccino, 2) });

            RegistrarVenda(contexto, estoques, unidadeCampinas.Id, administrador.Id, new DateTime(2026, 8, 15),
                new[] { (prodCafe, 2), (prodPaoQueijo, 1) });

            contexto.SaveChanges();

            // ----- Chamados -----
            contexto.Chamados.AddRange(
                new ChamadoSuporte
                {
                    UnidadeFranqueadaId = unidadeCentro.Id,
                    Titulo = "Maquina de cafe com defeito",
                    Categoria = "Equipamento",
                    Descricao = "A maquina de cafe esta vazando agua.",
                    Prioridade = PrioridadeChamado.Alta,
                    Status = StatusChamado.Aberto,
                    DataAbertura = new DateTime(2026, 8, 25)
                },
                new ChamadoSuporte
                {
                    UnidadeFranqueadaId = unidadeZonaSul.Id,
                    Titulo = "Duvida sobre reposicao de estoque",
                    Categoria = "Operacional",
                    Descricao = "Como solicitar reposicao emergencial de insumos?",
                    Prioridade = PrioridadeChamado.Media,
                    Status = StatusChamado.EmAndamento,
                    DataAbertura = new DateTime(2026, 8, 28)
                },
                new ChamadoSuporte
                {
                    UnidadeFranqueadaId = unidadeCampinas.Id,
                    Titulo = "Erro no sistema de PDV",
                    Categoria = "Sistema",
                    Descricao = "O PDV fechou sozinho durante uma venda.",
                    Prioridade = PrioridadeChamado.Baixa,
                    Status = StatusChamado.Encerrado,
                    DataAbertura = new DateTime(2026, 7, 30),
                    DataEncerramento = new DateTime(2026, 8, 3),
                    Resolucao = "Atualizacao do aplicativo do PDV resolveu o problema."
                });
            contexto.SaveChanges();

            // ----- Royalties -----
            GerarRoyaltySeed(contexto, unidadeCentro, 2026, 7, StatusPagamento.Pago, new DateTime(2026, 8, 5));
            GerarRoyaltySeed(contexto, unidadeCentro, 2026, 8, StatusPagamento.Pendente, null);
            GerarRoyaltySeed(contexto, unidadeZonaSul, 2026, 7, StatusPagamento.Pendente, null);
            contexto.SaveChanges();
        }

        private static void RegistrarVenda(AppDbContext contexto, List<Estoque> estoques, int unidadeId, int usuarioId,
            DateTime data, (ProdutoServico produto, int quantidade)[] itens)
        {
            var venda = new Venda
            {
                UnidadeFranqueadaId = unidadeId,
                UsuarioId = usuarioId,
                Data = data,
                Status = StatusVenda.Confirmada
            };

            decimal total = 0;
            foreach (var item in itens)
            {
                var subtotal = item.produto.PrecoBase * item.quantidade;
                total += subtotal;

                venda.Itens.Add(new ItemVenda
                {
                    ProdutoServicoId = item.produto.Id,
                    Quantidade = item.quantidade,
                    PrecoUnitario = item.produto.PrecoBase,
                    Subtotal = subtotal
                });

                var estoque = estoques.FirstOrDefault(e => e.UnidadeFranqueadaId == unidadeId && e.ProdutoServicoId == item.produto.Id);
                if (estoque != null)
                {
                    estoque.Quantidade -= item.quantidade;
                    estoque.Movimentacoes.Add(new MovimentacaoEstoque
                    {
                        Tipo = TipoMovimentacao.Saida,
                        Quantidade = item.quantidade,
                        Observacao = "Baixa por venda",
                        Data = data
                    });
                }
            }

            venda.ValorTotal = total;
            contexto.Vendas.Add(venda);
        }

        private static void GerarRoyaltySeed(AppDbContext contexto, UnidadeFranqueada unidade, int ano, int mes,
            StatusPagamento status, DateTime? dataPagamento)
        {
            var inicio = new DateTime(ano, mes, 1);
            var fim = inicio.AddMonths(1);

            var valoresVendas = contexto.Vendas
                .Where(v => v.UnidadeFranqueadaId == unidade.Id
                    && v.Status == StatusVenda.Confirmada
                    && v.Data >= inicio && v.Data < fim)
                .Select(v => v.ValorTotal)
                .ToList();

            var faturamento = valoresVendas.Sum();

            var percentual = unidade.PercentualRoyalty;
            var valor = Math.Round(faturamento * percentual / 100m, 2);

            contexto.Royalties.Add(new Royalty
            {
                UnidadeFranqueadaId = unidade.Id,
                Ano = ano,
                Mes = mes,
                FaturamentoPeriodo = faturamento,
                PercentualAplicado = percentual,
                ValorDevido = valor,
                StatusPagamento = status,
                DataGeracao = fim,
                DataPagamento = dataPagamento
            });
        }
    }
}
