using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Orders.API.Filter;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var problemDetails = new ProblemDetails();

        switch (context.Exception)
        {
            case InvalidOperationException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Operação inválida";
                problemDetails.Detail = context.Exception.Message;
                break;

            case UnauthorizedAccessException:
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Não autenticado";
                problemDetails.Detail = "Falha de auteticação";
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Erro interno";
                problemDetails.Detail = "Erro desconhecido";
                break;
        }

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };

        context.ExceptionHandled = true;
    }
}
