﻿using System.IO;
using StudyBuddy.Model;

namespace StudyBuddy.BusinessLogic
{
    public interface IChallengeService
    {
        // Basic CRUD-Services
        Challenge GetById(int id);
        ChallengeList All(ChallengeFilter filter);
        Challenge Insert(Challenge obj);
        Challenge Update(Challenge obj);
        void Delete(int id);

        // QR-Code
        MemoryStream GetQrCode(int challenge_id);
        Challenge AcceptFromQrCode(string payload);

        // Accept
        void RemoveAcceptance(int challenge_id, int user_id);
        void AddAcceptance(int challenge_id, int user_id);
        void Accept(int challenge_id);
        bool AcceptWithAddendum(int challenge_id, string prove_addendum);
        AcceptChallengeByLocationResultDTO AcceptWithLocation(AcceptChallengeByLocationRequestDTO obj);

        // Misc
        void CreateSeries(CreateSeriesParameter param);
        ChallengeList GetChallengesOfBadge(int badge_id);
    }
}