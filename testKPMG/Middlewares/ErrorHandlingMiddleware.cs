using System.Text.Json;
using testKPMG.AuxTools;
using testKPMG.AuxTools.models;

namespace testKPMG.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        //private readonly HttpContext _context;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(/*HttpContext context,*/ RequestDelegate next)
        {
            //_context = context;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleError(context, ex.Message, StatusCodes.Status404NotFound);
            }
            catch (ForbiddenException ex)
            {
                await HandleError(context, ex.Message, StatusCodes.Status403Forbidden);
            }
            catch (UnAuthorizedException ex)
            {
                await HandleError(context, ex.Message, StatusCodes.Status401Unauthorized);
            }
            catch (BadRequestException ex)
            {
                await HandleError(context, ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (ConflictException ex)
            {
                await HandleError(context, ex.Message, StatusCodes.Status409Conflict);
            }
            catch (InternalServerException ex)
            {
                await HandleError(context, ex.Message, StatusCodes.Status500InternalServerError);
            }
        }

        private Task HandleError(HttpContext context, string message, int statusCode)
        {
            var response = new ApiResponse<string>
            {
                Success = false,
                Message = message,
                Data = null
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
