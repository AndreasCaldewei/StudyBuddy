﻿using System.Collections.Generic;
using StudyBuddy.Model;
using StudyBuddy.App.ViewModels;
using TinyIoC;

namespace StudyBuddy.App.Views
{
    public partial class FriendPage
    {
        private readonly FriendViewModel view_model;

        public FriendPage(UserViewModel obj, UserStatistics userStatistics)
        {
            InitializeComponent();

            var parms = NamedParameterOverloads.FromIDictionary(new Dictionary<string, object>() { { "obj", obj }, { "userStatistics", userStatistics } });
            view_model = TinyIoCContainer.Current.Resolve<FriendViewModel>(parms);
            BindingContext = view_model;
        }
    }
}