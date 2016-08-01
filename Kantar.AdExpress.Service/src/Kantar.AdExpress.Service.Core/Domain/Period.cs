﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.BusinessService
{
    public class PeriodResponse
    {
        public PeriodResponse()
        {
            ControllerDetails = new ControllerDetails();
            ErrorMessage = string.Empty;
        }

        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public int SiteLanguage { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
        public ControllerDetails ControllerDetails { get; set; }

    }

    public class PeriodSaveRequest
    {
        #region Constructors
        public PeriodSaveRequest()
        {

        }
        public PeriodSaveRequest(string idWebSesion, string startDate, string endDate, string nextStep, int studyId)
        {
            IdWebSession = idWebSesion;
            StartDate = startDate;
            EndDate = endDate;
            NextStep = nextStep??string.Empty;
            StudyId = studyId;
        }
        public PeriodSaveRequest(string idWebSesion, string startDate, string endDate, string nextStep)
        {
            IdWebSession = idWebSesion;
            StartDate = startDate;
            EndDate = endDate;
            NextStep = nextStep ?? string.Empty;
        }
        public PeriodSaveRequest(string idWebSesion, int selectedPeriod, int selectedValue, string nextStep, int studyId)
        {
            IdWebSession = idWebSesion;
            SelectedPeriod = selectedPeriod;
            SelectedValue = selectedValue;
            NextStep = nextStep ?? string.Empty;
            StudyId = studyId;
        }
        #endregion
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NextStep { get; set; }
        public int StudyId { get; set; }
        public string IdWebSession { get; set; }
        public int SelectedValue { get; set; }
        public int SelectedPeriod { get; set; }
    }
}