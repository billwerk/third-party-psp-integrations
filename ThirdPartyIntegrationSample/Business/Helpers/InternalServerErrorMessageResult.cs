using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Business.Helpers
{
    // <summary>
    /// An action result that returns a <see cref="F:Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError" /> response and performs
    /// content negotiation on an <see cref="T:System.Web.Http.HttpError" /> with a <see cref="P:System.Web.Http.HttpError.Message" />.
    /// </summary>
    public class InternalServerErrorMessageResult : ObjectResult
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Http.BadRequestErrorMessageResult" /> class.</summary>
        /// <param name="message">The user-visible error message.</param>
        public InternalServerErrorMessageResult(string message) : base(null)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
            StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
        }

        /// <summary>Gets the error message.</summary>
        public string Message { get; }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            var statusCode = StatusCode;
            if (statusCode != null) context.HttpContext.Response.StatusCode = statusCode.Value;
            return base.ExecuteResultAsync(context);
        }
    }
}