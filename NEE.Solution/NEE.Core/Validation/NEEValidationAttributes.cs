using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

namespace NEE.Core.Validation
{
    public class NEEValidationAttributes : RequiredAttribute
    {
        static NEEValidationAttributes()
        { }
        public NEEValidationAttributes() : base()
        {
            this.ErrorMessage = @"Το πεδίο ""{0}"" είναι υποχρεωτικό";
        }
    }

    public class NEEStringLengthAttribute : StringLengthAttribute
    {
        static NEEStringLengthAttribute()
        {
        }

        public NEEStringLengthAttribute(int maximumLength) : base(maximumLength)
        {
            this.ErrorMessage = @"Το μήκος του πεδίου ""{0}"" δεν πρέπει να ξεπερνά τους {1} χαρακτήρες";
        }

        public NEEStringLengthAttribute(int minimumLength, int maximumLength) : base(maximumLength)
        {
            this.MinimumLength = minimumLength;
            if (this.MinimumLength == this.MinimumLength)
                this.ErrorMessage = @"Το μήκος του πεδίου ""{0}"" πρέπει να είναι {1} χαρακτήρες";
            else
                this.ErrorMessage = @"Το μήκος του πεδίου ""{0}"" πρέπει να είναι μεταξύ {2} και {1} χαρακτήρες";
        }
    }

    public class NEEMustBeTrueAttribute: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }
        public NEEMustBeTrueAttribute() : base()
        {
            this.ErrorMessage = "Πρέπει να αποδεκτείτε την υπεύθυνη δήλωση N.1599";
        }
    }

    public class NEEDateDisplayFormatAttribute : DisplayFormatAttribute
    {
        public NEEDateDisplayFormatAttribute() : base()
        {
            this.DataFormatString = "{0:dd/MM/yyyy}";
            ApplyFormatInEditMode = true;
        }
    }

    public class NEEMoneyDisplayFormatAttribute : DisplayFormatAttribute
    {
        public NEEMoneyDisplayFormatAttribute() : base()
        {
            this.DataFormatString = "{0:C}";
        }
    }

    public class NEEMoneyNonNegativeAttribute : RangeAttribute
    {
        static NEEMoneyNonNegativeAttribute()
        {
            // NOTE: either uncomment the following line to self-register adapter or place it in global.asax !!
            //DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(GmiRequiredAttribute), typeof(RequiredAttributeAdapter));
        }

        public NEEMoneyNonNegativeAttribute() : base(0.00, double.MaxValue)
        {
            this.ErrorMessage = @"Το ποσό ""{0}"" δεν μπορεί να είναι αρνητικό";
        }
    }


    public class StringValidationAttribute : ValidationAttribute
    {
        private readonly Func<string, bool> _validationFunc;
        public StringValidationAttribute(Func<string, bool> validationFunc, string errorMessage)
        {
            if (validationFunc == null)
                throw new ArgumentNullException(nameof(validationFunc));

            _validationFunc = validationFunc;
            ErrorMessage = errorMessage;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (!(value is string))
                    throw new ValidationException("Validation only applies to string value");
                if (!_validationFunc(value as string))
                    return new ValidationResult(this.FormatErrorMessage(validationContext.MemberName), new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
        public override string FormatErrorMessage(string name)
        {
            return string.Format(this.ErrorMessageString, name);
        }
    }


    public class ZipCodeAttribute : StringValidationAttribute
    {
        public ZipCodeAttribute() : base(ZipCodes.Exists, "Ο Τ.Κ. δεν είναι έγκυρος") { }
    }

}
