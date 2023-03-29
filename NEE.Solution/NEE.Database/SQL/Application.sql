/*--drop table "NEE_AppPerson" purge;
--drop table "NEE_AppRemark" purge;
--drop table "NEE_App" purge;*/

CREATE SEQUENCE  "AppIdSequence"  MINVALUE 1 MAXVALUE 999999 INCREMENT BY 1 START WITH 1 CACHE 20 ORDER  CYCLE  NOPARTITION ;

Create Table "NEE_App"
(
	--- Control: Identity ---

	"Id"						    varchar2(29)	    Constraint "CN_NEE_App_Id"					Not Null Enable		
                                        				Constraint "CP_NEE_App_Id"					Primary Key Enable
,   "GmiApplicationId"              varchar2(19)
,   "GMIAppIdWithDifApplicant"		varchar2(19)
,   "Type"                          number(1,0)         Constraint "CC_NEE_App_Type"                 Check ("Type"	Between 0 and 1)
,   "Version"						number(9,0)  		Constraint "CN_NEE_Version"					Not Null Enable	    
,   "Revision"						number(9,0)  		Constraint "CN_NEE_Revision"					Not Null Enable	
,	"State"							number(9,0)  		Constraint "CN_NEE_State"					Not Null Enable
,	"ApplicationIndex"				number(9,0)  		Constraint "CN_NEE_App_Index"					Not Null Enable
,	"AMKA"							varchar2(11) 		Constraint "CN_NEE_AMKA"						Not Null Enable	
,	"AFM"							varchar2(9) 		Constraint "CN_NEE_AFM"						Not Null Enable	
,	"SpouseAMKA"					varchar2(11) 		
,	"SpouseAFM"						varchar2(9) 
,   "IBAN"                          varchar2(27)
,	"Email"							varchar2(256)		
,	"HomePhone"						varchar2(10)	
,	"MobilePhone"					varchar2(10)	
,   "Street"						varchar2(60)
,	"StreetNumber"					varchar2(10)
,   "Zip"							varchar2(5)
,   "City"							varchar2(30)
,   "District"						varchar2(30)
,   "Country"						varchar2(30)

,   "Amount"						number(18,2)
,	"PeriodFrom"					date
,	"PeriodTo"						date

,   "TenancyNumber"                 number
   --- Tenancy Agreement Data: Πληροφορίες για το Μισθωτήριο από ΑΑΔΕ---
,   "AADE_TenancyNumber"			number
,	"AADE_TenancyStartDate"				date
,	"AADE_TenancyEndDate"				date   
,   "AADE_TenancyAccepted"				number(1,0)			Constraint "CC_NEE_App_AADE_TenancyAccepted"               Check ("AADE_TenancyAccepted"	     Between 0 and 1)
,   "AADE_TenancyPowerMeterNumber"	    varchar2(11)
,   "AADE_RentAmount"					number(18,2)

,	"PowerMeterNumber"	            varchar2(11)	

    --- Power Data: DEDDIE Related ---
,	"DEDDIE_PowerMeterNumber"	    varchar2(11)		-- Αρ. Παροχής/Μετρητή ΔΕΔΔΗΕ: 9 ή 9+2 ψηφία (XXXXXXXXX ή XXXXXXXXXNN, πχ: 123456789 ή 12345678901)
,	"DEDDIE_AFM"				    varchar2(9)			-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
,	"DEDDIE_IsActive"				number(1,0)			Constraint "CC_NEE_App_IsActive"               Check ("DEDDIE_IsActive"	     Between 0 and 1)	-- bool: yes/no
,	"DEDDIE_IsHomeUsage"			number(1,0)			Constraint "CC_NEE_App_IsHomeUsage"            Check ("DEDDIE_IsHomeUsage"      Between 0 and 1)	-- bool: yes/no
,	"DEDDIE_Street"				    varchar2(60)		-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
,	"DEDDIE_StreetNumber"			varchar2(10)		-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
,	"DEDDIE_ZipCode"				varchar2(5)			-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
,	"DEDDIE_City"				    varchar2(60)		-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
,	"DEDDIE_CallikratianMunicipal"  varchar2(60)		-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
,	"DEDDIE_UsageTypeCode"			varchar2(10)		-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
,	"DEDDIE_UsageTypeDescription"	varchar2(60)		-- Πληροφορίες από το WebService της ΔΕΔΔΗΕ
  
	--- Payload: Conclusion (computed fields that are written here denormalized) ---
,	"HouseholdMemberCount"			number(2,0)
,	"MemberCount"			        number(9,0)         -- excluding applicant (personId=0)
,   "HouseholdIncomeValue"          number(18,2)
,   "HouseholdRealEstateValue"      number(18,2)
,   "HouseholdInterestsValue"       number(18,2)
,	"HouseholdVehiclesValue"		number(18,2)
,	"SingleParentFamilies"	        number(9,0)
,	"IsSingleParentFamily"			number(1,0)			Constraint "CC_NEE_App_IsSingleParentFam"     Check ("IsSingleParentFamily"	Between 0 and 1)	-- bool: yes/no

	--- Payload: State ---

,	"DraftAt"						timestamp
,	"DraftBy"						varchar2(256)	

,	"CanceledAt"					timestamp
,	"CanceledBy"					varchar2(256)	

,	"RejectedAt"					timestamp
,	"RejectedBy"					varchar2(256)

,	"ApprovedAt"					timestamp
,	"ApprovedBy"					varchar2(256)	

,	"ArchiveReason"					varchar2(1000)	
   
    --- Payload: Other ---

,	"ClonedFromNEE_AppId"			varchar2(29)

	--- Control: State ---

,	"CreatedAt"						timestamp			Default Current_Timestamp
														Constraint "CN_NEE_App_CreatedAt"			Not Null Enable
,	"CreatedBy"						varchar2(256)	
,	"ModifiedAt"					timestamp			
,	"ModifiedBy"					varchar2(256)	

	--- Control: Special/Computed ---
,	"PayFrom"						timestamp
,	"PayTo"							timestamp
,   "Monthly_Loan_Amount"			number(18,2)
,   "IsCreatedByKK"                 number(1)  
,   "EntitledAmount"                number(18,2)
,   "IsSingleParentFamilyFromKEA"   number(1,0)
,   "IsSingleParentFamilyConfirmed" number(1,0)
,	"SuspendedAt"					timestamp
,	"SuspendedBy"					varchar2(256)
,	"RecalledAt"					timestamp
,	"RecalledBy"					varchar2(256)
,   "DeclarationLaw1599"            number(1,0)
,   "ExternalProgramHouseholdId"           varchar2(200)

);

