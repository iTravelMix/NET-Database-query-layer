namespace ADO.Query.Helper
{
    using System;
    using System.Data;

    using ADO.Query.Mapper;

    using global::MySql.Data.MySqlClient;

    public sealed class MySql : QueryRunner
    {
        public MySql(IQueryMappers mapper) : base(mapper)
        {
        }

        public override IDbConnection GetConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString)) throw new NullReferenceException("ConnectionString");
            return new MySqlConnection( ConnectionString );
        }

        protected override IDataParameter GetParameter()
        {
            return new MySqlParameter(); 
        }

        protected override void ClearCommand(IDbCommand command)
        {
            var canClear = true;
            foreach(IDataParameter commandParameter in command.Parameters)
            {
                if (commandParameter.Direction != ParameterDirection.Input) canClear = false;
            }
            
            if (canClear)
            {
                command.Parameters.Clear();
            }

            command.Parameters.Clear();
        }
    }
}