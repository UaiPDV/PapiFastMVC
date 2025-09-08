using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BixWeb.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http.Headers;

namespace BixWeb.Controllers
{
    public class SincronizarController : Controller
    {
        private readonly DbPrint _context;

        public SincronizarController(DbPrint context)
        {
            _context = context;
        }

        [Authorize(Roles = "Gerente,Funcionario")]
        public async Task<IActionResult> Sincronizar(int atualizar)
        {
            try
            {
                if (atualizar > 0) 
                {
                    var userId = (User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                    if (userId == null)
                    {
                        ViewData["Erro"] = "Erro ao identificar o usuário";
                        return View();
                    }

                    int codUsuario = int.Parse(userId);

                    LoginAPI? loginAPI = _context.LoginAPI.Where(s => s.codUsuario == codUsuario).FirstOrDefault();
                    if (loginAPI != null)
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/token");

                        var collection = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("username", loginAPI.username),
                        new KeyValuePair<string, string>("password", loginAPI.password),
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("client_id", loginAPI.client_id),
                        new KeyValuePair<string, string>("client_secret", loginAPI.client_secret)
                    };

                        var content = new FormUrlEncodedContent(collection);
                        request.Content = content;

                        var response = await client.SendAsync(request);
                        response.EnsureSuccessStatusCode();

                        string retorno = await response.Content.ReadAsStringAsync();

                        var jsonDocument = JsonDocument.Parse(retorno);
                        var token = jsonDocument.RootElement.GetProperty("access_token").GetString();
                        if (token == null)
                        {
                            ViewData["Erro"] = "Erro ao obter as credências de sincronização.";
                            return View();
                        }
                        if (atualizar == 1)
                        {
                            ViewData["SincroCategoria"] = SincroCategoria(token);
                            ViewData["SincroProduto"] = SincroProduto(token);
                            ViewData["SincroFilial"] = SincroFilial(token, codUsuario);
                            ViewData["SincroPreco"] = SincroPreco(token, codUsuario);
                            ViewData["SincroModificadores"] = SincroModificadores(token, codUsuario);

                            return View();

                        }
                        else
                        {
                            ViewData["SincroPreco"] = AtualizarProdutos(token);
                            ViewData["SincroProduto"] = AtualizarPrecos(token, codUsuario);
                            return View();
                        }
                    }
                    else
                    {
                        ViewData["Erro"] = "Erro ao obter as credências de sincronização.";
                        return View();
                    }
                }
                else
                {
                    return View();

                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar Sincronizar: {ex}");
                ViewData["Erro"] = "Erro ao sincronização.";
                return View();
            }
        }
        [Authorize(Roles = "Admin")]
        // Sincronizar Categorias
        public string SincroCategoria(string? token)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/grupoprodutos/get");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = client.Send(request);
                response.EnsureSuccessStatusCode();

                string retorno = response.Content.ReadAsStringAsync().Result;
                retorno = retorno.Replace("codigo", "codCategoria");
                var categorias = JsonSerializer.Deserialize<List<Categoria>>(retorno);
                int totalAtualizados = 0;
                int totalAdd = 0;
                if (categorias != null)
                {
                    foreach (Categoria categoria in categorias)
                    {
                        var existingCategoria = _context.Categorias.Find(categoria.codCategoria);
                        if (existingCategoria != null)
                        {
                            // Atualiza se houver alteração
                            if (existingCategoria.nome != categoria.nome ||
                                existingCategoria.corIcone != categoria.corIcone ||
                                existingCategoria.codRefExterna != categoria.codRefExterna ||
                                existingCategoria.ativo != categoria.ativo)
                            {
                                existingCategoria = categoria;
                                _context.Categorias.Update(existingCategoria);
                                _context.SaveChanges();
                                totalAtualizados++;
                            }
                        }
                        else
                        {
                            existingCategoria= null;

                            _context.Categorias.Add(categoria);

                            _context.Database.OpenConnection();
                            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Categorias ON");

                            _context.SaveChanges();
                            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Categorias OFF");
                            _context.Database.CloseConnection();
                            totalAdd++;
                        }
                    }
                    string parte1 = "Categorias: ";
                    if (totalAtualizados == 1 )
                    {
                        parte1 = parte1+ "1 categoria atualizada, ";
                    }
                    else if(totalAtualizados>1)
                    {
                        parte1 = parte1 + totalAtualizados+" categorias atualizadas, ";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhuma categoria foi atualizada. ";
                    }
                    
                    if (totalAdd == 1)
                    {
                        parte1 = parte1 + "1 categoria adicionada.";
                    }
                    else if(totalAdd > 1)
                    {
                        parte1 = parte1 + totalAdd + " categorias adicionadas";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhuma categoria foi adicionada.";
                    }
                    return parte1;
                }
                else
                {
                    return "Nenhuma categoria encontrada para sincronizar.";
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar SincroCategoria: {ex}");
                return "Ocorreu algum erro ao sincronizar as categorias!";
            }

        }
        [Authorize(Roles = "Admin")]
        public string SincroProduto(string? token) 
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/produtos/get");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = client.Send(request);
                response.EnsureSuccessStatusCode();

