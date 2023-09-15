using Dapper;
using MultipleConnect.Entidades;
using System.Data;

namespace MultipleConnect.Repositorios
{
    public class PagamentoRepository : Repository, IRepositorio<Pagamento>
    {
        private string _select =
            @"SELECT 
	                P.Id,
                    P.EmpresaId,
                    P.UserId,
                    P.Valor,
                    P.Acrescimo,
                    P.Desconto,
                    P.Obs,
                    P.DataCriacao,
                    P.DataAlteracao,
                    P.Deletado,
                    P.DuplicataId,
                    P.Status
                FROM 
	                Pagamento P
                ";
        public PagamentoRepository(IConexaoAsync conexao) : base(conexao) => _buscarTodosTemplate = _select;

        //Inserir
        public async Task<long> InserirAsync(Pagamento dominio, IDbTransaction? transaction = null)
        {
            return await InsertAsync("Pagamento", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.EmpresaId,
                dominio.Valor,
                dominio.Acrescimo,
                dominio.Desconto,
                Obs = dominio.Observacao,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.DuplicataId,
                dominio.Status
            }, transaction);
        }

        //Alterar
        public async Task<long> AlterarAsync(Pagamento dominio, IDbTransaction? transaction = null)
        {
            await UpdateAsync("Pagamento", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.EmpresaId,
                dominio.Valor,
                dominio.Acrescimo,
                dominio.Desconto,
                Obs = dominio.Observacao,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.DuplicataId,
                dominio.Status
            }, transaction);

            return dominio.Id;
        }

        //Salvar
        public async Task<long> SalvarAsync(Pagamento dominio, IDbTransaction? transaction = null)
        {
            var obj = await BuscarAsync(dominio, transaction);
            if (obj == null)
                return await InserirAsync(dominio, transaction);
            else
                return await AlterarAsync(dominio, transaction);
        }

        //Buscar
        public async Task<IEnumerable<Pagamento>> BuscarTodosAsync(IDbTransaction? transaction = null)
        {
            return await _conexao.QueryAsync<Pagamento>(_select, null, transaction);
        }

        public async Task<IEnumerable<Pagamento>> BuscarTodosFilterAsync(SqlBuilder sqlBuilder, int paginaAtual, int totalLinhas)
        {
            var sql = sqlBuilder.AddTemplate($"{_select} /**where**/");

            return await _conexao.QueryAsync<Pagamento>(sql.RawSql, paginaAtual, totalLinhas, sql.Parameters);
        }

        public async Task<Pagamento> BuscarAsync(long Id, IDbTransaction? transaction = null)
        {
            var sqlBuscar = $"{_select} Where P.Id = @Id AND P.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Pagamento>(sqlBuscar, new { Id }, transaction);
        }

        public async Task<Pagamento> BuscarAsync(Pagamento dominio, IDbTransaction? transaction = null)
        {
            return await BuscarAsync(dominio.Id, transaction);
        }

        //Excluir
        public async Task ExcluirAsync(long Id, IDbTransaction? transaction = null)
        {
            await DeleteAsync("Pagamento", new { Id }, transaction);
        }

        public async Task ExcluirAsync(Pagamento dominio, IDbTransaction? transaction = null)
        {
            await ExcluirAsync(dominio, transaction);
        }

        //Adicionais
        public async Task<IEnumerable<Pagamento>> BuscarPorDuplicataAsync(long DuplicateId, IDbTransaction? transaction = null)
        {
            var sql = $"{_select} Where P.DuplicataId = @DuplicataId AND P.Deletado = 0";
            return await _conexao.QueryAsync<Pagamento>(sql, new { DuplicataId = DuplicateId }, transaction);
        }

    }
}
