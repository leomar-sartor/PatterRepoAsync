using Dapper;
using MySqlConnector;
using System.Data;

namespace MultipleConnect
{
    public class ConexaoAsync : IConexaoAsync, IAsyncDisposable
    {
        #region Variaveis
        public string _stringConnection = "server=mysql.meuagro.net;database=meuagro;uid=meuagro;pwd=mr889030;port=3306;Allow User Variables=True;IgnoreCommandTransaction=true";

        public readonly MySqlConnection _connection;
        private ConnectionState _state;

        public string _preSQL { get; set; } = "set session sql_mode='TRADITIONAL';";
        #endregion

        //Construtor
        public ConexaoAsync()
        {
            _connection = new MySqlConnection(_stringConnection);
        }

        //Transaction
        public async Task<IDbTransaction> BeginTransactionAsync()
        {
            await CheckStateAsync();

            return await _connection.BeginTransactionAsync();
        }

        public async Task<IDbTransaction> BeginTransactionAsyncWithLevel(IsolationLevel? level = null)
        {
            await CheckStateAsync();

            if (level != null)
                return await _connection.BeginTransactionAsync((IsolationLevel)level);

            return await _connection.BeginTransactionAsync();
        }

        //Primeiro

        public async Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transacao = null)
        {
            try
            {
                sql = _preSQL + sql.ToLower();

                await CheckStateAsync();

                return await _connection.QueryFirstAsync<T>(sql.ToLower(), param, transacao);
            }
            catch (Exception ex)
            {
                throw new Exception(sql, ex);
            }
        }
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transacao = null)
        {
            try
            {
                sql = _preSQL + sql.ToLower();

                await CheckStateAsync();

                return await _connection.QueryFirstOrDefaultAsync<T>(sql.ToLower(), param, transacao);
            }
            catch (Exception ex)
            {
                throw new Exception(sql, ex);
            }
        }

        //Busca
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transacao = null)
        {
            try
            {
                await CheckStateAsync();

                sql = _preSQL + sql.ToLower();
                return await _connection.QueryAsync<T>(sql.ToLower(), param, transacao);
            }
            catch (Exception ex)
            {
                throw new Exception(sql, ex);
            }
        }

        //Paginacao
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, int pagina, int totalLinhas, object? param = null, short porPagina = 10)
        {
            var offset = 0;
            if (pagina > 1)
                offset = (porPagina * (pagina - 1));

            var totalSQL = _preSQL;
            totalSQL += $"select count(*) from ({sql}) as X";
            totalLinhas = await QueryFirstAsync<int>(totalSQL, param );

            sql = $"{_preSQL}{sql} limit {porPagina} offset {offset}";
            return await QueryAsync<T>(sql, param);
        }

        //Execute
        public async Task ExecuteAsync(string sql, object? param = null, IDbTransaction? transacao = null)
        {
            try
            {
                sql = _preSQL + sql.ToLower();

                await CheckStateAsync();

                await _connection.ExecuteAsync(sql.ToLower(), param, transacao);
            }
            catch (Exception e)
            {
                throw new Exception(sql, e);
            }
        }
        public async Task<long> ExecuteAsync<T>(string sql, object? param = null, IDbTransaction? transacao = null)
        {
            try
            {
                var pontoVirgula = sql[sql.Length - 1] == ';' ? "" : ";";
                sql = _preSQL + sql;
                sql = $"{sql}{pontoVirgula}SELECT LAST_INSERT_ID();";
                var Ids = await _connection.QueryAsync<long>(sql.ToLower(), param, transacao);
                return Ids.Single();
            }
            catch (Exception e)
            {
                throw new Exception(sql, e);
            }
        }

        //Estado Conexão
        public async Task CheckStateAsync()
        {
            _state = _connection.State;

            try
            {
                if (_state == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                    return;
                }

                if (_state == ConnectionState.Connecting)
                {
                    while (_state == ConnectionState.Connecting)
                    {
                        _state = _connection.State;

                        if (_state != ConnectionState.Connecting)
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Deu erro de Conexão com o Banco de Dados!", ex);
            }

        }

        public async ValueTask DisposeAsync()
        {
            await _connection.CloseAsync();

            GC.SuppressFinalize(this);
        }
    }
}
