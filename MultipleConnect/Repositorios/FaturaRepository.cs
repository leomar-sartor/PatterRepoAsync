using Dapper;
using MultipleConnect.Entidades;
using System.Data;

namespace MultipleConnect.Repositorios
{
    public class FaturaRepository : Repository, IRepositorio<Fatura>
    {
        private string _select =
            @"SELECT 
	                F.Id,
                    F.UserId,
                    F.EmpresaId,
                    F.DataCriacao,
                    F.DataAlteracao,
                    F.Deletado,
                    F.ClienteFornecedorId,
                    U.Nome ClienteFornecedorNome,
                    F.ValorTotal,
                    F.Tipo,
                    F.PedidoId,
                    F.FormaPagamentoId,
                    FP.Nome FormaPagamentoNome,
                    F.Status,
                    F.CentroCustoId,
                    F.Observacao
                FROM 
	                Fatura F
                INNER JOIN user U ON U.Id = F.ClienteFornecedorId 
	            INNER JOIN formapagamento FP ON FP.Id  = F.FormaPagamentoId
                ";
        public FaturaRepository(IConexaoAsync conexao) : base(conexao) => _buscarTodosTemplate = _select;

        //Inserir
        public async Task<long> InserirAsync(Fatura dominio, IDbTransaction? transaction = null)
        {
            return await InsertAsync("Fatura", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.EmpresaId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.ClienteFornecedorId,
                dominio.ValorTotal,
                dominio.Tipo,
                dominio.PedidoId,
                dominio.FormaPagamentoId,
                dominio.Status,
                dominio.CentroCustoId,
                dominio.Observacao
            }, transaction);
        }

        //Alterar
        public async Task<long> AlterarAsync(Fatura dominio, IDbTransaction? transaction = null)
        {
            await UpdateAsync("Fatura", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.EmpresaId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.ClienteFornecedorId,
                dominio.ValorTotal,
                dominio.Tipo,
                dominio.PedidoId,
                dominio.FormaPagamentoId,
                dominio.Status,
                dominio.CentroCustoId,
                dominio.Observacao
            }, transaction);

            return dominio.Id;
        }

        //Salvar
        public async Task<long> SalvarAsync(Fatura dominio, IDbTransaction? transaction = null)
        {
            var obj = await BuscarAsync(dominio, transaction);
            if (obj == null)
                return await InserirAsync(dominio, transaction);
            else
                return await AlterarAsync(dominio, transaction);
        }

        //Buscar
        public async Task<IEnumerable<Fatura>> BuscarTodosAsync(IDbTransaction? transaction = null)
        {
            return await _conexao.QueryAsync<Fatura>(_select, null, transaction);
        }

        public async Task<IEnumerable<Fatura>> BuscarTodosFilterAsync(SqlBuilder sqlBuilder, int paginaAtual, int totalLinhas)
        {
            var sql = sqlBuilder.AddTemplate($"{_select} /**where**/");

            return await _conexao.QueryAsync<Fatura>(sql.RawSql, paginaAtual, totalLinhas, sql.Parameters);
        }

        public async Task<Fatura> BuscarAsync(long Id, IDbTransaction? transaction = null)
        {
            var sqlBuscar = $"{_select} Where F.Id = @Id AND F.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Fatura>(sqlBuscar, new { Id }, transaction);
        }

        public async Task<Fatura> BuscarAsync(Fatura dominio, IDbTransaction? transaction = null)
        {
            return await BuscarAsync(dominio.Id, transaction);
        }

        //Excluir
        public async Task ExcluirAsync(long Id, IDbTransaction? transaction = null)
        {
            await DeleteAsync("Caixa", new { Id }, transaction);
        }

        public async Task ExcluirAsync(Fatura dominio, IDbTransaction? transaction = null)
        {
            await ExcluirAsync(dominio, transaction);
        }

        //Adicionais
        public async Task<Fatura> BuscarPorPedidoAsync(long PedidoId, IDbTransaction transaction)
        {
            var sql = $"{_select} Where F.PedidoId = @PedidoId AND F.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Fatura>(sql, new { PedidoId }, transaction);
        }
    }
}
