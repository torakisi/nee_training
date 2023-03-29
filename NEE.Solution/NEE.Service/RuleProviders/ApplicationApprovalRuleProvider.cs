using NEE.Core.BO;
using NEE.Core.Rules;
using NEE.Service.RuleProviders.Rules;

namespace NEE.Service.RuleProviders
{
    public class ApplicationApprovalRuleProvider : RuleProvider
    {
        private Application Application { get; set; }
        private AppService AppService { get; set; }

        public ApplicationApprovalRuleProvider(Application application, AppService appService)
        {
            Application = application;
            AppService = appService;

            DefineRules();
        }

        private void DefineRules()
        {
            AddRule(new ApplicationValidationAgeBelow67(Application)); //ΠΣ21
            AddRule(new ApplicationValidationErganhRecordFound(Application)); //ΠΣ5
            AddRule(new ApplicationValidationEdtoNotExpatriate(Application)); //ΣΟ1
            AddRule(new ApplicationValidationPensionExceeded(Application)); //ΟΣ2
            AddRule(new ApplicationValidationPensionFromAlbaniaExceeded(Application)); // ΟΣ10
            AddRule(new ApplicationValidationPensionSumExceeded(Application)); // ΟΣ10 + OS2
            AddRule(new ApplicationValidationAADEIncomeExceeded(Application)); //ΟΣ3
            AddRule(new ApplicationValidationAADEFamilyIncomeExceeded(Application)); //ΟΣ4
            AddRule(new ApplicationValidationAADERealEstateExceeded(Application)); //ΟΣ5
            AddRule(new ApplicationValidationAADEVehicleExceeded(Application)); //ΟΣ6
            AddRule(new ApplicationValidationHousingBenefitExceeded(Application)); //ΟΣ13
            AddRule(new ApplicationValidationHousingAssistanceBenefitExceeded(Application)); //ΟΣ14
            AddRule(new ApplicationValidationBenefitForOmogeneisExceeded(Application)); //ΟΣ15
            AddRule(new ApplicationValidationDisabilityBenefitsExceeded(Application)); //ΟΣ16
            AddRule(new ApplicationValidationA21BenefitExceeded(Application)); //ΟΣ17
            //documents
            AddRule(new ApplicationValidationFEKUploaded(Application)); //ΣΦ1
            AddRule(new ApplicationValidationPensionAlbaniaUploaded(Application)); //ΟΣ12
            AddRule(new ApplicationValidationMaritalStatusUploaded(Application)); //ΣΣ7
            AddRule(new ApplicationValidationSpousePensionAlbaniaUploaded(Application)); //ΣΣ14
            //spouse
            if (Application.Spouse != null)
            {
                AddRule(new ApplicationValidationSpousePensionExceeded(Application)); //ΣΣ10
                AddRule(new ApplicationValidationSpousePensionFromAlbaniaExceeded(Application)); //ΣΣ14
                AddRule(new ApplicationValidationSpousePensionSumExceeded(Application)); // ΣΣ10 + ΣΣ14 > athroisma 387,9 high
            }
            //iban
            AddRule(new ApplicationValidationIbanNotFound(Application));
            AddRule(new ApplicationValidationIbanNotValid(Application));
            AddRule(new ApplicationValidationIbanNotOwned(Application));
            AddRule(new ApplicationValidationIbanUnsupported(Application));
        }
    }
}