/
Create Index "CP_NEE_App_State_Id"	On "NEE_App" ("State", "Id");
/
Create Index "IX_NEE_App_Id"	        On "NEE_App" (SYS_OP_C2C("Id"));             --<<< Special Index!!
Create Index "IX_NEE_App_State_Id"	On "NEE_App" ("State", SYS_OP_C2C("Id"));    --<<< Special Index!!

Create Unique Index "IX_NEE_UnqAfmState" 
On "NEE_App" ("AFM", Case When "State"=0 Then 0 Else to_number(Replace("Id",'-',''))  End);

Create Unique Index "IX_NEE_UnqAfmAppIndex" 
On "NEE_App" ("AFM", "ApplicationIndex");

Set Define Off

   COMMENT ON COLUMN "NEE_App"."Id" IS 'Το Id (μοναδικός κωδικός) της αίτησης (xxx-xxx-xxx-xxx-xxx)';
   COMMENT ON COLUMN "NEE_App"."Type" IS '0=Αίτηση για Ενοίκιο, 1=Αίτηση για Δάνειο';
   COMMENT ON COLUMN "NEE_App"."Revision" IS 'Αυξάνεται κατά ένα μετά από κάθε αποθήκευση';
   COMMENT ON COLUMN "NEE_App"."State" IS '0=Νέα, 1=Ακυρωμένη, 3=Υποβληθείσα - Προς επεξεργασία, 4=Μη έγκριση, 5=Εγκεκριμένη';
   COMMENT ON COLUMN "NEE_App"."TenancyNumber" IS 'Αριθμός Μισθωτηρίου που καταχωρεί ο αιτών';
   COMMENT ON COLUMN "NEE_App"."AADE_TenancyNumber" IS 'Πληροφορίες από το WebService της ΑΑΔΕ';
   COMMENT ON COLUMN "NEE_App"."AADE_TenancyStartDate" IS 'Πληροφορίες από το WebService της ΑΑΔΕ';
   COMMENT ON COLUMN "NEE_App"."AADE_TenancyEndDate" IS 'Πληροφορίες από το WebService της ΑΑΔΕ';
   COMMENT ON COLUMN "NEE_App"."AADE_TenancyAccepted" IS 'Πληροφορίες από το WebService της ΑΑΔΕ';
   COMMENT ON COLUMN "NEE_App"."AADE_TenancyPowerMeterNumber" IS 'Πληροφορίες από το WebService της ΑΑΔΕ';
   COMMENT ON COLUMN "NEE_App"."AADE_RentAmount" IS 'Πληροφορίες από το WebService της ΑΑΔΕ';
   COMMENT ON COLUMN "NEE_App"."PowerMeterNumber" IS 'Αριθμός παροχής ηλεκτρικού ρεύματος που καταχωρεί ο αιτών';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_PowerMeterNumber" IS 'Αρ. Παροχής/Μετρητή ΔΕΔΔΗΕ: 9 ή 9+2 ψηφία (XXXXXXXXX ή XXXXXXXXXNN, πχ: 123456789 ή 12345678901)';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_AFM" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_IsActive" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_IsHomeUsage" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_Street" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_StreetNumber" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_ZipCode" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_City" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_CallikratianMunicipal" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_UsageTypeCode" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."DEDDIE_UsageTypeDescription" IS 'Πληροφορίες από το WebService της ΔΕΔΔΗΕ';
   COMMENT ON COLUMN "NEE_App"."HouseholdMemberCount" IS 'Αριθμός μελών νοικοκυριου';
   COMMENT ON COLUMN "NEE_App"."MemberCount" IS 'Αριθμός μελών νοικοκυριού χωρίς τον αιτούντα';
   COMMENT ON COLUMN "NEE_App"."HouseholdIncomeValue" IS 'Εισόδημα νοικοκυριού';
   COMMENT ON COLUMN "NEE_App"."HouseholdRealEstateValue" IS 'Ακίνητη περιουσία νοικοκυριού';
   COMMENT ON COLUMN "NEE_App"."HouseholdInterestsValue" IS 'Τόκοι καταθέσεων νοικοκυριού';
   COMMENT ON COLUMN "NEE_App"."SingleParentFamilies" IS 'Μονογονεϊκή οικογένεια';
   COMMENT ON COLUMN "NEE_App"."CreatedAt" IS 'Η ημερομηνία/ώρα που δημιουργήθηκε η εγγραφή';
   COMMENT ON COLUMN "NEE_App"."CreatedBy" IS 'Ο χρήστης που δημιούργησε την εγγραφή (name/email από τον πίνακα AspNetUsers)';
   COMMENT ON COLUMN "NEE_App"."ModifiedAt" IS 'Η ημερομηνία/ώρα τελευταίας μεταβολής της εγγραφής';
   COMMENT ON COLUMN "NEE_App"."ModifiedBy" IS 'Ο χρήστης τελευταίας μεταβολής της εγγραφής (name/email από τον πίνακα AspNetUsers)';
