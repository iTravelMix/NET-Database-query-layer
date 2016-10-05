
namespace ADO.Query.Helper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Extensions;
    using Mapper;
    using SqlQuery;

    public abstract class QueryRunner : IQueryRunner
    {
        #region Declare members

        private readonly IQueryMappers mapper;

        protected static Hashtable ParamCache = Hashtable.Synchronized(new Hashtable());
        protected static string ConnectionString;

        #endregion

        protected QueryRunner(IQueryMappers mapper)
        {
            this.mapper = mapper;
        }

        #region Provider specific abstract methods
        
        public abstract IDbConnection GetConnection();

        protected abstract IDataParameter GetParameter();
		
        #endregion

        #region - Factory -

        public static IQueryRunner CreateHelper(DataAccessSectionHandler settings)
        {
            return CreateHelper(settings, null);
        }

        public static IQueryRunner CreateHelper(DataAccessSectionHandler settings, IQueryMappers mapper )
        {
            try
            {
                var providerType = settings.Type;
                ConnectionString = settings.ConnectionString;

                var daType = Type.GetType(providerType);
                if (daType == null) throw new NullReferenceException("Null Reference in Provider type configuration Session.");

                var provider =  Activator.CreateInstance(daType, mapper);
                if (provider is QueryRunner)
                {
                    return provider as IQueryRunner;
                }

                throw new Exception("The provider specified does not extends the QueryRunner abstract class.");
            }
            catch (Exception e)
            {
                throw new Exception("If the section is not defined on the configuration file this method can't be used to create an QueryRunner instance.", e);
            }
        }

        #endregion

        #region - protected utility methods -

        protected virtual void AttachParameters(IDbCommand command, IDataParameter[] commandParameters)
        {
            if( command == null ) throw new ArgumentNullException("command");
            if (commandParameters == null) return;
            
            foreach (var p in commandParameters.Where(p => p != null))
            {
                // Check for derived output value with no value assigned
                if ( ( p.Direction == ParameterDirection.InputOutput || 
                       p.Direction == ParameterDirection.Input ) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        protected virtual void PrepareCommand(IDbCommand command, IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDataParameter[] commandParameters, out bool mustCloseConnection )
        {
            if( command == null ) throw new ArgumentNullException("command");
            if( string.IsNullOrEmpty(commandText) ) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if( transaction.Connection == null ) throw new ArgumentException( "The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                this.AttachParameters(command, commandParameters);
            }
        }

        protected virtual void ClearCommand(IDbCommand command )
        {
        }

        #endregion private utility methods

        #region - ExecuteQueryMapper -

        public virtual QueryMapperResult<TResult> Execute<TResult>(SqlQuery criterial) where TResult : class
        {
            using (var dr = this.ExecuteReader(CommandType.Text, criterial.Expression, this.GetCriterialParameters(criterial.Parameters)))
            {
                return new QueryMapperResult<TResult>(this.mapper, dr.ToDynamic());
            }
        }

        public virtual PageSqlResult<TResult> Execute<TResult>(SqlPagedQuery criterial) where TResult : class
        {
            var dataParameters = this.GetCriterialParameters(criterial.Parameters);

            var total = this.ExecuteScalar<long>(CommandType.Text, criterial.SqlCount, dataParameters);
            var totalPages = total % criterial.ItemsPerPage == 0 ? total / criterial.ItemsPerPage : (total / criterial.ItemsPerPage) + 1;

            IEnumerable<TResult> result;
            using (var dr = this.ExecuteReader(CommandType.Text, criterial.Expression, dataParameters))
            {
                result = this.mapper.MapDynamicToList<TResult>((List<object>)dr.ToDynamic());
            }

            return new PageSqlResult<TResult>
            {
                CurrentPage = criterial.Page,
                TotalItems = total,
                TotalPages = totalPages,
                Result = result
            };
        }

        #endregion

        #region - ExecuteReader -

        public virtual IDataReader ExecuteReader(SqlQuery criterial)
        {
            // Pass through the call providing null for the set of IDataParameters
            return this.ExecuteReader(CommandType.Text, criterial.Expression, this.GetCriterialParameters(criterial.Parameters));
        }

        private IDataReader ExecuteReader(CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            return this.ExecuteReader(this.GetConnection(), null, commandType, commandText, commandParameters);
        }

        private IDataReader ExecuteReader(IDbConnection connection, IDbTransaction transaction, CommandType commandType, string commandText, IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            bool mustCloseConnection;

            // Create a command and prepare it for execution
            var cmd = connection.CreateCommand();
            this.PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create a reader

            // Call ExecuteReader with the appropriate CommandBehavior
            var dataReader = mustCloseConnection ? cmd.ExecuteReader(CommandBehavior.CloseConnection) : cmd.ExecuteReader();

            this.ClearCommand(cmd);
            return dataReader;
        }

        #endregion ExecuteReader

        #region - ExecuteScalar -

        public virtual T ExecuteScalar<T>(SqlQuery criterial)
        {
            // Pass through the call providing null for the set of IDataParameters
            return this.ExecuteScalar<T>(CommandType.Text, criterial.Expression, this.GetCriterialParameters(criterial.Parameters));
        }

        private T ExecuteScalar<T>(CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            using (var connection = this.GetConnection())
            {
                return this.ExecuteScalar<T>(connection, commandType, commandText, commandParameters);
            }
        }

        private T ExecuteScalar<T>(IDbConnection connection, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            var mustCloseConnection = false;

            try
            {
                // Create a command and prepare it for execution
                var cmd = connection.CreateCommand();

                this.PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);

                // Execute the command & return the results
                var retval = cmd.ExecuteScalar();

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();

                return (T)retval;
            }
            finally 
            {
                if (mustCloseConnection && connection.State != ConnectionState.Closed) connection.Close();
            }
        }

        #endregion	

        #region - Parameter Discovery Functions -

        public virtual IDataParameter GetParameter(string name, object value)
        {
            var parameter = this.GetParameter();
            parameter.ParameterName = name;
            parameter.Value = value;

            return parameter;
        }

        private IDataParameter[] GetCriterialParameters(IEnumerable<KeyValuePair<string, object>> parameters)
        {
            return parameters != null ? parameters.Select(p => this.GetParameter(p.Key, p.Value)).ToArray() : null;
        }
        
        #endregion Parameter Discovery Functions
    }
}