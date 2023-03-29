namespace NEE.Core.Contracts.Enumerations
{
    public enum AppRenewalStatus
    {
        Renewed = 0,
        NotRenewedForeignIdentificationNumberExpired = 1,
        NotRenewedDeadMemberFound = 2,
        NotRenewedMemberCameToAge = 3,
        NotRenewedNewNotApproved = 5,
        NotRenewedThreePaymentsErrors = 6,
        NotRenewedNotProcessed = 9,
    }
}
