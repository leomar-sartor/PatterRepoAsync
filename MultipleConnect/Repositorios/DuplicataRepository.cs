using Dapper;
using MultipleConnect.Entidades;
using System.Data;

namespace MultipleConnect.Repositorios
{
    public class DuplicataRepository : Repository, IRepositorio<Duplicata>
    {
        private string _select =
            @"SELECT 
	                D.Id,
                    D.UserId,
                    D.DataCriacao,
                    D.DataAlteracao,
                    D.Deletado,
                    D.FaturaId,
                    D.NumeroParcela,
                    D.ValorParcela,
                    D.Vencimento,
                    D.Status,
                    D.Desconto,
                    D.Acrescimo
                FROM 
	                Duplicata D
                ";
        public DuplicataRepository(IConexaoAsync conexao) : base(conexao) => _buscarTodosTemplate = _select;

        //Inserir
        public async Task<long> InserirAsync(Duplicata dominio, IDbTransaction? transaction = null)
        {
            return await InsertAsync("Duplicata", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.FaturaId,
                dominio.NumeroParcela,
                dominio.ValorParcela,
                dominio.Vencimento,
                dominio.Status,
                dominio.Desconto,
                dominio.Acrescimo
            }, transaction);
        }

        //Alterar
        public async Task<long> AlterarAsync(Duplicata dominio, IDbTransaction? transaction = null)
        {
            await UpdateAsync("Duplicata", new
            {
                dominio.Id,
                dominio.UserId,
                dominio.DataCriacao,
                dominio.DataAlteracao,
                dominio.Deletado,
                dominio.FaturaId,
                dominio.NumeroParcela,
                dominio.ValorParcela,
                dominio.Vencimento,
                dominio.Status,
                dominio.Desconto,
                dominio.Acrescimo
            }, transaction);

            return dominio.Id;
        }

        //Salvar
        public async Task<long> SalvarAsync(Duplicata dominio, IDbTransaction? transaction = null)
        {
            var obj = await BuscarAsync(dominio, transaction);
            if (obj == null)
                return await InserirAsync(dominio, transaction);
            else
                return await AlterarAsync(dominio, transaction);
        }

        //Buscar
        public async Task<IEnumerable<Duplicata>> BuscarTodosAsync(IDbTransaction? transaction = null)
        {
            return await _conexao.QueryAsync<Duplicata>(_select, null, transaction);
        }

        public async Task<IEnumerable<Duplicata>> BuscarTodosFilterAsync(SqlBuilder sqlBuilder, int paginaAtual, int totalLinhas)
        {
            var sql = sqlBuilder.AddTemplate($"{_select} /**where**/");

            return await _conexao.QueryAsync<Duplicata>(sql.RawSql, paginaAtual, totalLinhas, sql.Parameters);
        }

        public async Task<Duplicata> BuscarAsync(long Id, IDbTransaction? transaction = null)
        {
            var sqlBuscar = $"{_select} Where D.Id = @Id AND D.Deletado = 0";
            return await _conexao.QueryFirstOrDefaultAsync<Duplicata>(sqlBuscar, new { Id }, transaction);
        }

        public async Task<Duplicata> BuscarAsync(Duplicata dominio, IDbTransaction? transaction = null)
        {
            return await BuscarAsync(dominio.Id, transaction);
        }

        //Excluir
        public async Task ExcluirAsync(long Id, IDbTransaction? transaction = null)
        {
            await DeleteAsync("Duplicata", new { Id }, transaction);
        }

        public async Task ExcluirAsync(Duplicata dominio, IDbTransaction? transaction = null)
        {
            await ExcluirAsync(dominio, transaction);
        }

        //Adicionais
        public async Task<IEnumerable<Duplicata>> BuscarPorFaturaAsync(long faturaId, IDbTransaction? transaction = null)
        {
            var sql = $"{_select} Where D.FaturaId = @FaturaId AND D.Deletado = 0";
            return await _conexao.QueryAsync<Duplicata>(sql, new { FaturaId = faturaId }, transaction);
             
        }
    }
}