--- Payload: General ---
--- End ---
Set Define On
/
----------------------------------------------------------------------------------

Create Table "NEE_AppPerson"
(
	--- Control: Identity ---

	"Id"						    varchar2(29)	    Constraint "CN_NEE_AppPerson_Id"				Not Null Enable
														Constraint "CF_NEE_AppPerson_Id"			    References "NEE_App"("Id")	

,	"PersonId"						number(17,0)		Default 0
														Constraint "CN_NEE_AppPerson_PersonId"			Not Null Enable

,	"MemberNo"						number(9,0)			Default 0
														Constraint "CN_NEE_AppPerson_MemberNo"			Not Null Enable

,	"MemberSource"					number(1,0)			Default 0
														Constraint "CN_NEE_AppPerson_MemberSource"		Not Null Enable
														Constraint "CC_NEE_AppPerson_MemberSource"		Check ("MemberSource" Between 0 and 9)		-- 0: User, 1: System, ... (could extend to 2:Imported, etc.)

,	"MemberState"					number(1,0)			Default 0
														Constraint "CN_NEE_AppPerson_MemberState"		Not Null Enable
														Constraint "CC_NEE_AppPerson_MemberState"		Check ("MemberState" Between 0 and 9)		-- 0: Normal (Include), 1: Deleted (Ignore), ...

	--- Payload: Identity ---

,	"Relationship"					number(1,0)			Constraint "CC_NEE_AppPerson_Relationship"		Check ("Relationship" Between 1 and 9)		-- 1: spouse, 2: child, 3:parent, 4:other-relative, 5:other, ...

,	"OriginatorAMKA"				varchar2(11)
,	"OriginatedRelationship"		number(1,0)			Constraint "CC_NEE_AppPerson_OriginatedRel"		Check ("OriginatedRelationship" Between 0 and 9)	-- 1: spouse, 2: child, 3:parent, 4:other-relative, 5:other, ...


,	"AMKA"							varchar2(11)        Constraint "CN_NEE_AppPerson_AMKA"				Not Null Enable
,	"AFM"							varchar2(9)

	--- Payload: Personal-Info ---

,	"LastName"						varchar2(100)
,	"FirstName"						varchar2(100)
,	"FatherName"					varchar2(100)
,	"MotherName"					varchar2(100)
,	"LastNameEN"					varchar2(100)
,	"FirstNameEN"					varchar2(100)
,	"FatherNameEN"					varchar2(100)
,	"MotherNameEN"					varchar2(100)
,	"DOB"							date

,	"Gender"						number(1,0)			Constraint "CC_NEE_AppPerson_Gender"			Check ("Gender" Between 1 and 2)			-- 1: male, 2: female

,   "CitizenCountry"				varchar2(30)

	--- Payload: Possessions ---

,	"HasLuxuryItem"					number(1,0)			Constraint "CC_NEE_AppPerson_HasLuxuryItem"		Check ("HasLuxuryItem"		Between 0 and 1)	-- bool: yes/no

	--- Payload: Financial: Income ---

,	"TaxableIncome"			        number(18,2)
,	"SalaryIncome"					number(18,2)
,	"BenefitsIncome"				number(18,2)
,	"OtherIncome"					number(18,2)

	--- Payload: Financial: Valuables ---

,	"InterestsValue"				number(18,2)
,	"RealEstateValue"				number(18,2)
,	"VehiclesValue"					number(18,2)
	--- Payload: General ---

--,	"OaedRegistrationNumber"		varchar2(10)
--,	"UnemployedSince"				date

	--- Payload: ---

,	"IsGuest"					    number(1,0)			Constraint "CC_NEE_AppPerson_IsGuest"		    Check ("IsGuest"		        Between 0 and 1)	-- bool: yes/no
,	"HostAFM"					    varchar2(9)
,	"IsForeignResident"			    number(1,0)			Constraint "CC_NEE_AppPerson_IsForeignRes"       Check ("IsForeignResident"	    Between 0 and 1)	-- bool: yes/no
,	"DisabledPersons"			    number(9,0)
,	"HasTaxisCredentials"	        number(1,0)			Constraint "CC_NEE_AppPerson_HasTaxisCred"       Check ("HasTaxisCredentials"    Between 0 and 1)	-- bool: yes/no
,	"NumTaxis"					    number(9,0)
,	"HasTaxis"					    number(1,0)			Constraint "CC_NEE_AppPerson_HasTaxis"		    Check ("HasTaxis"		        Between 0 and 1)	-- bool: yes/no
,	"AMKARegistrationDate"			date
,	"IsTechnicalSupported"	        number(1,0)			Constraint "CC_NEE_AppPerson_IsTechSup"       Check ("IsTechnicalSupported"	    Between 0 and 1)	-- bool: yes/no
,	"TechSupCertification"			varchar2(100)
,	"IsSingleParentFamily"	        number(1,0)			Constraint "CC_NEE_AppPerson_IsSiPaFamily"       Check ("IsSingleParentFamily"	    Between 0 and 1)	-- bool: yes/no

--,	"MAM"					        number(9,0)
--,	"InsuredMonths"			        number(9,0)
--,	"ReductionAmount"				number(18,2)
--,	"UnemploymentMonths"			number(9,0)

	--- Payload: Conclusion (computed fields that are written here denormalized) ---

,   "IncomeSum"                     number(18,2)

	--- Payload: Consent ---

,	"NeedConsent"	    			number(1,0)			Constraint "CC_NEE_AppPerson_NeedConsent"	Check ("NeedConsent"    Between 0 and 1)	-- bool: yes/no
,	"ConsentedValue"			    number(1,0)			Constraint "CC_NEE_AppPerson_ConsentedVal"	Check ("ConsentedValue" Between 0 and 1)	-- bool: yes/no
,	"ConsentedAt"					timestamp			Default Current_Timestamp
,	"ConsentedBy"					varchar2(256)	--	Constraint "CF_NEE_AppPerson_ConsentedBy"	References "AspNetUsers"("UserName")

	--- Control: State ---

,	"CreatedAt"						timestamp			Default Current_Timestamp
														Constraint "CN_NEE_AppPerson_CreatedAt"		Not Null Enable
,	"CreatedBy"						varchar2(256)	--	Constraint "CF_NEE_AppPerson_CreatedBy"		References "AspNetUsers"("UserName")

,	"ModifiedAt"					timestamp			Default Current_Timestamp
														Constraint "CN_NEE_AppPerson_ModifiedAt"		Not Null Enable
,	"ModifiedBy"					varchar2(256)	--	Constraint "CF_NEE_AppPerson_ModifiedBy"		References "AspNetUsers"("UserName"),
,	"DOD"							date
,	"DeathStatus"					number
,	"IsStudent"						number(1,0)
,	"IsUnprotectedMember"			number(1,0)
,	"IsRelationshipValidated"		number(1,0)
,	"EntityId"						varchar2(31)
,	"Revision"						number
,	"IdentificationNumber"			varchar2(100)
,	"IdentificationType"			varchar2(1)
,	"IdentificationEndDate"			date
,   "IdentificationNumberConfirmed" number(1,0)
,   "IsUnprotectedMemberFromKEA"	number(1,0)
,   "DeletedAt"                     timestamp			Default Current_Timestamp
,   "DeletedBy"						varchar2(256)
,   "RestoredAt"                    timestamp			Default Current_Timestamp
,    "RestoredBy"					varchar2(256)
,    "IsUnprotectedMemberConfirmed" number(1,0)
,    "MaritalStatus"                number(1,0)
,    "EducationStatus"              number(9,0)
,    "DisabilityDegree"             number(3,0)
,    "EmploymentStatus"             number(1,0)
,    "IsActualRelationshipChild"    number(1,0)
,    "NoParticipationReason"        varchar2(200)

,	"PermitDocumentType"			number(9,0)
,	"IdentificationTypeFromUser"	number(9,0)
,	"BlueCardType"					varchar2(50)

	--- Table-Level-Constraints ---

,	Constraint "TP_NEE_AppPerson"		Primary Key ("Id", "PersonId") Enable
);
/
Create Index "IX_NEE_AppPerson_AMKA"		    On "NEE_AppPerson" ("AMKA");
Create Index "IX_NEE_AppPerson_AFM"			On "NEE_AppPerson" ("AFM");
Create Index "IX_NEE_AppPerson_Last_First"	On "NEE_AppPerson" ("LastName", "FirstName");
/
Create Index "IX_NEE_AppPerson_Id_"			On "NEE_AppPerson" (SYS_OP_C2C("Id"));	--<<< Special Index!!
Create Index "IX_NEE_AppPerson_AMKA_"		On "NEE_AppPerson" (SYS_OP_C2C("AMKA"));	--<<< Special Index!!
/
Set Define Off
--- Control: Identity ---
Comment On Column "NEE_AppPerson"."Id"				Is 'Το Id (μοναδικός κωδικός) της αίτησης (xxx-xxx-xxx-xxx-xxx)';
Comment On Column "NEE_AppPerson"."PersonId"		Is 'Id του μέλους της αίτησης (0: κυρίως/αιτών, αλλιώς κατά προτίμηση: yyyyMMddHHmmssfff σαν αριθμός, πχ:20160218235959000)';
--- Control: State ---
Comment On Column "NEE_AppPerson"."CreatedAt"		Is 'Η ημερομηνία/ώρα που δημιουργήθηκε η εγγραφή';
Comment On Column "NEE_AppPerson"."CreatedBy"		Is 'Ο χρήστης που δημιούργησε την εγγραφή (name/email από τον πίνακα AspNetUsers)';
Comment On Column "NEE_AppPerson"."ModifiedAt"		Is 'Η ημερομηνία/ώρα τελευταίας μεταβολής της εγγραφής';
Comment On Column "NEE_AppPerson"."ModifiedBy"		Is 'Ο χρήστης τελευταίας μεταβολής της εγγραφής (name/email από τον πίνακα AspNetUsers)';
--- Payload: General ---
Comment On Column "NEE_AppPerson"."MemberNo"		Is 'Ο αύξων αριθμός του μέλους της αίτησης, χρησιμοποιείται κυρίως για ταξινόμηση εμφάνισης (0: κυρίως/αιτών/δεν-ενδιαφέρει, 1...: εξαρτώμενα-μέλη)';
--- End ---
Set Define On
/


