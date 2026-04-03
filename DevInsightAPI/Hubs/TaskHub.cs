using System.Security.Claims;
using DevInsightAPI.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DevInsightAPI.Hubs
{
    [Authorize]
    public class TaskHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.IsInRole(UserRoles.Admin) == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, HubEventNames.AdminGroup);
            }

            var userIdValue = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdValue, out var userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, HubEventNames.UserGroup(userId));
            }

            await base.OnConnectedAsync();
        }
    }
}
