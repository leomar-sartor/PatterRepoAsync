using System.Data;

namespace MultipleConnect
{
    public interface IConexaoAsync
    {
        //Variaveis
        string _preSQL { get; set; }

        //Transaction
        Task<IDbTransaction> BeginTransactionAsync();

        Task<IDbTransaction> BeginTransactionAsyncWithLevel(IsolationLevel? level = null);

        //Primeiro
        Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transacao = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction ? transacao = null);

        //Busca
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction ? transacao = null);

        //Paginacao
        Task<IEnumerable<T>> QueryAsync<T>(string sql, int pagina, int totalLinhas, object? param = null, short porPagina = 10);

        //Execute
        Task ExecuteAsync(string sql, object? param = null, IDbTransaction? transacao = null);
        Task<long> ExecuteAsync<T>(string sql, object? param = null, IDbTransaction? transacao = null);
    }
}
