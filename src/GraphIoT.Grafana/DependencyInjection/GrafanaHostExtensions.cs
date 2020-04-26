﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhilipDaubmeier.GraphIoT.Grafana.Controllers;
using PhilipDaubmeier.GraphIoT.Grafana.Services;
using ProxyKit;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.GraphIoT.Grafana.DependencyInjection
{
    public static class GrafanaHostExtensions
    {
        public static IServiceCollection AddGrafanaHost(this IServiceCollection serviceCollection)
        {
            // add grafana json api
            serviceCollection.AddControllers().AddApplicationPart(typeof(GrafanaController).Assembly);

            // add grafana hosted service for starting/stopping/restarting the grafana server process
            serviceCollection.AddHostedService<GrafanaBackendProcessService>();

            // add reverse proxy middleware for relaying to the grafana web server
            serviceCollection.AddProxy();

            return serviceCollection;
        }

        public static IApplicationBuilder ConfigureGrafanaHost(this IApplicationBuilder app, string appBasePath)
        {
            // configure reverse proxy for 'grafana' path
            var grafanaRegex = string.IsNullOrWhiteSpace(appBasePath) ? @"^/grafana($|/.*)" : @"^(" + appBasePath + ")?/grafana($|/.*)";
            string rewriteFunc(string s) => new PathString(((IEnumerable<Group>?)Regex.Matches(s, grafanaRegex).FirstOrDefault()?.Groups)?.Skip(2)?.FirstOrDefault()?.Value ?? string.Empty);
            app.UseWhen(context => Regex.IsMatch(context.Request.Path, grafanaRegex), appInner => appInner
                .UseRewriter(new RewriteOptions().Add(context => context.HttpContext.Request.Path = rewriteFunc(context.HttpContext.Request.Path)))
                .RunProxy(context => context.ForwardTo("http://localhost:8088").Send()));

            return app;
        }
    }
}