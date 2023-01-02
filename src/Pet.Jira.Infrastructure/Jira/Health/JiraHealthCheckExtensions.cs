﻿using Microsoft.Extensions.DependencyInjection;

namespace Pet.Jira.Infrastructure.Jira.Health
{
    public static class JiraHealthCheckExtensions
    {
        public static IHealthChecksBuilder AddJiraHealthCheck(this IHealthChecksBuilder builder)
        {
            builder.AddCheck<JiraHealthCheck>(JiraHealthCheck.Name, tags: new[] { "jira", "network" });
            return builder;
        }
    }
}
