﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using StudyBuddy.App.Api;
using StudyBuddy.App.Misc;
using StudyBuddy.App.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace StudyBuddy.App.ViewModels
{
    public class ChallengesViewModel : ViewModelBase
    {
        public string Header => string.Format("Herausforderungen am {0}", DateTime.Now.ToShortDateString());
        public RangeObservableCollection<ChallengeViewModel> Challenges { get; private set; }
        public ChallengeViewModel SelectedChallenge { get; set; }
        public IAsyncCommand RefreshCommand { get; }
        public IAsyncCommand DetailsCommand { get; }
        public IAsyncCommand ScanQrCodeCommand { get; }
        public IAsyncCommand LoadMoreCommand { get; }
        public IAsyncCommand SearchCommand { get; }
        public bool IsRefreshing { get; set; } = false;
        public int Skip { get; set; } = 0;
        public bool IsBusy { get; set; } = false;

        public ChallengesViewModel(IApi api) : base(api)
        {
            Challenges = new RangeObservableCollection<ChallengeViewModel>();
            LoadMoreCommand = new AsyncCommand(LoadChallenges);
            SearchCommand = new AsyncCommand(Refresh);
            DetailsCommand = new AsyncCommand(ShowDetails);
            ScanQrCodeCommand = new AsyncCommand(ScanQrCode);
            RefreshCommand = new AsyncCommand(Refresh);

            this.search_text = api.Device.GetPreference("SearchText", String.Empty);
            api.ChallengeAccepted += async (sender, e) => { await LoadChallenges(); };
        }

        private string search_text = string.Empty;
        public string SearchText
        {
            get { return search_text; }
            set
            {
                if (search_text == value)
                    return;

                search_text = value ?? string.Empty;
                api.Device.SetPreference("SearchText", search_text);

                Task.Run(async () =>
                {
                    string SearchText = search_text;
                    await Task.Delay(500);

                    if (search_text == SearchText)
                        await Refresh();
                });
            }
        }

        private int item_treshold = 1;
        public int ItemThreshold
        {
            get { return item_treshold; }
            set
            {
                item_treshold = value;
                NotifyPropertyChanged();
            }
        }

        private async Task Refresh()
        {
            Challenges.Clear();
            Skip = 0;
            await LoadChallenges();
            IsRefreshing = false;
            NotifyPropertyChanged(nameof(IsRefreshing));
        }

        private async Task LoadChallenges()
        {
                if (IsBusy)
                return;
            else
                IsBusy = true;

            try
            {
                ItemThreshold = 1;
                var challenges = await api.Challenges.ForToday(SearchText, Skip);
                if (challenges.Objects.Count() == 0)
                {
                    ItemThreshold = -1;
                    return;
                }

                Challenges.AddRange(challenges.Objects);
                Skip += 10;
            }
            catch (ApiException e)
            {
                api.Device.ShowError(e, "Ein Fehler ist aufgetreten!", "Ok", null);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ShowDetails()
        {
            if (SelectedChallenge == null)
                return;
            
            await api.Device.PushPage(new ChallengeDetailsPage(SelectedChallenge));
            SelectedChallenge = null;
            NotifyPropertyChanged(nameof(SelectedChallenge));
        }

        private async Task ScanQrCode()
        {
            var route = $"{nameof(QrCodePage)}";
            await Shell.Current.GoToAsync(route);
        }
    }
}