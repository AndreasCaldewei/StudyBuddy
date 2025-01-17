﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudyBuddy.App.Api;
using StudyBuddy.App.ViewModels;
using StudyBuddy.Model;

namespace StudyBuddy.App.Test.Mocks
{
    public class ChallengeServiceMock : IChallengeService
    {
        private List<ChallengeViewModel> challenges = new List<ChallengeViewModel>();

        public ChallengeServiceMock()
        {
            for (int i = 0; i < 100; i++)
            {
                challenges.Add(new ChallengeViewModel()
                {
                    ID = i + 1,
                    Category = RandomEnumValue<ChallengeCategory>(),
                    Name = "Test-Challenge " + (i + 1).ToString(),
                    Points = _R.Next(1, 100),
                    OwnerID = _R.Next(1, 10),
                    ValidityStart = DateTime.Now.Date,
                    ValidityEnd = DateTime.Now.Date.AddDays(1),
                    Prove = RandomEnumValue<ChallengeProve>()
                });
            }
        }

        static Random _R = new Random();
        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public Task<bool> Accept(ChallengeViewModel cvm)
        {
            return Task.Run(() =>
            {
                return true;
            });
        }

        public Task<ChallengeViewModel> AcceptFromQrCode(string code)
        {
            return Task.Run(() =>
            {
                return challenges[2];
            });
        }

        public Task<bool> AcceptWithAddendum(ChallengeViewModel cvm, string prove_addendum)
        {
            return Task.Run(() =>
            {
                return true;
            });
        }

        public async Task AddChallenges(IEnumerable<RequestViewModel> requests)
        {
        }

        public Task<IEnumerable<ChallengeViewModel>> GetAcceptedChallenges()
        {
            throw new NotImplementedException();
        }

        public Task<ChallengeViewModel> GetById(int challenge_id)
        {
            return Task.Run(() =>
            {
                return challenges[0];
            });
        }

        public async Task<ChallengeListViewModel> ForToday(string search_string, int skip)
        {
            return await Task.Run(() =>
            {
                return new ChallengeListViewModel()
                {
                    Objects = this.challenges.Skip(skip).Take(10),
                    Count = this.challenges.Count
                };
            });
        }

        public Task<ChallengeListViewModel> Accepted(string search_string = "", int skip = 0)
        {
            throw new NotImplementedException();
        }

        public Task<AcceptChallengeByLocationResultDTO> AcceptWithLocation(ChallengeViewModel cvm, GeoCoordinate location)
        {
            throw new NotImplementedException();
        }
    }
}