CREATE TABLE "NEE_AppRemark" 
   (	"Id" VARCHAR2(29 BYTE) CONSTRAINT "CN_NEE_AppRemark_Id" NOT NULL ENABLE, 
	"Name" VARCHAR2(50 BYTE) CONSTRAINT "CN_NEE_AppRemark_Name" NOT NULL ENABLE, 
	"Index" NUMBER(9,0) CONSTRAINT "CN_NEE_AppRemark_Index" NOT NULL ENABLE, 
	"RemarkCode" NUMBER(9,0) CONSTRAINT "CN_NEE_AppRemark_RemarkCode" NOT NULL ENABLE, 
	"Description" VARCHAR2(500 BYTE), 
	"Status" NUMBER(9,0) CONSTRAINT "CN_NEE_AppRemark_Status" NOT NULL ENABLE, 
	"Severity" NUMBER(9,0) CONSTRAINT "CN_NEE_AppRemark_Severity" NOT NULL ENABLE, 
	"Message" VARCHAR2(500 BYTE), 
	"RelatedAMKA" VARCHAR2(11 BYTE), 
	"RelatedAFM" VARCHAR2(9 BYTE), 
	"Released" NUMBER(1,0), 
	"ReleaseText" VARCHAR2(1000 BYTE), 
	"ReleasedAt" TIMESTAMP (6), 
	"ReleasedBy" VARCHAR2(256 BYTE), 
	"CreatedAt" TIMESTAMP (6) DEFAULT Current_Timestamp CONSTRAINT "CN_NEE_AppRemark_CreatedAt" NOT NULL ENABLE, 
	"CreatedBy" VARCHAR2(256 BYTE), 
	"ModifiedAt" TIMESTAMP (6) DEFAULT Current_Timestamp CONSTRAINT "CN_NEE_AppRemark_ModifiedAt" NOT NULL ENABLE, 
	"ModifiedBy" VARCHAR2(256 BYTE), 
	"EntityId"    VARCHAR2(31 BYTE), 
	"Revision"    number, 
	"ReferToMember" number(1,0)
	 CONSTRAINT "CC_NEE_AppRemark_Index" CHECK ("Index" > 0) ENABLE, 
	 CONSTRAINT "CN_NEE_AppRemark_Released" CHECK ("Released" Between 0 and 1) ENABLE, 
	 CONSTRAINT "TP_NEE_AppRemark" PRIMARY KEY ("Id", "Name", "Index")
  ENABLE, 
	 CONSTRAINT "CF_NEE_AppRemark_Id" FOREIGN KEY ("Id")
	  REFERENCES "NEE_App" ("Id") ENABLE
   ) ;

   COMMENT ON COLUMN "NEE_AppRemark"."Id" IS 'Το Id (μοναδικός κωδικός) της αίτησης (xxx-xxx-xxx-xxx-xxx)';
   COMMENT ON COLUMN "NEE_AppRemark"."Name" IS 'Το όνομα της ομάδας εγγραφών ελέγχων (ex: "default")';
   COMMENT ON COLUMN "NEE_AppRemark"."Index" IS 'Ο αύξων αριθμός της εγγραφής ελέγχου μέσα στην ομάδα εγγραφών (1,2,3,...)';
   COMMENT ON COLUMN "NEE_AppRemark"."CreatedAt" IS 'Η ημερομηνία/ώρα που δημιουργήθηκε η εγγραφή';
   COMMENT ON COLUMN "NEE_AppRemark"."CreatedBy" IS 'Ο χρήστης που δημιούργησε την εγγραφή (name/email από τον πίνακα AspNetUsers)';
   COMMENT ON COLUMN "NEE_AppRemark"."ModifiedAt" IS 'Η ημερομηνία/ώρα τελευταίας μεταβολής της εγγραφής';
   COMMENT ON COLUMN "NEE_AppRemark"."ModifiedBy" IS 'Ο χρήστης τελευταίας μεταβολής της εγγραφής (name/email από τον πίνακα AspNetUsers)';

  CREATE INDEX "IX_NEE_AppRemark_Id_Name" ON "NEE_AppRemark" ("Id", "Name")  ;

  CREATE INDEX "IX_NEE_AppRemark_Id_Name_" ON "NEE_AppRemark" (SYS_OP_C2C("Id"), SYS_OP_C2C("Name")) ;


