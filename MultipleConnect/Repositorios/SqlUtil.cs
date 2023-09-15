using System.Data;

namespace MultipleConnect.Repositorios
{
    public class SqlUtil
    {
        public IConexaoAsync _conexao;

        public SqlUtil(IConexaoAsync conexao)
        {
            _conexao = conexao;
        }

        public async Task<long> InsertAsync(string nomeTabela, object colunas, IDbTransaction? transacao = null)
        {
            var setColunas = new List<string>();
            var setValores = new List<string>();
            var objColunas = colunas.GetType().GetProperties();

            foreach (var coluna in objColunas)
            {
                setColunas.Add(coluna.Name);
                setValores.Add($"@{coluna.Name}");
            }

            var sql = $"INSERT INTO {nomeTabela} ({string.Join(",", setColunas)}) VALUES ({string.Join(",", setValores)})";

            return await _conexao.ExecuteAsync<long>(sql, colunas, transacao);
        }

        public async Task UpdateAsync(string nomeTabela, object colunas, IDbTransaction? transacao = null, string chave = "Id" )
        {
            var setColunas = new List<string>();
            var objColunas = colunas.GetType().GetProperties();

            foreach (var coluna in objColunas)
            {
                var nome = coluna.Name;
                if (nome == chave)
                    continue;

                setColunas.Add($"{nome} = @{nome}");
            }

            var sql = $"UPDATE {nomeTabela} SET {string.Join(",", setColunas)} WHERE {chave} = @{chave}";

            await _conexao.ExecuteAsync(sql, colunas, transacao);
        }

        public async Task DeleteAsync(string nomeTabela, object colunas, IDbTransaction? transacao = null, string chave = "Id")
        {
            var sql = $"DELETE FROM {nomeTabela} WHERE {chave} = @{chave}";
            await _conexao.ExecuteAsync(sql, colunas, transacao);
        }

    }
}
