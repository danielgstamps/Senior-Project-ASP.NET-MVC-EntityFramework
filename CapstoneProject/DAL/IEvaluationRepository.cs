using CapstoneProject.Models;
using System;
using System.Collections.Generic;

namespace CapstoneProject.DAL
{
    public interface IEvaluationRepository : IDisposable
    {
        IEnumerable<Evaluation> GetEvaluations();
        Evaluation GetEvaluationByID(int? evaluationID);
        void InsertEvaluation(Evaluation evaluation);
        void DeleteEvaluation(int evaluationID);
        void UpdateEvaluation(Evaluation evaluation);
        void Save();
    }
}