CREATE TABLE "NEE_AppLog" 
(	
    "Id" VARCHAR2(29 BYTE) NOT NULL ENABLE, 
	"Revision" NUMBER(9,0),  
	"EventType" NUMBER(9,0), 
	"OccuredAt" TIMESTAMP (6),
    "UserName" VARCHAR2(256 BYTE)
) SEGMENT CREATION IMMEDIATE

CREATE TABLE "NEE_ChangeLog" 
(	
	"User" VARCHAR2(2000 BYTE) NOT NULL ENABLE, 
	"ModifiedAt" DATE NOT NULL ENABLE, 
	"ChangedField" VARCHAR2(2000 BYTE) NOT NULL ENABLE, 
	"OriginalValue" VARCHAR2(2000 BYTE), 
	"CurrentValue" VARCHAR2(2000 BYTE), 
	"EntityType" VARCHAR2(2000 BYTE) NOT NULL ENABLE, 
	"Id" VARCHAR2(200 BYTE) NOT NULL ENABLE, 
	"ChangeLogId" VARCHAR2(200 BYTE) NOT NULL ENABLE, 
	"Revision" NUMBER(9,0), 
	 CONSTRAINT "NEE_ChangeLog_PK" PRIMARY KEY ("Id", "ChangeLogId")
) SEGMENT CREATION IMMEDIATE 

