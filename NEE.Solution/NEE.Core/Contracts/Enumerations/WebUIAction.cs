namespace NEE.Core.Contracts.Enumerations
{
    public enum WebUIAction
{
    ManageApplications = 1,
    ManageApplicationsPost = 2,
    ConsentToApplicationHandle = 3,
    CreateNewApplication = 4,
    CancelApplication = 8,
    ValidateSaveFinalSubmitApplication = 9,
    ValidatePartialApplication = 10,
    ValidateSaveApplication = 11,
    SaveApplication = 12,
    EditApplication = 13,
    ViewApplication = 14,

    SuspendApplication = 16,
    RecallApplication = 17,
    UndoSuspendApplication = 18,
    UndoRecallApplication = 19,

    AddMemberConfirmedPost = 20,
    AddMemberPost = 21,
    EditMemberPost = 22,
    EditMember = 23,
    ViewMember = 24,
    RemoveMember = 25,
    DeleteMember = 26,

    FillDictionaries = 40,
    PrintApplication = 41,
    AddMember = 42,
    FixMemberAFM = 43,
    SearchMemberPenalties = 44,
    ManageMemberPenalty = 45,

    FileUpload = 46,

    SaveDocumentHandle = 47,
    ChangeDistrict = 48,
}
}
