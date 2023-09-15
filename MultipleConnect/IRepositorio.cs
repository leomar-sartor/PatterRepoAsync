using Dapper;
using System.Data;

namespace MultipleConnect
{
    public interface IRepositorio<T>
    {
        Task<long> InserirAsync(T dominio, IDbTransaction? transaction = null);
        Task<long> AlterarAsync(T dominio, IDbTransaction? transaction = null);
        Task<long> SalvarAsync(T dominio, IDbTransaction? transaction = null);

        Task<T> BuscarAsync(long Id, IDbTransaction? transaction = null);
        Task<T> BuscarAsync(T dominio, IDbTransaction? transaction = null);
        Task<IEnumerable<T>> BuscarTodosAsync(IDbTransaction? transaction = null);
        Task<IEnumerable<T>> BuscarTodosFilterAsync(SqlBuilder sqlBuilder, int paginaAtual, int totalLinhas);

        Task ExcluirAsync(long Id, IDbTransaction? transaction = null);
        Task ExcluirAsync(T dominio, IDbTransaction? transaction = null);
    }
}
