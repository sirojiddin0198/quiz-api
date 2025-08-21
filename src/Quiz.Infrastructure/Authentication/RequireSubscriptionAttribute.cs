namespace Quiz.Infrastructure.Authentication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Quiz.Shared.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequireSubscriptionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string _requiredFeature;

    public RequireSubscriptionAttribute(string requiredFeature)
    {
        _requiredFeature = requiredFeature;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
            return;

        var subscriptionService = context.HttpContext.RequestServices.GetRequiredService<ISubscriptionService>();
        
        var hasAccess = await subscriptionService.HasAccessAsync(_requiredFeature, context.HttpContext.RequestAborted);
        
        if (hasAccess is false)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
} 