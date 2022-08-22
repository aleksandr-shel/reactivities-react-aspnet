using API.Extensions;
using API.Middleware;
using API.SignalR;
using Application.Activities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opt =>
            {
                //authorazation policy so that entire app requires authentication to access controllers
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssemblyContaining<Create>();
            });
            services.AddApplicationServices(_config);
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //our own exception middleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer());
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
            app.UseXfo(opt=>opt.Deny());
            app.UseCsp(opt=>opt
                .BlockAllMixedContent()
                .StyleSources(s => s.Self().CustomSources("sha256-yR2gSI6BIICdRRE2IbNP1SJXeA5NYPbaM32i/Y8eS9o=", "https://fonts.googleapis.com", "https://fonts.googleapis.com", "https://cdnjs.cloudflare.com"))
                .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:", "https://cdnjs.cloudflare.com"))
                .FormActions(s =>s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com", "https://www.facebook.com", "data:", "https://platform-lookaside.fbsbx.com/"))
                .ScriptSources(s => s.Self().CustomSources("sha256-KTnfqKV5ClKVL0SsIdfIgdc1YtjPmzbEp2k347nq+HA=", "sha256-kPx0AsF0oz2kKiZ875xSvv693TBHkQ/0SkMJZnnNpnQ=", 
                    "http://conoret.com", "https://connect.facebook.net/", "sha256-kPx0AsF0oz2kKiZ875xSvv693TBHkQ/0SkMJZnnNpnQ="))
                );

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
            } else
            {
                //app.UseHsts();
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Add("Strict-Transport-Security", "max-age=3156000");
                    await next.Invoke();
                });
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            //looking for index.html in wwwroot folder 
            app.UseDefaultFiles();
            //serving files from wwwroot folder
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoint for signalr
                endpoints.MapHub<ChatHub>("/chat");
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
