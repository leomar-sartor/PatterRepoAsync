using Dapper;
using MultipleConnect.Entidades;
using System.Data;

namespace MultipleConnect.Repositorios
{
    public class MovimentoRepository : Repository, IRepositorio<Movimento>
    {
        private string _select =
            @"SELECT 
		            M.Id,
                    M.EmpresaId,
                    M.PedidoId,
		            M.LoteId,
                    L.Identificador,
		            M.TalhaoId,
                    M.LocalId,
                    M.GramaId,
                    TP.Descricao LocalNome,
                    T.Nome TalhaoNome,
                    L.TipoProdutoId TipoProdutoId,
                    P.Descricao TipoProdutoNome,
                    M.ClienteId,
                    C.Nome ClienteNome,
		            M.Quantidade,
		            M.Operacao,
		            M.DataCriacao,
		            M.DataAlteracao,
		            M.Deletado,
                    M.SaldoAnterior,
                    M.Saldo,
                    M.UserId,
                    M.Estorno,
                    M.Perca,
                    M.Transferencia,
                    M.Ignorar,
                    M.Obs
	            FROM 
		            Movimento M
		            INNER JOIN Lote L ON L.Id = M.LoteId
                    INNER JOIN Talhao T ON T.Id = M.TalhaoId
                    INNER JOIN TipoLocal TP ON TP.Id = M.LocalId
                    INNER JOIN TipoProduto P ON P.Id = L.TipoProdutoId 
                    LEFT JOIN User C ON C.Id = M.ClienteId
                ";
        public MovimentoRepository(IConexaoAsync conexao) : base(conexao) => _buscarTodosTemplate = _select;

        //Inserir
        public async Task<long> InserirAsync(Movimento dominio, IDbTransaction? transaction = null)
        {
            return await InsertAsync("Movimento", new
            {
                dominio.Id,
                dominio.EmpresaId,
                dominio.PedidoId,
                dominio.LoteId,
                dominio.Identificador,
                dominio.TalhaoId,
                dominio.LocalId,
                dominio.ClienteId,
                dominio.GramaId,
                dominio.IntervaloDeCorte,
                dominio.Quantidade,
                dominio.Operacao,
                dominio.SaldoAnterior,
                dominio.Saldo,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.UserId,
                dominio.Estorno,
                dominio.Perca,
                dominio.Transferencia,
                dominio.Ignorar,
                dominio.Obs
            }, transaction);
        }

        //Alterar
        public async Task<long> AlterarAsync(Movimento dominio, IDbTransaction? transaction = null)
        {
            await UpdateAsync("Movimento", new
            {
                dominio.Id,
                dominio.EmpresaId,
                dominio.PedidoId,
                dominio.LoteId,
                dominio.Identificador,
                dominio.TalhaoId,
                dominio.LocalId,
                dominio.ClienteId,
                dominio.GramaId,
                dominio.IntervaloDeCorte,
                dominio.Quantidade,
                dominio.Operacao,
                dominio.SaldoAnterior,
                dominio.Saldo,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.UserId,
                dominio.Estorno,
                dominio.Perca,
                dominio.Transferencia,
                dominio.Ignorar,
                dominio.Obs
            }, transaction);

            return dominio.Id;
        }

        //Salvar
        public async Task<long> SalvarAsync(Movimento dominio, IDbTransaction? transaction = null)
        {
            var obj = await BuscarAsync(dominio, transaction);
            if (obj == null)
                return await InserirAsync(dominio, transaction);
            else
                return await AlterarAsync(dominio, transaction);
        }

        //Buscar
        public async Task<IEnumerable<Movimento>> BuscarTodosAsync(IDbTransaction? transaction = null)
        {
            return await _conexao.QueryAsync<Movimento>(_select, null, transaction);
        }

        public async Task<IEnumerable<Movimento>> BuscarTodosFilterAsync(SqlBuilder sqlBuilder, int paginaAtual, int totalLinhas)
        {
            var sql = sqlBuilder.AddTemplate($"{_select} /**where**/");

            return await _conexao.QueryAsync<Movimento>(sql.RawSql, paginaAtual, totalLinhas, sql.Parameters);
        }

        public async Task<Movimento> BuscarAsync(long Id, IDbTransaction? transaction = null)
        {
            var sql = $"{_select} Where M.Id = @Id AND M.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Movimento>(sql, new { Id }, transaction);
        }

        public async Task<Movimento> BuscarAsync(Movimento dominio, IDbTransaction? transaction = null)
        {
            return await BuscarAsync(dominio.Id, transaction);
        }

        //Excluir
        public async Task ExcluirAsync(long Id, IDbTransaction? transaction = null)
        {
            await DeleteAsync("Movimento", new { Id }, transaction);
        }

        public async Task ExcluirAsync(Movimento dominio, IDbTransaction? transaction = null)
        {
            await ExcluirAsync(dominio, transaction);
        }

        //Adicionais
        public async Task<Movimento> BuscarPorPedidoAsync(long pedidoId, IDbTransaction transaction)
        {
            var sql = $"{_select} Where M.PedidoId = @PedidoId AND M.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Movimento>(sql, new { PedidoId = pedidoId }, transaction);
        }

        public async Task<Movimento> BuscarUltimoMovimentoAsync(long LoteId, long TalhaoId, long LocalId, long EmpresaId, IDbTransaction transaction)
        {
            return await _conexao.QueryFirstOrDefaultAsync<Movimento>
            (
              @$"{_select} Where
                L.Id = @LoteId 
                AND T.Id = @TalhaoId
                AND M.LocalId = @LocalId 
                AND M.EmpresaId = @EmpresaId
                Order By M.Id DESC",
                 new
                 {
                     LoteId,
                     TalhaoId,
                     LocalId,
                     EmpresaId
                 }
                 , transaction
            );
        }
    }
}
