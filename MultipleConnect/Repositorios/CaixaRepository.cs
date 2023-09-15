using Dapper;
using MultipleConnect.Entidades;
using System.Data;

namespace MultipleConnect.Repositorios
{
    public class CaixaRepository : Repository, IRepositorio<Caixa>
    {
        private string _select =
            @"SELECT 
	                C.Id,
                    C.UserId,
                    C.EmpresaId,
                    C.DataCriacao,
                    C.DataAlteracao,
                    C.Deletado,
                    C.FormaPagamentoId,
                    C.NumeroParcela,
                    C.ChequeId,
                    C.ClienteId,
                    C.CentroCustoId,
                    C.ContaId,
                    C2.Nome ContaNome,
                    C.ContaOrigemId,
                    C.ContaDestinoId,
                    C.PedidoId,
                    C.Operacao,
                    C.Estorno,
                    C.Transferencia,
                    C.Quantidade,
                    C.ValorUnitario,
                    C.ValorTotal,
                    C.SaldoConta,
                    C.SaldoCaixaAnterior,
                    C.SaldoCaixa,
                    C.Obs
                FROM 
	                Caixa C
                INNER JOIN Conta C2 ON C2.Id = C.ContaId
                ";
        public CaixaRepository(IConexaoAsync conexao) : base(conexao) => _buscarTodosTemplate = _select;

        //Inserir
        public async Task<long> InserirAsync(Caixa dominio, IDbTransaction? transaction = null)
        {
            return await InsertAsync("Caixa", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.EmpresaId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.FormaPagamentoId,
                dominio.NumeroParcela,
                dominio.ChequeId,
                dominio.ClienteId,
                dominio.PedidoId,
                dominio.CentroCustoId,
                dominio.ContaId,
                dominio.ContaOrigemId,
                dominio.ContaDestinoId,
                dominio.Operacao,
                dominio.Estorno,
                dominio.Transferencia,
                dominio.Quantidade,
                dominio.ValorUnitario,
                dominio.ValorTotal,
                dominio.SaldoConta,
                dominio.SaldoCaixaAnterior,
                dominio.SaldoCaixa,
                dominio.Obs
            }, transaction);
        }

        //Alterar
        public async Task<long> AlterarAsync(Caixa dominio, IDbTransaction? transaction = null)
        {
            await UpdateAsync("Caixa", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.EmpresaId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.FormaPagamentoId,
                dominio.NumeroParcela,
                dominio.ChequeId,
                dominio.ClienteId,
                dominio.PedidoId,
                dominio.CentroCustoId,
                dominio.ContaId,
                dominio.ContaOrigemId,
                dominio.ContaDestinoId,
                dominio.Operacao,
                dominio.Estorno,
                dominio.Transferencia,
                dominio.Quantidade,
                dominio.ValorUnitario,
                dominio.ValorTotal,
                dominio.SaldoConta,
                dominio.SaldoCaixaAnterior,
                dominio.SaldoCaixa,
                dominio.Obs
            }, transaction);

            return dominio.Id;
        }

        //Salvar
        public async Task<long> SalvarAsync(Caixa dominio, IDbTransaction? transaction = null)
        {
            var obj = await BuscarAsync(dominio, transaction);
            if (obj == null)
                return await InserirAsync(dominio, transaction);
            else
                return await AlterarAsync(dominio, transaction);
        }

        //Buscar
        public async Task<IEnumerable<Caixa>> BuscarTodosAsync(IDbTransaction? transaction = null)
        {
            return await _conexao.QueryAsync<Caixa>(_select, null, transaction);
        }

        public async Task<IEnumerable<Caixa>> BuscarTodosFilterAsync(SqlBuilder sqlBuilder, int paginaAtual, int totalLinhas)
        {
            var sql = sqlBuilder.AddTemplate($"{_select} /**where**/");

            return await _conexao.QueryAsync<Caixa>(sql.RawSql, paginaAtual, totalLinhas, sql.Parameters);
        }

        public async Task<Caixa> BuscarAsync(long Id, IDbTransaction? transaction = null)
        {
            var sqlBuscar = $"{_select} Where C.Id = @Id AND C.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Caixa>(sqlBuscar, new { Id }, transaction);
        }

        public async Task<Caixa> BuscarAsync(Caixa dominio, IDbTransaction? transaction = null)
        {
            return await BuscarAsync(dominio.Id, transaction);
        }

        //Excluir
        public async Task ExcluirAsync(long Id, IDbTransaction? transaction = null)
        {
            await DeleteAsync("Caixa", new { Id }, transaction);
        }

        public async Task ExcluirAsync(Caixa dominio, IDbTransaction? transaction = null)
        {
            await ExcluirAsync(dominio, transaction);
        }

        //Adicionais
        public async Task<Caixa> BuscarUltimoAsync(IDbTransaction? transaction = null)
        {
            var sql = $"{_select} ORDER BY C.Id DESC LIMIT 1";
            return await _conexao.QueryFirstOrDefaultAsync<Caixa>(sql, null, transaction);
        }

        public async Task<Caixa> BuscarUltimoPorContaAsync(long ContaId, IDbTransaction? transaction = null)
        {
            var sql = $"{_select}  Where C.ContaId = @ContaId ORDER BY C.Id DESC LIMIT 1";
            return await _conexao.QueryFirstOrDefaultAsync<Caixa>(sql, new { ContaId }, transaction);
        }

        public async Task<IEnumerable<Caixa>> BuscarPorPedidoAsync(long PedidoId, IDbTransaction? transaction = null)
        {
            var sql = $"{_select} Where C.PedidoId = @PedidoId AND C.Deletado = 0";
            return await _conexao.QueryAsync<Caixa>(sql, new { PedidoId }, transaction);
        }
    }
}
