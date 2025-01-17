using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using StudyBuddy.App.Misc;
using StudyBuddy.App.ViewModels;
using StudyBuddy.Model;
using StudyBuddy.Model.Filter;
using TinyIoC;

namespace StudyBuddy.App.Api
{
    public class NotificationService : INotificationService
    {
        private readonly IApi api;
        private readonly string base_url;
        private readonly HttpClient client;

        public NotificationService(IApi api, string base_url)
        {
            this.api = api;
            this.base_url = base_url;
            client = new HttpClient(Helper.GetInsecureHandler());
        }

        public async Task<IEnumerable<NotificationViewModel>> GetNotificationsForFriends(int skip)
        {
            var filter = new NotificationFilter()
            {
                Start = skip,
                Count = 10,
                UserID = api.Authentication.CurrentUser.ID,
                WithOwner = true,
                WithLikedUsers = true,
                WithBadge = true
            };

            var rh = new WebRequestHelper(api.Authentication.Token);
            var content = await rh.Get<IEnumerable<NotificationViewModel>>(base_url + "Notification/Feed", filter);
            if (content == null)
                return new List<NotificationViewModel>();

            return content;
        }

 
    }
}