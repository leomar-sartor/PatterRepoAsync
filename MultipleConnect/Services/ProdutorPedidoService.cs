using MultipleConnect.Entidades;
using MultipleConnect.Enuns;
using MultipleConnect.Repositorios;

namespace MultipleConnect.Services
{
    public class ProdutorPedidoService
    {
        public IConexaoAsync _conexao;

        private readonly PedidoRepository _rPedido;
        private readonly MovimentoRepository _rMovimento;
        private readonly FaturaRepository _rFatura;
        private readonly DuplicataRepository _rDuplicata;
        private readonly PagamentoRepository _rPagamento;
        private readonly CaixaRepository _rCaixa;

        public ProdutorPedidoService(IConexaoAsync? conexao = null)
        {
            if (conexao == null)
                _conexao = new ConexaoAsync();
            else
                _conexao = conexao;

            _rPedido = new PedidoRepository(_conexao);
            _rMovimento = new MovimentoRepository(_conexao);
            _rFatura = new FaturaRepository(_conexao);
            _rDuplicata = new DuplicataRepository(_conexao);
            _rPagamento = new PagamentoRepository(_conexao);
            _rCaixa = new CaixaRepository(_conexao);
        }

        public async Task<bool> EstornarPedido(long PedidoId, long UserId, long EmpresaId)
        {
            using (var transaction = await _conexao.BeginTransactionAsync())
            {
                Console.WriteLine("Transacionando...");

                try
                {
                    //ESTORNO
                    var pedido = await _rPedido.BuscarAsync(PedidoId, transaction);
                    var statusPedido = pedido.Status;

                    if (pedido is not null)
                    {
                        pedido.Status = TipoStatusPedido.Cancelado;
                        pedido.Situacao = TipoSituacaoPedido.Cancelado;
                        await _rPedido.AlterarAsync(pedido, transaction);

                        //ForcaErro.DividePorZero();

                        if (statusPedido == TipoStatusPedido.Realizado)
                        {
                            var movimentoBd = await _rMovimento.BuscarPorPedidoAsync(PedidoId, transaction);

                            var estorno = new Movimento();
                            estorno.Estorno = true;
                            estorno.UserId = UserId;
                            estorno.EmpresaId = EmpresaId;
                            estorno.PedidoId = PedidoId;
                            estorno.LoteId = movimentoBd.LoteId;
                            estorno.Identificador = pedido.LoteIdentificador ?? "";
                            estorno.TalhaoId = movimentoBd.TalhaoId;
                            estorno.LocalId = movimentoBd.LocalId;
                            estorno.ClienteId = movimentoBd.ClienteId;
                            estorno.GramaId = movimentoBd.GramaId;
                            estorno.IntervaloDeCorte = 0;
                            estorno.Quantidade = movimentoBd.Quantidade;
                            estorno.Operacao = TipoMovimento.Entrada;

                            //Calcular o Saldo
                            var ultimoSaldo = await _rMovimento.BuscarUltimoMovimentoAsync(movimentoBd.LoteId, movimentoBd.TalhaoId, movimentoBd.LocalId, EmpresaId, transaction);
                            if (ultimoSaldo is null)
                            {
                                estorno.SaldoAnterior = 0;
                                estorno.Saldo = estorno.Quantidade;
                            }
                            else
                            {
                                estorno.SaldoAnterior = ultimoSaldo.Saldo;
                                estorno.Saldo = ultimoSaldo.Saldo + estorno.Quantidade;
                            }

                            await _rMovimento.SalvarAsync(estorno, transaction);

                            //ForcaErro.DividePorZero();
                        }

                        //Verificar se Pedido tem Fatura, duplicata e pagamento
                        //Fatura 
                        var fatura = await _rFatura.BuscarPorPedidoAsync(PedidoId, transaction);
                        if (fatura != null)
                        {
                            fatura.Status = TipoFaturaStatus.Cancelada;
                            await _rFatura.SalvarAsync(fatura, transaction);

                            //ForcaErro.DividePorZero();

                            //Duplicatas
                            var duplicatasEnumerable = await _rDuplicata.BuscarPorFaturaAsync(fatura.Id, transaction);
                            var duplicatas = duplicatasEnumerable.ToList();
                            foreach (var dup in duplicatas)
                            {
                                dup.Status = TipoDuplicataStatus.Cancelada;
                                await _rDuplicata.SalvarAsync(dup, transaction);

                                //ForcaErro.DividePorZero();

                                //Pagamentos 
                                var pagamentosEnumerable = await _rPagamento.BuscarPorDuplicataAsync(dup.Id, transaction);
                                var pagamentos = pagamentosEnumerable.ToList();
                                foreach (var pag in pagamentos)
                                {
                                    pag.Status = TipoPagamentoStatus.Cancelado;
                                    await _rPagamento.SalvarAsync(pag, transaction);

                                    //ForcaErro.DividePorZero();
                                }

                                //Caixa
                                var movsEnumerable = await _rCaixa.BuscarPorPedidoAsync(PedidoId, transaction);
                                var movs = movsEnumerable.ToList();
                                foreach (var mov in movs)
                                {
                                    var estornoCaixa = new Caixa();
                                    estornoCaixa.UserId = UserId;
                                    estornoCaixa.EmpresaId = EmpresaId;
                                    estornoCaixa.FormaPagamentoId = mov.FormaPagamentoId;
                                    estornoCaixa.NumeroParcela = mov.NumeroParcela;
                                    estornoCaixa.ChequeId = mov.ChequeId;
                                    estornoCaixa.PedidoId = mov.PedidoId;
                                    estornoCaixa.Operacao = mov.Operacao == TipoMovimento.Entrada ? TipoMovimento.Saida : TipoMovimento.Entrada;
                                    estornoCaixa.Estorno = true;
                                    estornoCaixa.Quantidade = mov.Quantidade;
                                    estornoCaixa.ValorUnitario = mov.ValorUnitario;
                                    estornoCaixa.ValorTotal = mov.ValorTotal;
                                    estornoCaixa.Obs = "Estorno por Cancelamento de Pedido";
                                    estornoCaixa.ContaId = mov.ContaId;

                                    //Saldo Geral
                                    var ultimoRegistro = await _rCaixa.BuscarUltimoAsync(transaction);
                                    if (ultimoRegistro == null)
                                    {
                                        estornoCaixa.SaldoCaixa = mov.ValorTotal;
                                        estornoCaixa.SaldoCaixaAnterior = 0;
                                    }
                                    else
                                    {
                                        estornoCaixa.SaldoCaixa = ultimoRegistro.SaldoCaixa - mov.ValorTotal;
                                        estornoCaixa.SaldoCaixaAnterior = ultimoRegistro.SaldoCaixa;
                                    }

                                    //Saldo por Conta
                                    var ultimoRegistroConta = await _rCaixa.BuscarUltimoPorContaAsync(mov.ContaId ?? 0, transaction);

                                    if (ultimoRegistroConta == null)
                                    {
                                        estornoCaixa.SaldoConta = mov.ValorTotal;
                                    }
                                    else
                                    {
                                        estornoCaixa.SaldoConta = ultimoRegistroConta.SaldoConta - mov.ValorTotal;
                                    }

                                    await _rCaixa.SalvarAsync(estornoCaixa, transaction);

                                    ForcaErro.DividePorZero();
                                }
                            }
                        }

                        ForcaErro.DividePorZero();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Treta Mano! - {ex.Message}");
                    Console.WriteLine("Mandar Erro");
                    return false;
                }
                finally
                {
                    Console.WriteLine("Terminando Sempre!");
                }

                return true;
            }
        }
    }
}