                string retorno = response.Content.ReadAsStringAsync().Result;
                var produtosUai = JsonSerializer.Deserialize<List<ProdutoUai>>(retorno);
                int totalAtualizados = 0;
                int totalAdd = 0;

                if (produtosUai!=null)
                {
                    foreach (var produto in produtosUai)
                    {
                        
                        var existeProduto = _context.Produtos.Find(produto.codigo);
                        if (existeProduto != null) {
                            // Atualiza se houver alteração
                            if (existeProduto.nomeProduto != produto.descricaoCupom ||
                                existeProduto.descricaoDetalhada != produto.descricaoDetalhada ||
                                existeProduto.codCategoria != produto.codGrupo ||
                                existeProduto.imagem != produto.imagem ||
                                existeProduto.chamaModificadores != produto.chamaModificadores)
                            {
                                totalAtualizados++;
                                existeProduto.nomeProduto = produto.descricaoDetalhada;
                                existeProduto.descricaoDetalhada= produto.descricaoDetalhada;
                                existeProduto.codCategoria = produto.codGrupo;
                                existeProduto.imagem = produto.imagem;
                                existeProduto.chamaModificadores = produto.chamaModificadores;
                                _context.Produtos.Update(existeProduto);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            Categoria? categoria = _context.Categorias.Find(produto.codGrupo);
                            if (categoria!=null)
                            {
                                existeProduto = null;
                                Produto newProduto = new Produto
                                {
                                    codProduto = produto.codigo,
                                    descricaoDetalhada = produto.descricaoDetalhada,
                                    nomeProduto = produto.descricaoCupom,
                                    codCategoria = produto.codGrupo,
                                    imagem = produto.imagem,
                                    chamaModificadores = produto.chamaModificadores,
                                };

                                _context.Produtos.Add(newProduto);

                                _context.Database.OpenConnection();
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Produtos ON");
                                _context.SaveChanges();
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Produtos OFF");
                                _context.SaveChanges();
                                _context.Database.CloseConnection();
                                totalAdd++;
                            }
                        }
                    }
                    string parte1 = "Produtos: ";
                    if (totalAtualizados == 1)
                    {
                        parte1 = parte1 + "1 produto atualizado, ";
                    }
                    else if (totalAtualizados > 1)
                    {
                        parte1 = parte1 + totalAtualizados + " produtos atualizados, ";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhum produto foi atualizado. ";
                    }

                    if (totalAdd == 1)
                    {
                        parte1 = parte1 + "1 produto adicionado.";
                    }
                    else if (totalAdd > 1)
                    {
                        parte1 = parte1 + totalAdd + " produtos adicionados";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhum produto foi adicionado.";
                    }
                    return parte1;
                }
                else
                {
                    return "Nenhum produto encontrado para sincronizar";
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar SincroProduto: {ex}");
                return "Ocorreu algum erro ao sincronizar os produtos!";
            }
        }
        [Authorize(Roles = "Admin")]
        public string SincroFilial(string? token, int codUsuario) 
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/filial/get");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = client.Send(request);
                response.EnsureSuccessStatusCode();

