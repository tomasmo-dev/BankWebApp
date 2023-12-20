using BankWebApp.env;
using Microsoft.Data.SqlClient;

namespace BankWebApp.Services;

public partial class DatabaseService : IDisposable
{
    private readonly string _connectionString;

    private SqlConnection _connection;
    
    public DatabaseService()
    {
        _connectionString = Envs.ConnectionString;
        _connection = new SqlConnection(_connectionString);
        
        Open();
    }

    private void Open()
    {
        _connection.Open();
    }
    
    private void Close()
    {
        _connection.Close();
    }

    public void Dispose()
    {
        Close();
    }
}

/*
 * Microsoft.Data.SqlClient.SqlException (0x80131904): New transaction is not allowed because there are other threads running in the session.
   at Microsoft.Data.SqlClient.SqlConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
   at Microsoft.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
   at Microsoft.Data.SqlClient.TdsParser.TryRun(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj, Boolean& dataReady)
   at Microsoft.Data.SqlClient.TdsParser.Run(RunBehavior runBehavior, SqlCommand cmdHandler, SqlDataReader dataStream, BulkCopySimpleResultSet bulkCopyHandler, TdsParserStateObject stateObj)
   at Microsoft.Data.SqlClient.TdsParser.TdsExecuteTransactionManagerRequest(Byte[] buffer, TransactionManagerRequestType request, String transactionName, TransactionManagerIsolationLevel isoLevel, Int32 timeout, SqlInternalTransaction transaction, TdsParserStateObject stateObj, Boolean isDelegateControlRequest)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.ExecuteTransaction2005(TransactionRequest transactionRequest, String transactionName, IsolationLevel iso, SqlInternalTransaction internalTransaction, Boolean isDelegateControlRequest)
   at Microsoft.Data.SqlClient.SqlInternalConnectionTds.ExecuteTransaction(TransactionRequest transactionRequest, String name, IsolationLevel iso, SqlInternalTransaction internalTransaction, Boolean isDelegateControlRequest)
   at Microsoft.Data.SqlClient.SqlInternalConnection.BeginSqlTransaction(IsolationLevel iso, String transactionName, Boolean shouldReconnect)
   at Microsoft.Data.SqlClient.SqlConnection.BeginTransaction(IsolationLevel iso, String transactionName)
   at Microsoft.Data.SqlClient.SqlConnection.BeginTransaction()
   at BankWebApp.Services.DatabaseService.TransferFunds(Guid from, Guid To, Decimal Amount) in C:\Users\tomas\source\repos\BankWebApp\Services\DatabaseServiceFunctions.cs:line 170
   at BankWebApp.Services.TransferService.TransferMoney(String FromAcc, String ToAcc, Decimal Amount) in C:\Users\tomas\source\repos\BankWebApp\Services\TransferService.cs:line 34
   at BankWebApp.Controllers.AccountController.Transfer(TransferViewModel model) in C:\Users\tomas\source\repos\BankWebApp\Controllers\AccountController.cs:line 116
   at lambda_method67(Closure, Object, Object[])
   at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.SyncActionResultExecutor.Execute(ActionContext actionContext, IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeActionMethodAsync()
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeNextActionFilterAsync()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.InvokeInnerFilterAsync()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResourceFilter>g__Awaited|25_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResourceExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeFilterPipelineAsync()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddlewareImpl.Invoke(HttpContext context)
ClientConnectionId:a234c0b2-f586-48b2-ad02-819629fed641
Error Number:3988,State:1,Class:16
 */