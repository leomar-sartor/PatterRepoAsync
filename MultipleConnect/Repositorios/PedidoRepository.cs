using Dapper;
using MultipleConnect.Entidades;
using System.Data;

namespace MultipleConnect.Repositorios
{
    public class PedidoRepository : Repository, IRepositorio<Pedido>
    {
        private string _select =
            @"SELECT 
	                P.Id,
                    P.EmpresaId,
                    P.ClienteId,
                    P.LoteId,
                    P.LocalId,
                    TL.Descricao LocalNome,
                    L.TalhaoId,
                    T.Nome TalhaoNome,
                    P.Quantidade,
                    P.ProdutoId,
                    TP.Descricao ProdutoNome,
                    P.GramaId,   
                    TG.Descricao GramaNome,
                    P.DataCriacao,
                    P.DataAlteracao,
                    P.Deletado,
                    P.Status,
                    L.Identificador LoteIdentificador,
                    C.Nome ClienteNome,
                    P.PrecoPedido,
                    P.PrecoFrete,
                    P.PrecoTotalPedido,
                    P.ResponsavelId,
                    P.FormaPagamentoId,
                    FP.Nome FormaPagamentoNome,
                    R.Nome ResponsavelNome,
                    P.Situacao
                FROM 
	                Pedido P
                    INNER JOIN TIPOLOCAL TL ON TL.Id = P.LocalId
                    LEFT JOIN LOTE L ON L.Id = P.LoteId
                    LEFT JOIN TALHAO T ON T.id = L.TalhaoId
                    LEFT JOIN USER C ON C.Id = P.ClienteId
                    LEFT JOIN USER R ON R.Id = P.ResponsavelId
                    LEFT JOIN TipoProduto TP ON TP.Id = P.ProdutoId
                    LEFT JOIN TipoGrama TG ON TG.Id = P.GramaId
                    LEFT JOIN FormaPagamento FP ON FP.Id = P.FormaPagamentoId
                ";
        public PedidoRepository(IConexaoAsync conexao) : base(conexao) => _buscarTodosTemplate = _select;

        //Inserir
        public async Task<long> InserirAsync(Pedido dominio, IDbTransaction? transaction = null)
        {
            return await InsertAsync("Pedido", new
            {
                dominio.Id,
                dominio.EmpresaId,
                dominio.ClienteId,
                dominio.LoteId,
                dominio.LocalId,
                dominio.Quantidade,
                dominio.ProdutoId,
                dominio.GramaId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.Status,
                dominio.PrecoPedido,
                dominio.PrecoFrete,
                dominio.PrecoTotalPedido,
                dominio.ResponsavelId,
                dominio.Situacao,
                dominio.FormaPagamentoId
            }, transaction);
        }

        //Alterar
        public async Task<long> AlterarAsync(Pedido dominio, IDbTransaction? transaction = null)
        {
            await UpdateAsync("Pedido", new
            {
                dominio.Id,
                dominio.EmpresaId,
                dominio.ClienteId,
                dominio.LoteId,
                dominio.LocalId,
                dominio.Quantidade,
                dominio.ProdutoId,
                dominio.GramaId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.Status,
                dominio.PrecoPedido,
                dominio.PrecoFrete,
                dominio.PrecoTotalPedido,
                dominio.ResponsavelId,
                dominio.Situacao,
                dominio.FormaPagamentoId
            }, transaction);

            return dominio.Id;
        }

        //Salvar
        public async Task<long> SalvarAsync(Pedido dominio, IDbTransaction? transaction = null)
        {
            var obj = await BuscarAsync(dominio, transaction);
            if (obj == null)
                return await InserirAsync(dominio, transaction);
            else
                return await AlterarAsync(dominio, transaction);
        }

        //Buscar
        public async Task<IEnumerable<Pedido>> BuscarTodosAsync(IDbTransaction? transaction = null)
        {
            return await _conexao.QueryAsync<Pedido>(_select, null, transaction);
        }

        public async Task<IEnumerable<Pedido>> BuscarTodosFilterAsync(SqlBuilder sqlBuilder, int paginaAtual, int totalLinhas)
        {
            var sql = sqlBuilder.AddTemplate($"{_select} /**where**/");

            return await _conexao.QueryAsync<Pedido>(sql.RawSql, paginaAtual, totalLinhas, sql.Parameters);
        }

        public async Task<Pedido> BuscarAsync(long Id, IDbTransaction? transaction = null)
        {
            var sqlBuscar = $"{_select} Where P.Id = @Id AND P.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Pedido>(sqlBuscar, new { Id }, transaction);
        }

        public async Task<Pedido> BuscarAsync(Pedido dominio, IDbTransaction? transaction = null)
        {
            return await BuscarAsync(dominio.Id, transaction);
        }

        //Excluir
        public async Task ExcluirAsync(long Id, IDbTransaction? transaction = null)
        {
            await DeleteAsync("Pedido", new { Id }, transaction);
        }

        public async Task ExcluirAsync(Pedido dominio, IDbTransaction? transaction = null)
        {
            await ExcluirAsync(dominio, transaction);
        }

        //Adicionais
    }
}
