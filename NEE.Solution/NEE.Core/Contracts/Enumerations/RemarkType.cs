namespace NEE.Core.Contracts.Enumerations
{
	public enum RemarkType
	{
		System_Error = -1001,
		System_Info = -1000,

		Audit_Suspend = 8001,
		Audit_Resume = 8002,

        DeclarationLaw1599 = 1599,
		CommunicationInfo = 100,
		AFM = 101,
		AMKA = 102,
		FirstName = 103,
		LastName = 104,
		DOB = 105,
		Gender = 106,
		
		BenefitsIncome = 201,
		SalaryIncome = 208,
		OtherIncome = 210,
		SalaryIncomeInconsistency = 211,

		Deposits = 301,
		InterestsValue = 302,
		RealEstateValue = 303,
		VehiclesValue = 304,

		HasLuxuryItem = 801,

		IncomeSumExcess = 1000,
		RealEstateValueExcess = 1001,
		Eligibility = 1002,
		ReferenceIncomeHigher = 1003,
		VehiclesValueExcess = 1004,
		AssetsValueExcess = 1005,
		ReferenceAssetsValueHigher = 1006,
		ReferenceVehiclesValueHigher = 1007,


		MobilePhone = 2009,

		ApplicantWithoutTaxis = 3003,
		MemberWithoutTaxis = 3004,

		UnknownAMKA = 3011,
		UnknownAFM = 3012,
		MultipleAFM = 3013,

		// Other
		IbanNotFound = 8001,

        #region personal
        AgeBelow67 = 7001,
        EdtoNotExpatriate = 7002,
        ErganhRecordFound = 7003,        
        #endregion

        #region financial
        AADEIncomeExceeded = 8013,
        AADEFamilyIncomeExceeded = 8014,
        AADERealEstateExceeded = 8015,
		AADEVehicleExceeded = 8016,

		HousingBenefitExceeded = 8017,
        HousingAssistanceBenefitExceeded = 8018,
        BenefitForOmogeneisExceeded = 8019,
        DisabilityBenefitsExceeded = 8020,
		A21BenefitExceeded = 8021,
        PensionExceeded = 8022,
        PensionFromAlbaniaExceeded = 8023,
        PensionSumExceeded = 8024,

        AADESpouseIncomeExceeded = 9013,
        AADESpouseRealEstateExceeded = 9014,
        AADESpouseVehicleExceeded = 9015,
        SpousePensionExceeded = 9016,
        SpousePensionFromAlbaniaExceeded = 9017,
        SpousePensionSumExceeded = 9018,
        #endregion

        IbanNotValid = 8029,
		
		MaritalStatusNotFound = 8039,
		
		IbanNotOwned = 10009,
		UnsupportedBank = 10010,

		FekDocumentRejected = 110001,
        PensionAlbaniaDocumentRejected = 110002,
        MaritalStatusDocumentRejected = 110003,
		SpousePensionAlbaniaDocumentRejected = 110004,

        #region files
        FEKUploaded = 1101,
        PensionAlbaniaUploaded = 1102,
        MaritalStatusUploaded = 1103,
        SpousePensionAlbaniaUploaded = 1104,

        #endregion
    }

}
