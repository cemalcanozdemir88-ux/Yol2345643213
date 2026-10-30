using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Transactions.Helpers
{
    public class FixDuplicateTransactionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var originalUrl = request.GetEncodedUrl(); // Get the full original URL

            var query = request.Query;
            if (query.ContainsKey("transaction"))
            {
                // Check if the URL contains multiple '?' signs
                var firstQuestionMarkIndex = originalUrl.IndexOf('?');
                var secondQuestionMarkIndex = originalUrl.IndexOf('?', firstQuestionMarkIndex + 1);

                if (secondQuestionMarkIndex > -1)
                {
                    // Clean the URL by removing the part after the first transaction
                    var fixedUrl = originalUrl.Substring(0, secondQuestionMarkIndex);

                    // Redirect to the cleaned URL
                    context.Result = new RedirectResult(fixedUrl);
                }
            }

            base.OnActionExecuting(context);
        }
    }

}
