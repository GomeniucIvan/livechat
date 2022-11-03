using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Smartstore.Extensions;
using Smartstore.Utilities;

namespace Smartstore.Core.Data.Bootstrapping
{
    public static class DBStoreProcedure
    {
        public static IEnumerable<T> ExecStoreProcedure<T>(this SmartDbContext db, string sql, params object[] parameters)
        {
            var testExec = $"EXEC {sql}";
            sql = $"EXEC {sql}";

            if (parameters != null && parameters.Any())
            {
                //add parameters to sql
                for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
                {
                    if (!(parameters[i] is DbParameter parameter))
                        continue;

                    sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";


                    //whether parameter is output
                    if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                        sql = $"{sql} output";
                }
            }

            #region Test region

            try
            {
                var parameterList = new List<string>();
                if (parameters != null && parameters.Any())
                {
                    //add parameters to sql
                    for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
                    {
                        if (!(parameters[i] is DbParameter parameter))
                            continue;

                        //whether parameter is output
                        if (parameter.Direction == ParameterDirection.InputOutput ||
                            parameter.Direction == ParameterDirection.Output)
                        {
                            //nothing
                        }
                        else
                        {
                            parameterList.Add(parameter.Value != null && !string.IsNullOrEmpty(parameter.Value.ToString()) ? (parameter.DbType == DbType.String ? $"'{parameter.Value.ToString()}'" : parameter.Value.ToString()) : "NULL");
                            testExec = testExec + (i > 0 ? ", " : " ") + $"@{parameter.ParameterName}=" + "{" + i + "}";
                        }
                    }
                }

                testExec = string.Format(testExec, parameterList.ToArray());
            }
            catch (Exception e)
            {
                testExec = e.Message;
            }

            #endregion

            testExec = testExec;

            if (CommonHelper.IsDevEnvironment)
            {
                Debug.WriteLine(testExec);
            }

            return db.Database.ExecuteQueryRaw<T>(sql, parameters);
        }

        public static int TotalCount(this DbParameter dbParameter)
        {
            return dbParameter.Value != DBNull.Value ? Convert.ToInt32(dbParameter.Value) : 0;
        }

        public static List<int> ToListOfInt(this DbParameter dbParameter)
        {
            if (dbParameter == null || dbParameter.Value == DBNull.Value || dbParameter.Value == null)
            {
                return new List<int>();
            }

            var dbParameterString = dbParameter.Value.ToString();
            return dbParameterString.ToListOfInt().ToList();
        }
    }
}
