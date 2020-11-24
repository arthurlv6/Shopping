using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSP.Shared
{
    public static class Policies
    {
        public const string CanManageContent = "CanManageContent";

        public static AuthorizationPolicy CanManageContentPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("roleclaim", "admin")
                .Build();
        }
    }
}
