using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Porygon.Identity.Entity;
using Porygon.Identity.Stores;

namespace Porygon.Identity
{
    public static class Extensions
    {
        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddTransient<IUserStore<User>, UserStore>();
            services.AddTransient<IRoleStore<IdentityRole>, RoleStore>();
            
            services.AddRazorPages();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }
    }
}
