namespace NEE.Core.Contracts.Enumerations
{
    public enum ServiceAction
    {
        CreateApplication = 1,
        GetApplication = 4,
        CancelApplication = 5,
        SaveApplication = 6,
        GetApplicationOwner = 7,
        FinalSubmitApplication = 8,
        SearchApplication = 9,
        TryAddMember = 11,
        AddMember = 12,
        SaveMember = 13,
        EditMember = 14,
        ConcentMember = 15,
        RestoreMember = 16,
        RemoveMember = 17,
        GetMember = 18,

        ManageApplications = 21,
        ValidateTenancyNumber = 22,
        ValidatePowerMeterNumber = 23,
        ValidateApplication = 24,
        SetPersonConsent = 25,
        SetMemberState = 26,
        TryAddMemberAdmin = 28,

        SuspendApplication = 29,
        RecallApplication = 30,
        UndoSuspendApplication = 31,
        ParticipatesInSpecialProgram = 32,
        ParticipatesInBlackList = 33,
        PrintApplication = 34,
        SubmittedDetails = 35,
        ValidateInsuranceCriteria = 36,
        UndoRecallApplication = 37,
        ManageMemberPenalty = 38,
        ApplicantExceptionDeclared = 39,
        GetPaymentTransactions = 40,
        AddPaymentTransaction = 41,

        GetPaymentsData = 42,
        GetPaymentTransactionsMaster = 43,

        ValidateIBAN = 47,

        GetCustomContracts = 50,
        GetCustomContract = 51,

        UploadFiles = 52,
        RejectNonEligibleApplication = 53,
        ChangeApplicationDistrict = 54,
    }

}
