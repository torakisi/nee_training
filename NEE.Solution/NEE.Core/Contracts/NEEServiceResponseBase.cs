using NEE.Core.BO;
using NEE.Core.Contracts.Enumerations;
using NEE.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEE.Core.Contracts
{
    public abstract class NEEServiceResponseBase
    {
        public bool _IsSuccessful { get; set; } = true;

        public List<Error> _Errors { get; private set; } = new List<Error>();

        [JsonIgnore]
        public bool _HasUnhandledErrors => _Errors.Any(x => x.Category == ErrorCategory.Unhandled);

        public IErrorLogger _errorLogger;
        public string _userName;

        public NEEServiceResponseBase(IErrorLogger errorLogger = null, string userName = null)
        {
            _errorLogger = errorLogger;
            _userName = userName;
        }

        // Helper Methods

        public void ClearErrors()
        {
            _Errors.Clear();
            _IsSuccessful = true;
        }



        public void AddErrors(List<Error> errors)
        {
            if (_Errors == null) _Errors = new List<Error>();

            foreach (Error error in errors)
            {
                AddError(error);
                _IsSuccessful = false;
            }
        }

        public void AddError(ErrorCategory category, string message = null, Exception ex = null)
        {
            string msg = string.IsNullOrEmpty(message) ? "Παρουσιάστηκε σφάλμα κατά το τελευταίο αίτημά σας. Παρακαλώ δοκιμάστε αργότερα." : message;

            if (ex is ApplicationMaintainanceModeException)
            {
                msg = ex.Message;
                category = ErrorCategory.UIDisplayed;
            }

            //if (category == ErrorCategory.Unhandled)
            //{
            //	OnUnhandledException(this, ex, message);
            //}

            Error error = new Error()
            {
                Category = category,
                Exception = ex,
                Message = msg
            };

            AddError(error);
        }

        private void AddError(Error error)
        {
            this._IsSuccessful = false;

            if ((error.Category == ErrorCategory.Unhandled || error.Category == ErrorCategory.Unauthorized) && !error.IsLogged)
                LoggingError(error);

            this._Errors.Add(error);
        }

        public static event EventHandler<NEEServiceResponseUnhandledExceptionEventArgs> UnhandledException;
        public class NEEServiceResponseUnhandledExceptionEventArgs
        {
            public Exception Exception { get; set; }
            public string Message { get; set; }
            public string User { get; set; }
        }
        private void OnUnhandledException(NEEServiceResponseBase response, Exception ex, string message)
        {
            if (UnhandledException != null)
            {
                UnhandledException.Invoke(response, new NEEServiceResponseUnhandledExceptionEventArgs
                {
                    Exception = ex,
                    Message = message
                });
            }
        }

        private string LoggingErrorException(Exception ex)
        {
            if (_errorLogger != null)
            {
                var errorLog = new ErrorLog()
                {
                    CreatedAt = DateTime.Now,
                    ErrorLogSource = Enumerations.ErrorLogSource.Service,
                    Exception = ex.ToString(),
                    User = _userName,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.ToString()
                };

                return _errorLogger.LogErrorAsync(errorLog);
            }

            return null;
        }
        private string LoggingError(Error error)
        {
            if (_errorLogger != null)
            {
                var errorLog = new ErrorLog()
                {
                    User = _userName,
                    CreatedAt = DateTime.Now,
                    Exception = "Υπήρξε σφάλμα"
                };

                if (error.Exception != null)
                {
                    errorLog.ErrorLogSource = Enumerations.ErrorLogSource.Service;
                    errorLog.Exception = error.Exception.ToString();
                    errorLog.StackTrace = error.Exception.StackTrace;
                    errorLog.InnerException = error.Exception.InnerException?.ToString();
                }
                else if (!string.IsNullOrEmpty(error.Message))
                {
                    errorLog.ErrorLogSource = Enumerations.ErrorLogSource.Service;
                    errorLog.Exception = error.Message;
                    errorLog.StackTrace = null;
                    errorLog.InnerException = null;
                }

                error.IsLogged = true;
                return _errorLogger.LogErrorAsync(errorLog);
            }

            return null;
        }

        [JsonIgnore]
        public string _ErrorsFormatted => string.Join(" - ", (_Errors ?? new List<Error>()).Select(p => $"{p.Message}").ToArray());

        public List<Error> UIDisplayedErrors
        {
            get
            {
                List<Error> ret = new List<Error>();

                if (_Errors.Any(x => x.Category == ErrorCategory.UIDisplayed))
                {
                    var _displayedErrors = _Errors.Where(x => x.Category == ErrorCategory.UIDisplayed).ToList();
                    foreach (Error error in _displayedErrors)
                    {
                        ret.Add(error);
                    }
                }
                else if (_Errors.Any(x => x.Category == ErrorCategory.UIDisplayedServiceCallFailure))
                {
                    var _serviceErrorsDisplayedErrors = _Errors.Where(x => x.Category == ErrorCategory.UIDisplayedServiceCallFailure).ToList();
                    foreach (Error error in _serviceErrorsDisplayedErrors)
                    {
                        ret.Add(error);
                    }
                }
                else if (_Errors.Any(x => x.Category == ErrorCategory.UIDisplayedGenericServiceFailure))
                {
                    ret.Add(_Errors.Where(x => x.Category == ErrorCategory.UIDisplayedGenericServiceFailure).First());
                }

                return ret;
            }
        }

        [JsonIgnore]
        public string UIDisplayedErrorsFormatted => string.Join(" - ", UIDisplayedErrors.Select(p => $"{p.Message}").ToArray());
    }

    public class Error
    {
        public ErrorCategory Category { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public bool IsLogged { get; set; } = false;
    }
}

