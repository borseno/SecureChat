using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecureChat.Shared;

namespace SecureChat.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello");
                });
                endpoints.MapHub<SecureChatHub>("/chathub");
            });
        }
        public class SecureChatHub : Hub
        {
            static Dictionary<string, User> Pairs_ConnectionId_User = new Dictionary<string, User>();
            public override Task OnConnectedAsync()
            {
                var connectionID = Context.ConnectionId;
                User user = UserInitializer.GetNewUser(null);  

              
                Pairs_ConnectionId_User.Add(connectionID, user);

                return base.OnConnectedAsync();
            }
           

            public async Task SendMessage(string message, string Name, DateTime dateTime)
            {
                var connectionID = Context.ConnectionId;
                User user;

                if (!Pairs_ConnectionId_User.TryGetValue(connectionID, out user))
                {
                    return; 
                }

                if (Name != user.Name)
                {
                    user.Name = Name;
                    await OnUserNameChanged(user);
                }
                    
                await Clients.Others.SendAsync("ReceiveMessage", message, user, dateTime);
                ;
            }

            public async Task OnUserNameChanged(User user)
            {
                await Clients.Others.SendAsync("OnUserNameChanged", user);
                ;
            }
        }

        

        /// TODO:+ вынести логику в статический класс на стороне сервера
        ///      + создать библиотеку с юзером
        ///      + инициализатор юзера
        ///      + метод изменение имени. оповещение остальных
        ///       уведомление о кол-ве оставшихся символов + блокировка
        }
}
