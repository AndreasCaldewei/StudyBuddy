﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StudyBuddy.App.ViewModels;
using StudyBuddy.Model;
using StudyBuddy.Model.Filter;

namespace StudyBuddy.App.Api
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationViewModel>> GetNotificationsForFriends(int skip);
    }
}