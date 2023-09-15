﻿using Dapper;

namespace MultipleConnect.Repositorios
{
    public class Repository : SqlUtil
    {
        protected SqlBuilder.Template? _sql = null;
        protected string _buscarTodosTemplate = "";

        public Repository(IConexaoAsync conexao) : base(conexao) { }
    }
}