                string retorno = response.Content.ReadAsStringAsync().Result;
                var filiais = JsonSerializer.Deserialize<List<FilialUai>>(retorno);
                
                int totalAtualizados = 0;
                int totalAdd = 0;

                if (filiais != null) 
                {
                    foreach (var filial in filiais)
                    {
                        request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/oem/configuracoes/"+filial.codigo);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        response = client.Send(request);
                        response.EnsureSuccessStatusCode();

                        if (!string.IsNullOrWhiteSpace(filial.cnpj) || filial.cnpj.Length == 14)
                        {
                            filial.cnpj =Convert.ToUInt64(filial.cnpj).ToString(@"00\.000\.000\/0000\-00");
                        }
                        
                        retorno = response.Content.ReadAsStringAsync().Result;
                        var imagens = JsonSerializer.Deserialize<ImagemPDV>(retorno);
                        
                        Filial? filialexiste = _context.Filiais.Where(s => s.codFilial == filial.codigo).Include(e => e.Endereco).FirstOrDefault();
                        
                        if (filialexiste != null && imagens !=null)
                        {
                            if (filialexiste.nome != filial.nome || filialexiste.telefone != filial.telefone ||
                                filialexiste.cnpj != filial.cnpj || filialexiste.email != filial.email ||
                                filialexiste.logoHome!=imagens.logoTablet || filialexiste.logoPDV!=imagens.logoPDV)
                            {
                                filialexiste.email = filial.email;
                                filialexiste.nome = filial.nome;
                                filialexiste.telefone = filial.telefone;
                                filialexiste.cnpj = filial.cnpj;
                                filialexiste.logoHome= imagens.logoTablet;
                                filialexiste.logoPDV=imagens.logoPDV;

                                if (filialexiste.Endereco != null)
                                {
                                    if (filialexiste.Endereco.cep != filial.codigomunicipio.ToString() || filialexiste.Endereco.cidade != filial.cidade ||
                                        filialexiste.Endereco.bairro != filial.bairro || filialexiste.Endereco.logradouro != filial.ender ||
                                        filialexiste.Endereco.numeroendereco != int.Parse(filial.numeroendereco))
                                    {
                                        filialexiste.Endereco.cidade = filial.cidade;
                                        filialexiste.Endereco.bairro = filial.bairro;
                                        filialexiste.Endereco.estado = filial.estado;
                                        filialexiste.Endereco.logradouro = filial.ender;
                                        filialexiste.Endereco.numeroendereco = int.Parse(filial.numeroendereco);
                                    }

                                    _context.Filiais.Update(filialexiste);
                                    _context.SaveChanges();
                                    totalAtualizados++;
                                }
                            }
                        }
                        else
                        {
                            filialexiste=null;
                            Endereco endereco = new Endereco
                            {
                                estado = filial.estado,
                                cidade = filial.cidade,
                                bairro = filial.bairro,
                                cep = filial.codigomunicipio.ToString(),
                                logradouro = filial.ender,
                                numeroendereco = int.Parse(filial.numeroendereco) ,
                            };
                            
                            _context.Enderecos.Add(endereco);
                            _context.SaveChanges();
                            
                            Filial filialadd = new Filial
                            {
                                codFilial = filial.codigo,
                                nome = filial.nome,
                                cnpj = filial.cnpj,
                                telefone = filial.telefone,
                                email = filial.email,
                                codEndereco = endereco.codEndereco
                            };

                            if (imagens != null)
                            {
                                filialadd.logoPDV=imagens.logoPDV;
                                filialadd.logoHome = imagens.logoTablet;
                            }

                            _context.Filiais.Add(filialadd);
                            _context.Database.OpenConnection();
                            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Filiais ON");
                            _context.SaveChanges();
                            _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Filiais OFF");
                            _context.SaveChanges();
                            _context.Database.CloseConnection();

                            totalAdd++;
                            UsuarioFilial usuarioFilial = new UsuarioFilial
                            {
                                codFilial = filial.codigo,
                                codUsuario = codUsuario,
                                ativo = true,
                                cargoUsuario = "Dono",
                                tipoUsuario = "Gerente",
                                dataCadastro= DateTime.Now,
                            };
                            _context.UsuarioFiliais.Add(usuarioFilial);
                            _context.SaveChanges();
                        }
                    }
                    string parte1 = "Filiais: ";
                    if (totalAtualizados == 1)
                    {
                        parte1 = parte1 + "1 filial atualizada, ";
                    }
                    else if (totalAtualizados > 1)
                    {
                        parte1 = parte1 + totalAtualizados + " filiais atualizadas, ";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhuma filial foi atualizada. ";
                    }

                    if (totalAdd == 1)
                    {
                        parte1 = parte1 + "1 filial adicionada.";
                    }
                    else if (totalAdd > 1)
                    {
                        parte1 = parte1 + totalAdd + " filiais adicionadas";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhuma filial foi adicionada.";
                    }
                    return parte1;
                }
                else
                {
                    return "Nenhuma filial encontrada para sincronizar";
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar SincroFilial: {ex}");
                return "Ocorreu algum erro ao sincronizar as filiais!";
            }
        }
        [Authorize(Roles = "Admin")]
        public string SincroPreco(string? token, int codUsuario) 
        {
            try
            {
                List<UsuarioFilial> filiais = _context.UsuarioFiliais.Where(s => s.codUsuario == codUsuario).ToList();
                int totalAtualizados = 0;
                int totalAdd = 0;
                if (filiais.Count>0)
                {
                    foreach (var filial in filiais)
                    {
                        var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/tabelapreco/get/" + filial.codFilial);
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                        var response = client.Send(request);
                        response.EnsureSuccessStatusCode();

                        string retorno = response.Content.ReadAsStringAsync().Result;
                        var precos = JsonSerializer.Deserialize<List<Preco>>(retorno);

                        if (precos != null)
                        {
                            foreach (var preco in precos)
                            {
                                Preco? precoExiste = _context.Precos.Where(s => s.codProduto == preco.codProduto && s.codFilial == preco.codFilial).FirstOrDefault();
                                if (precoExiste != null)
                                {
                                    if (precoExiste.valor != preco.valor)
                                    {
                                        precoExiste.valor = preco.valor;
                                        _context.Precos.Update(preco);
                                        _context.SaveChanges();
                                        totalAtualizados++;
                                    }
                                }
                                else
                                {
                                    precoExiste=null;
                                    _context.Precos.Add(preco);
                                    _context.SaveChanges();
                                    totalAdd++;
                                }
                            }
                        }
                    }
                    string parte1 = "Preços: ";
                    if (totalAtualizados == 1)
                    {
                        parte1 = parte1 + "1 preço atualizado, ";
                    }
                    else if (totalAtualizados > 1)
                    {
                        parte1 = parte1 + totalAtualizados + " preços atualizados, ";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhum preço foi atualizado. ";
                    }

                    if (totalAdd == 1)
                    {
                        parte1 = parte1 + "1 preço adicionado.";
                    }
                    else if (totalAdd > 1)
                    {
                        parte1 = parte1 + totalAdd + " preços adicionados";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhum preço foi adicionado.";
                    }
                    return parte1;
                }
                else
                {
                    return "Nenhum preço encontrado para sincronizar";
                }
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar SincroPreco: {ex}");
                return "Ocorreu algum erro ao sincronizar os preços!";
            }
        }
        [Authorize(Roles = "Admin")]
        public string SincroModificadores(string? token, int codUsuario)
        {
            try
            {
                List<UsuarioFilial> filiais = _context.UsuarioFiliais.Where(s => s.codUsuario == codUsuario).ToList();
                int totalAtualizados = 0;
                int totalAdd = 0;
                foreach (var filial in filiais)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/modificador/get/" + filial.codFilial + "/00000000000000" + filial.codFilial);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = client.Send(request);
                    response.EnsureSuccessStatusCode();

                    string retorno = response.Content.ReadAsStringAsync().Result;
                    var modificadores = JsonSerializer.Deserialize<List<Modificador>>(retorno);

                    if (modificadores != null)
                    {
                        foreach (var modificador in modificadores)
                        {
                            Modificador? existingModificador = _context.Modificadores.Find(modificador.codigo);
                            if (existingModificador != null)
                            {
                                bool isUpdated = false;

                                // Verifica se há mudanças nos campos e atualiza se necessário
                                if (existingModificador.classificacaoGrupo != modificador.classificacaoGrupo)
                                {
                                    existingModificador.classificacaoGrupo = modificador.classificacaoGrupo;
                                    isUpdated = true;
                                }
                                if (existingModificador.ordemGrupo != modificador.ordemGrupo)
                                {
                                    existingModificador.ordemGrupo = modificador.ordemGrupo;
                                    isUpdated = true;
                                }
                                if (existingModificador.descricaoGrupo != modificador.descricaoGrupo)
                                {
                                    existingModificador.descricaoGrupo = modificador.descricaoGrupo;
                                    isUpdated = true;
                                }
                                if (existingModificador.ordemModificador != modificador.ordemModificador)
                                {
                                    existingModificador.ordemModificador = modificador.ordemModificador;
                                    isUpdated = true;
                                }
                                if (existingModificador.codEmpresa != modificador.codEmpresa)
                                {
                                    existingModificador.codEmpresa = modificador.codEmpresa;
                                    isUpdated = true;
                                }
                                if (existingModificador.codmodificador != modificador.codmodificador)
                                {
                                    existingModificador.codmodificador = modificador.codmodificador;
                                    isUpdated = true;
                                }
                                if (existingModificador.classificacao != modificador.classificacao)
                                {
                                    existingModificador.classificacao = modificador.classificacao;
                                    isUpdated = true;
                                }
                                if (existingModificador.preco != modificador.preco)
                                {
                                    existingModificador.preco = modificador.preco;
                                    isUpdated = true;
                                }
                                if (existingModificador.ativo != modificador.ativo)
                                {
                                    existingModificador.ativo = modificador.ativo;
                                    isUpdated = true;
                                }
                                if (existingModificador.descricaoModificador != modificador.descricaoModificador)
                                {
                                    existingModificador.descricaoModificador = modificador.descricaoModificador;
                                    isUpdated = true;
                                }
                                if (existingModificador.qtdMinima != modificador.qtdMinima)
                                {
                                    existingModificador.qtdMinima = modificador.qtdMinima;
                                    isUpdated = true;
                                }
                                if (existingModificador.qtdMaxima != modificador.qtdMaxima)
                                {
                                    existingModificador.qtdMaxima = modificador.qtdMaxima;
                                    isUpdated = true;
                                }
                                if (existingModificador.codproduto != modificador.codproduto)
                                {
                                    existingModificador.codproduto = modificador.codproduto;
                                    isUpdated = true;
                                }
                                if (existingModificador.codGrupo != modificador.codGrupo)
                                {
                                    existingModificador.codGrupo = modificador.codGrupo;
                                    isUpdated = true;
                                }
                                if (existingModificador.codfilial != modificador.codfilial)
                                {
                                    existingModificador.codfilial = modificador.codfilial;
                                    isUpdated = true;
                                }

                                // Se houver alguma atualização, salva as mudanças
                                if (isUpdated)
                                {
                                    _context.Modificadores.Update(existingModificador);
                                    _context.SaveChangesAsync();
                                }

                            }
                            else
                            {
                                _context.Modificadores.Add(modificador);
                                _context.Database.OpenConnection();
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Modificadores ON");
                                _context.SaveChanges();
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Modificadores OFF");
                                _context.Database.CloseConnection();
                                totalAdd++;
                            }
                        }
                    }
                }
                string parte1 = "Modificadores: ";
                if (totalAtualizados == 1)
                {
                    parte1 = parte1 + "1 modificador atualizado, ";
                }
                else if (totalAtualizados > 1)
                {
                    parte1 = parte1 + totalAtualizados + " modificadores atualizados, ";
                }
                else
                {
                    parte1 = parte1 + "Nenhum modificador foi atualizado. ";
                }

                if (totalAdd == 1)
                {
                    parte1 = parte1 + "1 modiicador adicionado.";
                }
                else if (totalAdd > 1)
                {
                    parte1 = parte1 + totalAdd + " modificadores adicionados";
                }
                else
                {
                    parte1 = parte1 + "Nenhum modificador foi adicionado.";
                }
                return parte1;
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar SincroModificadores: {ex}");
                return "Ocorreu algum erro ao sincronizar os modificadores!";
            }
        }
        [Authorize(Roles = "Admin")]
        public string AtualizarProdutos(string? token) 
        {
            try
            {
                DateTime date = DateTime.Now;
                string date_str = date.ToString("yyyy-MM-dd");

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/produtos/getbydata/"+ date_str);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = client.Send(request);
                response.EnsureSuccessStatusCode();

                string retorno = response.Content.ReadAsStringAsync().Result;
                var produtosUai = JsonSerializer.Deserialize<List<ProdutoUai>>(retorno);

                int totalAtualizados = 0;
                int totalAdd = 0;

                if (produtosUai != null)
                {
                    foreach (var produto in produtosUai)
                    {

                        var existeProduto = _context.Produtos.Find(produto.codigo);
                        if (existeProduto != null)
                        {
                            // Atualiza se houver alteração
                            if (existeProduto.nomeProduto != produto.descricaoCupom ||
                                existeProduto.descricaoDetalhada != produto.descricaoDetalhada ||
                                existeProduto.codCategoria != produto.codGrupo ||
                                existeProduto.imagem != produto.imagem ||
                                existeProduto.chamaModificadores != produto.chamaModificadores)
                            {
                                totalAtualizados++;
                                existeProduto.nomeProduto = produto.descricaoDetalhada;
                                existeProduto.descricaoDetalhada = produto.descricaoDetalhada;
                                existeProduto.codCategoria = produto.codGrupo;
                                existeProduto.imagem = produto.imagem;
                                existeProduto.chamaModificadores = produto.chamaModificadores;
                                _context.Produtos.Update(existeProduto);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            Categoria? categoria = _context.Categorias.Find(produto.codGrupo);
                            if (categoria != null)
                            {
                                Produto newProduto = new Produto
                                {
                                    codProduto = produto.codigo,
                                    descricaoDetalhada = produto.descricaoDetalhada,
                                    nomeProduto = produto.descricaoCupom,
                                    codCategoria = produto.codGrupo,
                                    imagem = produto.imagem,
                                    chamaModificadores = produto.chamaModificadores,
                                };

                                _context.Produtos.Add(newProduto);
                                _context.Database.OpenConnection();
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Produtos ON");
                                _context.SaveChanges();
                                _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Produtos OFF");
                                _context.SaveChanges();
                                _context.Database.CloseConnection();
                                totalAdd++;
                            }
                        }
                    }
                    string parte1 = "Produtos: ";
                    if (totalAtualizados == 1)
                    {
                        parte1 = parte1 + "1 produto atualizado, ";
                    }
                    else if (totalAtualizados > 1)
                    {
                        parte1 = parte1 + totalAtualizados + " produtos atualizados, ";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhum produto foi atualizado. ";
                    }

                    if (totalAdd == 1)
                    {
                        parte1 = parte1 + "1 produto adicionado.";
                    }
                    else if (totalAdd > 1)
                    {
                        parte1 = parte1 + totalAdd + " produtos adicionados";
                    }
                    else
                    {
                        parte1 = parte1 + "Nenhum produto foi adicionado.";
                    }
                    return parte1;
                }
                return "Ocorreu algum erro ao atualizar os Produtos!";
            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar AtualizarProdutos: {ex}");
                return "Ocorreu algum erro ao sincronizar os produtos!";
            }
        }
        [Authorize(Roles = "Admin")]
        public string AtualizarPrecos(string? token, int codUsuario)
        {
            try
            {
                DateTime date = DateTime.Now;
                string date_str = date.ToString("yyyy-MM-dd");
                List<UsuarioFilial> filiais = _context.UsuarioFiliais.Where(s => s.codUsuario == codUsuario).ToList();
                int totalAtualizados = 0;
                int totalAdd = 0;
                foreach (var filial in filiais)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://api.tabletcloud.com.br/tabelapreco/getbydata/" + filial.codFilial + "/" + date_str);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = client.Send(request);
                    response.EnsureSuccessStatusCode();

                    string retorno = response.Content.ReadAsStringAsync().Result;
                    var precos = JsonSerializer.Deserialize<List<Preco>>(retorno);

                    if (precos != null)
                    {
                        foreach (var preco in precos)
                        {
                            Preco? precoExiste = _context.Precos.Where(s=>s.codProduto== preco.codProduto && s.codFilial==preco.codFilial).FirstOrDefault();
                            if (precoExiste != null)
                            {
                                if (precoExiste.valor != preco.valor)
                                {
                                    precoExiste.valor = preco.valor;
                                    _context.Precos.Update(preco);
                                    _context.SaveChanges();
                                    totalAtualizados++;
                                }
                            }
                            else
                            {
                                _context.Precos.Add(preco);
                                _context.SaveChanges();
                                totalAdd++;
                            }
                        }
                    }
                }
                string parte1 = "Preços: ";
                if (totalAtualizados == 1)
                {
                    parte1 = parte1 + "1 preço atualizado, ";
                }
                else if (totalAtualizados > 1)
                {
                    parte1 = parte1 + totalAtualizados + " preços atualizados, ";
                }
                else
                {
                    parte1 = parte1 + "Nenhum preço foi atualizado. ";
                }

                if (totalAdd == 1)
                {
                    parte1 = parte1 + "1 preço adicionado.";
                }
                else if (totalAdd > 1)
                {
                    parte1 = parte1 + totalAdd + " preços adicionados";
                }
                else
                {
                    parte1 = parte1 + "Nenhum preço foi adicionado.";
                }
                return parte1;

            }
            catch (Exception ex)
            {
                ErrorViewModel.LogError($"Erro ao chamar AtualizarPrecos: {ex}");

                return "Ocorreu algum erro ao sincronizar os preços!";
            }
        }

        // GET: Sincronizar
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var dbPrint = _context.LoginAPI.Include(l => l.usuario);
            return View(await dbPrint.ToListAsync());
        }
        [Authorize(Roles = "Admin")]
        // GET: Sincronizar/Create
        public IActionResult Create()
        {
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "password");
            return View();
        }

        // POST: Sincronizar/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("client_id,username,password,client_secret,codUsuario")] LoginAPI loginAPI)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loginAPI);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "password", loginAPI.codUsuario);
            return View(loginAPI);
        }

        // GET: Sincronizar/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loginAPI = await _context.LoginAPI.FindAsync(id);
            if (loginAPI == null)
            {
                return NotFound();
            }
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "password", loginAPI.codUsuario);
            return View(loginAPI);
        }

        // POST: Sincronizar/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("client_id,username,password,client_secret,codUsuario")] LoginAPI loginAPI)
        {
            if (id != loginAPI.client_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loginAPI);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoginAPIExists(loginAPI.client_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["codUsuario"] = new SelectList(_context.Usuario, "codUsuario", "password", loginAPI.codUsuario);
            return View(loginAPI);
        }

        // GET: Sincronizar/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loginAPI = await _context.LoginAPI
                .Include(l => l.usuario)
                .FirstOrDefaultAsync(m => m.client_id == id);
            if (loginAPI == null)
            {
                return NotFound();
            }

            return View(loginAPI);
        }

        // POST: Sincronizar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var loginAPI = await _context.LoginAPI.FindAsync(id);
            if (loginAPI != null)
            {
                _context.LoginAPI.Remove(loginAPI);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        private bool LoginAPIExists(string id)
        {
            return _context.LoginAPI.Any(e => e.client_id == id);
        }
    }
}
