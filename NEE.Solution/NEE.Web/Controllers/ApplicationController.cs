using NEE.Core.Contracts.Enumerations;
using NEE.Core.Validation;
using NEE.Service.Core;
using NEE.Service;
using NEE.Web.Code;
using NEE.Web.Models.ApplicationViewModels;
using NEE.Web.Models.Core;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using static NEE.Web.Models.Core.ApplicationViewModel;
using System.Web;
using System.Collections.Generic;
using NEE.Core.BO;
using System;
using NEE.Service.FileService;
using NEE.Core.Contracts;
using System.Globalization;
using System.Net.Mime;
using Newtonsoft.Json.Linq;

namespace NEE.Web.Controllers
{
    [Authorize]
    public class ApplicationController : NEEBaseController
    {
        private AppService _gsAppService;
        private FileAccessService _fileAccessService;
        private IErrorLogger _errorLogger;

        public ApplicationController(AppService gsAppService, UserService gsUserService, FileAccessService fileAccessService, IErrorLogger errorLogger)
            : base(gsUserService)
        {
            _gsAppService = gsAppService;
            _fileAccessService = fileAccessService;
            _errorLogger = errorLogger;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> ManageApplications(string AMKA = "", string AFM = "")
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.ManageApplications
            };

            GetApplicationOwnerRequest req = new GetApplicationOwnerRequest()
            {
                AFM = (this.UserInfo == null) ? AFM : this.UserInfo.AFM,
                AMKA = (this.UserInfo == null) ? AMKA : this.UserInfo.AMKA
            };

            GetApplicationOwnerResponse resp = await _gsAppService.SetWebUIContext(context).GetApplicationOwner(req);
            if (!resp._IsSuccessful)
            {
                ViewBag.errorMessage = ServiceErrorMessages.UIActionFailedMessage;
                ViewBag.errorDescription = resp.UIDisplayedErrorsFormatted;

                return View("Error");
            }
            ApplicationManagementViewModel model = new ApplicationManagementViewModel()
            {
                ApplicationOwner = resp.ApplicationOwner
            };

            return View(model);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> Edit(string id, string returnUrl)
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.EditApplication
            };

            PRGApplyModelState();
            //FillDictionariesAsync();

            GetApplicationRequest req = new GetApplicationRequest()
            {
                Id = id,
                ForWhatUse = GetApplicationUse.EditApplication
                // pass the Remote Host IP for the IBAN Validation WS 
                ,
                RemoteHostIP = NEECurrentUserContext.GetRemoteHostIPs().First()
            };

            GetApplicationResponse resp = await _gsAppService.SetWebUIContext(context).GetApplicationAsync(req);

            if (!resp._IsSuccessful)
            {
                ViewBag.errorMessage = ServiceErrorMessages.UIActionFailedMessage;
                ViewBag.errorDescription = resp.UIDisplayedErrorsFormatted;

                return View("Error");
            }

            ApplicationViewModel model = resp.Application.Map();
            model.IsNormalUser = IsNormalUser();
            model.ReturnUrl = returnUrl;
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.IsFromPrint = TempData["IsFromPrint"];
            ViewBag.IsFromPrintWholeApplication = TempData["IsFromPrintWholeApplication"];
            ViewBag.Action = TempData["Action"];
            ViewBag.CanEditResults = Session["CanEditResults"];


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageApplications(ApplicationManagementViewModel model, string command)
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.ManageApplicationsPost
            };

            ApplicationEditCommands commandName = ApplicationEditCommands.ViewApplication;
            string appId = "";
            string returnUrl = "";
            string canEditResultsStr = "";
            bool canEditResults = true;
            string applicantAfm = "";
            string applicantAmka = "";

            var commandParams = command.Split(':');

            if (command.StartsWith("CreateNewApplication:"))
            {
                if (commandParams.Count() == 3)
                {
                    applicantAfm = commandParams[1];
                    applicantAmka = commandParams[2];
                }
                if (commandParams.Count() == 4)
                {
                    applicantAfm = commandParams[1];
                    applicantAmka = commandParams[2];
                    returnUrl = HttpUtility.UrlDecode(commandParams[3]);
                }

                if (!string.IsNullOrEmpty(applicantAfm) && !string.IsNullOrEmpty(applicantAmka))
                    commandName = ApplicationEditCommands.CreateNewApplication;
            }

            else if (command.StartsWith("EditApplication:"))
            {
                if (commandParams.Count() == 2)
                {
                    appId = commandParams[1];
                }
                if (commandParams.Count() == 3)
                {
                    appId = commandParams[1];
                    returnUrl = HttpUtility.UrlDecode(commandParams[2]);
                    canEditResultsStr = returnUrl.Split(new string[] { "CanEditResults=" }, StringSplitOptions.None)[1];
                    bool.TryParse(canEditResultsStr, out canEditResults);
                }
                if (!string.IsNullOrEmpty(appId))
                    commandName = ApplicationViewModel.ApplicationEditCommands.EditApplication;
            }

            else if (command.StartsWith("ViewApplicationReadOnly:"))
            {
                if (commandParams.Count() == 2)
                {
                    appId = commandParams[1];
                }
                if (commandParams.Count() == 3)
                {
                    appId = commandParams[1];
                    returnUrl = HttpUtility.UrlDecode(commandParams[2]);
                    canEditResultsStr = returnUrl.Split(new string[] { "CanEditResults=" }, StringSplitOptions.None)[1];
                    bool.TryParse(canEditResultsStr, out canEditResults);
                }

                if (!string.IsNullOrEmpty(appId))
                    commandName = ApplicationViewModel.ApplicationEditCommands.ViewApplicationReadOnly;
            }

            else if (command.StartsWith("ViewApplication:"))
            {
                if (commandParams.Count() == 2)
                {
                    appId = commandParams[1];
                }
                if (commandParams.Count() == 3)
                {
                    appId = commandParams[1];
                    returnUrl = HttpUtility.UrlDecode(commandParams[2]);
                    canEditResultsStr = returnUrl.Split(new string[] { "CanEditResults=" }, StringSplitOptions.None)[1];
                    bool.TryParse(canEditResultsStr, out canEditResults);
                }
                if (!string.IsNullOrEmpty(appId))
                    commandName = ApplicationViewModel.ApplicationEditCommands.ViewApplication;
            }

            switch (commandName)
            {
                case ApplicationViewModel.ApplicationEditCommands.CreateNewApplication:
                    return await CreateNewApplicationHandle(applicantAfm, applicantAmka, returnUrl);
                case ApplicationViewModel.ApplicationEditCommands.EditApplication:
                    return EditApplicationHandle(appId, returnUrl, canEditResults);
                case ApplicationViewModel.ApplicationEditCommands.ViewApplication:
                    return ViewApplicationHandle(appId, returnUrl, canEditResults);
                case ApplicationViewModel.ApplicationEditCommands.ViewApplicationReadOnly:
                    return ViewApplicationReadOnlyHandle(appId, returnUrl, canEditResults);
                default:
                    return await ErrorAsync();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationViewModel model, string command, string returnUrl)
        {
            var commandName = ApplicationEditCommands.ValidateSave;
            
            if (command == "Save") commandName = ApplicationEditCommands.Save;
            if (command == "ValidateSave") commandName = ApplicationEditCommands.ValidateSave;
            if (command == "ValidateSaveFinalSubmit") commandName = ApplicationEditCommands.ValidateSaveFinalSubmit;
            if (command == "BackInManageApplications") commandName = ApplicationEditCommands.RedirectToManage;
            if (command == "CancelReturn") commandName = ApplicationEditCommands.CancelReturn;
            if (command == "GoToPayments") commandName = ApplicationEditCommands.GoToPayments;
            if (command == "PensionAlbaniaUpload") commandName = ApplicationEditCommands.PensionAlbaniaUpload;
            if (command == "SpousePensionAlbaniaUpload") commandName = ApplicationEditCommands.SpousePensionAlbaniaUpload;
            if (command == "MaritalStatusUpload") commandName = ApplicationEditCommands.MaritalStatusUpload;
            if (command == "FEKUpload") commandName = ApplicationEditCommands.FEKUpload;
            if (command == "ApproveMaritalStatus") commandName = ApplicationEditCommands.ApproveMaritalStatus;
            if (command == "RejectMaritalStatus") commandName = ApplicationEditCommands.RejectMaritalStatus;
            if (command == "ApprovePensionAlbania") commandName = ApplicationEditCommands.ApprovePensionAlbania;
            if (command == "RejectPensionAlbania") commandName = ApplicationEditCommands.RejectPensionAlbania;
            if (command == "ApproveSpousePensionAlbania") commandName = ApplicationEditCommands.ApproveSpousePensionAlbania;
            if (command == "RejectSpousePensionAlbania") commandName = ApplicationEditCommands.RejectSpousePensionAlbania;
            if (command == "ApproveFEK") commandName = ApplicationEditCommands.ApproveFEK;
            if (command == "RejectFEK") commandName = ApplicationEditCommands.RejectFEK;
            if (command == "FileDownload") commandName = ApplicationEditCommands.FileDownload;

            model.IsNormalUser = IsNormalUser();
            switch (commandName)
            {
                case ApplicationEditCommands.ValidateSave:
                    return await ValidateSaveHandle(model);
                case ApplicationEditCommands.Save:
                    var saveRespSave = await Save(model);
                    if (!saveRespSave._IsSuccessful)
                    {
                        ModelState.Clear();
                        foreach (var item in saveRespSave.UIDisplayedErrors)
                        {
                            ModelState.AddModelError("", item.Message);
                        }
                        return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
                    }
                    return RedirectToEdit(model.Id, withModelState: false, returnUrl: model.ReturnUrl);
                case ApplicationEditCommands.ValidateSaveFinalSubmit:
                    return await ValidateSaveFinalSubmitHandle(model);
                case ApplicationEditCommands.RedirectToManage:
                    return RedirectToManageAppPage();
                case ApplicationEditCommands.CancelReturn:
                    return await CancelHandle(model);
                case ApplicationEditCommands.GoToPayments:
                    await ValidateSaveHandle(model);
                    return RedirectToAction("Index", "Payments", new { id = model.Id });
                case ApplicationEditCommands.PensionAlbaniaUpload:
                    model.DocumentCategory = DocumentCategory.PensionAlbania;
                    await ValidateSaveHandle(model);
                    await FileUploadHandle(model);
                    return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
                case ApplicationEditCommands.SpousePensionAlbaniaUpload:
                    model.DocumentCategory = DocumentCategory.SpousePensionAlbania;
                    await ValidateSaveHandle(model);
                    await FileUploadHandle(model);
                    return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
                case ApplicationEditCommands.MaritalStatusUpload:
                    model.DocumentCategory = DocumentCategory.MaritalStatus;
                    await ValidateSaveHandle(model);
                    await FileUploadHandle(model);
                    return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
                case ApplicationEditCommands.FEKUpload:
                    model.DocumentCategory = DocumentCategory.FEK;
                    await ValidateSaveHandle(model);
                    await FileUploadHandle(model);
                    return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
                case ApplicationEditCommands.FileDownload:
                    return RedirectToEdit(model.Id, withModelState: false, returnUrl: model.ReturnUrl);
                case ApplicationEditCommands.ApproveMaritalStatus:
                    model.IsMaritalStatusDocumentApproved = true;
                    return await SaveDocumentHandle(model, DocumentCategory.MaritalStatus, true);
                case ApplicationEditCommands.RejectMaritalStatus:
                    model.IsMaritalStatusDocumentApproved = false;
                    return await SaveDocumentHandle(model, DocumentCategory.MaritalStatus, false);
                case ApplicationEditCommands.ApprovePensionAlbania:
                    model.IsPensionAlbaniaDocumentApproved = true;
                    return await SaveDocumentHandle(model, DocumentCategory.PensionAlbania, true);
                case ApplicationEditCommands.RejectPensionAlbania:
                    model.IsPensionAlbaniaDocumentApproved = false;
                    return await SaveDocumentHandle(model, DocumentCategory.PensionAlbania, false);
                case ApplicationEditCommands.ApproveSpousePensionAlbania:
                    model.IsSpousePensionDocumentApproved = true;
                    return await SaveDocumentHandle(model, DocumentCategory.SpousePensionAlbania, true);
                case ApplicationEditCommands.RejectSpousePensionAlbania:
                    model.IsSpousePensionDocumentApproved = false;
                    return await SaveDocumentHandle(model, DocumentCategory.SpousePensionAlbania, false);
                case ApplicationEditCommands.ApproveFEK:
                    model.IsFEKDocumentApproved = true;
                    return await SaveDocumentHandle(model, DocumentCategory.FEK, true);
                case ApplicationEditCommands.RejectFEK:
                    model.IsFEKDocumentApproved = false;
                    return await SaveDocumentHandle(model, DocumentCategory.FEK, false);
                default:
                    return await ErrorAsync();
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> ApplicationSubmitted(string id, string returnUrl = "")
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.ViewApplication
            };

            ApplicationSubmittedDetailsResponse res = await _gsAppService.SetWebUIContext(context).ApplicationSubmittedDetailsAsync(new ApplicationSubmittedDetailsRequest
            {
                Id = id,
                ForWhatUse = GetApplicationUse.SubmittedDetails
                // pass the Remote Host IP for the IBAN Validation WS 
                ,
                RemoteHostIP = NEECurrentUserContext.GetRemoteHostIPs().First()
            });

            if (!res._IsSuccessful)
            {
                ModelState.Clear();
                foreach (var item in res.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", item.Message);
                }
                return RedirectToEdit(id, withModelState: true, returnUrl: returnUrl);

            }
            var model = res.Application.Map();
            model.ReturnUrl = returnUrl;
            ViewBag.PrintSubmit = TempData["PrintSubmit"];
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationSubmitted(ApplicationViewModel model, string command)
        {
            if (command == "BackInEdit")
            {
                return RedirectToEdit(model.Id, withModelState: false, returnUrl: model.ReturnUrl);
            }
            TempData["PrintSubmit"] = true;
            return RedirectToAction(nameof(ApplicationSubmitted), new { id = model.Id, returnUrl = model.ReturnUrl });

        }

        #region Action: Upload / Download File
        private async Task FileUploadHandle(ApplicationViewModel model)
        {
            try
            {
                WebUIContext context = new WebUIContext()
                {
                    WebUIAction = WebUIAction.FileUpload
                };
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase postedFile = Request.Files[file];
                    if (postedFile.ContentLength > 0)
                    {
                        // prepare for database submit:
                        string fileName = postedFile.FileName;
                        string contentType = postedFile.ContentType;

                        if (contentType != "application/pdf")
                        {

                        }

                        var req = new UploadFileRequest
                        {
                            AppId = model.Id,
                            Files = new AppFileSave[]
                            {
                        new AppFileSave
                        {
                            ApplicationId= model.Id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ContentStream = postedFile.InputStream,
                            ContentType = postedFile.ContentType,
                            FileName = fileName,
                            FileSize = postedFile.ContentLength.ToString(),
                            FileType = postedFile.ContentType,
                            UploadedFromIP = string.Join(", ", NEECurrentUserContext.GetRemoteHostIPs()),
                            DocumentCategory = model.DocumentCategory
                        }
                            }
                        };
                        UploadFileResponse res = await _gsAppService.SetWebUIContext(context).UploadFileAsync(req);

                        if (!res._IsSuccessful)
                        {
                            ModelState.Clear();
                            foreach (var item in res.UIDisplayedErrors)
                            {
                                ModelState.AddModelError("", item.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var log1 = new ErrorLog()
                {
                    CreatedAt = DateTime.Now,
                    ErrorLogSource = ErrorLogSource.Service,
                    Exception = ex.ToString(),
                    User = "TEST",
                    StackTrace = "fileUpload",
                    InnerException = ex.InnerException.ToString()
                };
                _errorLogger.LogErrorAsync(log1);
            }            
        }
        [HttpGet]
        public async Task<FileResult> GetDocumentById(string appId, string fileId)
        {
            var req = new GetFileByIdRequest
            {
                UploadedFileId = fileId,
                ApplicationId = appId

            };

            var log1 = new ErrorLog()
            {
                CreatedAt = DateTime.Now,
                ErrorLogSource = ErrorLogSource.Service,
                Exception = "log1",
                User = "TEST",
                StackTrace = "log1",
                InnerException = ""
            };
            _errorLogger.LogErrorAsync(log1);

            GetFileByIdResponse response = await _fileAccessService.GetFileByIdAsync(req);
            if (response._IsSuccessful)
            {
                ContentDisposition cd = new ContentDisposition
                {
                    FileName = response.FileName,
                    Inline = false
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());
                return File(response.File, response.FileContentType);
            }
            return null;
        }

        #endregion

        private async Task<ActionResult> CreateNewApplicationHandle(string applicantAfm, string applicantAmka, string returnUrl = "")
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.CreateNewApplication
            };

            CreateApplicationRequest request = new CreateApplicationRequest()
            {
                AFM = (UserInfo == null) ? applicantAfm : UserInfo.AFM,
                AMKA = (UserInfo == null) ? applicantAmka : UserInfo.AMKA,
                RemoteHostIP = NEECurrentUserContext.GetRemoteHostIPs().First()// pass the Remote Host IP for the IBAN Validation WS
            };

            CreateApplicationResponse response = await _gsAppService.SetWebUIContext(context).CreateApplicationAsync(request);

            if (!response._IsSuccessful)
            {
                ViewBag.errorMessage = ServiceErrorMessages.UIActionCreateApplicationFailedMessage;
                ViewBag.errorDescription = response.UIDisplayedErrorsFormatted;

                return View("Error");
            }

            if (response.CreatedApplication.Remarks.Where(r => r.RemarkCode != RemarkType.EdtoNotExpatriate).Any(r => r.ReasonForNoApproval))
            {
                RejectNonEligibleApplicationRequest rejectReq = new RejectNonEligibleApplicationRequest()
                {
                    Application = response.CreatedApplication
                };
                await _gsAppService.SetWebUIContext(context).RejectNonEligibleApplication(rejectReq);
                return RedirectToAction(nameof(ApplicationSubmitted), new { id = response.CreatedApplication.Id });
            }

            return RedirectToEdit(response.CreatedApplication.Id, withModelState: false, returnUrl: returnUrl);
        }

        private ActionResult EditApplicationHandle(string applicationId, string returnUrl = "", bool canEditResults = true)
        {
            return RedirectToEdit(applicationId, withModelState: false, returnUrl: returnUrl, canEditResults: canEditResults);
        }
        private ActionResult ViewApplicationHandle(string applicationId, string returnUrl = "", bool canEditResults = true)
        {
            return RedirectToEdit(applicationId, withModelState: false, returnUrl: returnUrl, canEditResults: canEditResults);
        }
        private ActionResult ViewApplicationReadOnlyHandle(string applicationId, string returnUrl = "", bool canEditResults = true)
        {
            return RedirectToEdit(applicationId, withModelState: false, returnUrl: returnUrl, canEditResults: canEditResults);
        }
        private async Task<ActionResult> CancelHandle(ApplicationViewModel model)
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.CancelApplication
            };

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                return Error();
            }

            var res = await _gsAppService.SetWebUIContext(context).CancelApplicationAsync(new CancelApplicationRequest
            {
                Id = model.Id,
                Revision = model.Revision,
                RemoteHostIP = NEECurrentUserContext.GetRemoteHostIPs().First(),
                IsModelStateValid = ModelState.IsValid
            });

            if (!res._IsSuccessful)
            {
                ModelState.Clear();
                foreach (var item in res.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", item.Message);
                }
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            if (!string.IsNullOrEmpty(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
                return RedirectToManageAppPage();
        }

        private ActionResult RedirectToEdit(string id, bool withModelState, string returnUrl, string action = null, bool isFromPrint = false, bool printWholeApp = false, bool canEditResults = true)
        {
            TempData["IsFromPrint"] = isFromPrint;
            TempData["IsFromPrintWholeApplication"] = printWholeApp;
            TempData["Action"] = action;
            Session["CanEditResults"] = canEditResults;

            return withModelState ? 
                ModelStateIncludeAction(nameof(Edit), new { id, returnUrl }) : 
                RedirectToAction(nameof(Edit), new { id, returnUrl });
        }
        private ActionResult RedirectToManageAppPage() =>
            RedirectToAction(nameof(ManageApplications));

        private async Task<SaveApplicationResponse> Save(ApplicationViewModel model)
        {
            SaveApplicationResponse res = await SaveApplicationHandle(model);
            return res;
        }

        private async Task<SaveApplicationResponse> SaveApplicationHandle(ApplicationViewModel model)
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.SaveApplication
            };

            var req = MapSaveApplicationRequest(model);
            SaveApplicationResponse res = await _gsAppService.SetWebUIContext(context).SaveApplicationAsync(req);
            return res;
        }
        
        private async Task<ActionResult> ValidateSaveHandle(ApplicationViewModel model, bool validateModel = true)
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.ValidateSaveApplication
            };

            var req = MapSaveApplicationRequest(model);

            SaveApplicationResponse res = await _gsAppService.SetWebUIContext(context).SaveApplicationAsync(req);

            if (!res._IsSuccessful)
            {
                ModelState.Clear();
                foreach (var item in res.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", item.Message);
                }
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            if (!ModelState.IsValid && validateModel)
            {
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            model.IsSuccesfull = true;
            return RedirectToEdit(model.Id, withModelState: false, returnUrl: model.ReturnUrl, action: "save");
        }

        private async Task<ActionResult> SaveDocumentHandle(ApplicationViewModel model, DocumentCategory docCategory, bool isApproved)
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.ValidateSaveApplication
            };

            var req = MapSaveApplicationRequest(model);

            SaveApplicationResponse res = await _gsAppService.SetWebUIContext(context).SaveApplicationAsync(req);

            if (!res._IsSuccessful)
            {
                ModelState.Clear();
                foreach (var item in res.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", item.Message);
                }
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            string fileUrl = "";
            string rejectionReason = "";
            switch (docCategory)
            {
                case DocumentCategory.FEK:
                    fileUrl = res.FEKDocumentId;
                    rejectionReason = model.FEKRejectionReason;
                    break;
                case DocumentCategory.PensionAlbania:
                    fileUrl = res.PensionDocumentAlbaniaId;
                    rejectionReason = model.PensionRejectionReason; 
                    break;
                case DocumentCategory.MaritalStatus:
                    fileUrl = res.MaritalStatusId;
                    rejectionReason = model.MaritalStatusRejectionReason; 
                    break;
                case DocumentCategory.SpousePensionAlbania:
                    fileUrl = res.SpousePensionDocumentAlbaniaId; 
                    rejectionReason = model.SpouseRejectionReason;
                    break;
            }

            SaveFileRequest fileReq = new SaveFileRequest()
            {
                AppId = model.Id,
                RejectionReason = rejectionReason,
                IsApproved = isApproved,
                FileUrl = fileUrl
            };

            SaveFileResponse fileRes = await _gsAppService.SetWebUIContext(context).SaveFileAsync(fileReq);

            if (!fileRes._IsSuccessful)
            {
                ModelState.Clear();
                foreach (var item in fileRes.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", item.Message);
                }
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            if (!ModelState.IsValid)
            {
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            model.IsSuccesfull = true;
            return RedirectToEdit(model.Id, withModelState: false, returnUrl: model.ReturnUrl, action: "save");
        }

        private SaveApplicationRequest MapSaveApplicationRequest (ApplicationViewModel model)
        {
            List<Remark> destRemarks = new List<Remark>();
            foreach (RemarkViewModel remarkV in model.Remarks)
            {
                destRemarks.Add(remarkV.Map());
            }
            model.ApplicantFinancialInfo.PensionAmountAlbania = model.ApplicantFinancialInfo.PensionAmountAlbania?.Replace('.', ',');
            decimal.TryParse(model.ApplicantFinancialInfo.PensionAmountAlbania, out decimal PensionAmountAlbaniaDecimal);
            SaveApplicationRequest req = new SaveApplicationRequest()
            {
                Id = model.Id,
                Revision = model.Revision,
                Street = model.AddressInfo.Street,
                StreetNumber = model.AddressInfo.StreetNumber,
                Zip = model.AddressInfo.Zip,
                PostalNumber = model.AddressInfo.PostalNumber,
                Email = model.ApplicationContactInfo.Email,
                Iban = model.ApplicationContactInfo.IBAN,
                MobilePhone = model.ApplicationContactInfo.MobilePhone,
                HomePhone = model.ApplicationContactInfo.HomePhone,
                ProvidedFEKDocument = model.EDTOInfo.ProvidedFEKDocument,
                FEKDocumentName = model.EDTOInfo.FEKDocumentName,
                MaritalStatusDocumentName = model.ApplicantPersonalInfo.MaritalStatusDocumentName,
                PensionDocumentAlbaniaName = model.PensionDocumentAlbaniaName,
                SpousePensDocumentName = model.SpousePensDocumentName,
                MaritalStatus = model.ApplicantPersonalInfo.MaritalStatus ?? MaritalStatus.Unknown,
                UnknownMaritalStatus = model.ApplicantPersonalInfo.UnknownMaritalStatus,
                PensionAmount = model.ApplicantFinancialInfo.PensionAmount,
                PensionFromAlbania = model.ApplicantFinancialInfo.PensionFromAlbania,
                PensionAmountAlbania = PensionAmountAlbaniaDecimal > 0 ? (decimal?)PensionAmountAlbaniaDecimal : null,
                ApplicantCurrency = model.ApplicantFinancialInfo.Currency,
                PensionStartDateAlbania = model.ApplicantFinancialInfo.PensionStartDateAlbania != null ? (DateTime?)DateTime.ParseExact(model.ApplicantFinancialInfo.PensionStartDateAlbania, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null,
                IsPensionAlbaniaDocumentApproved = model.IsPensionAlbaniaDocumentApproved,
                IsSpousePensionDocumentApproved = model.IsSpousePensionDocumentApproved,
                IsMaritalStatusDocumentApproved = model.IsMaritalStatusDocumentApproved,
                IsFEKDocumentApproved = model.IsFEKDocumentApproved,
                DeclarationLaw1599 = model.DeclarationLaw1599,
                ChangedRemarks = destRemarks,
                RemoteHostIP = NEECurrentUserContext.GetRemoteHostIPs().First(),
                IsModelStateValid = ModelState.IsValid
            };

            if (model.SpouseInfo != null)
            {
                model.SpouseInfo.PensionAmountAlbania?.Replace('.', ',');
                decimal.TryParse(model.SpouseInfo.PensionAmountAlbania, out decimal SpousePensionAmountAlbaniaDecimal);
                req.SpousePensionFromAlbania = model.SpouseInfo.PensionFromAlbania;
                req.SpousePensionAmountAlbania = SpousePensionAmountAlbaniaDecimal > 0 ? (decimal?)SpousePensionAmountAlbaniaDecimal : null;
                req.SpouseCurrency = model.SpouseInfo.Currency;
                req.SpousePensionStartDateAlbania = model.SpouseInfo.PensionStartDateAlbania != null ? (DateTime?)DateTime.ParseExact(model.SpouseInfo.PensionStartDateAlbania, "dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
                req.IsSpousePensionDocumentApproved = model.IsSpousePensionDocumentApproved;
            }
            req.ReturnApplication = true;
            return req;
        }


        private async Task<ActionResult> ValidateSaveFinalSubmitHandle(ApplicationViewModel model)
        {
            WebUIContext context = new WebUIContext()
            {
                WebUIAction = WebUIAction.ValidateSaveFinalSubmitApplication
            };

            var req = MapSaveApplicationRequest(model);

            SaveApplicationResponse resSave = await _gsAppService.SetWebUIContext(context).SaveApplicationAsync(req);

            if (!resSave._IsSuccessful)
            {
                ModelState.Clear();
                foreach (var item in resSave.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", item.Message);
                }
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            if (!ModelState.IsValid)
            {
                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            FinalSubmitApplicationRequest reqFinal = new FinalSubmitApplicationRequest()
            {
                Id = model.Id,
                Revision = model.Revision,
                FEKDocumentRequired = model.EDTOInfo.EDTONumber == null,
                MaritalStatusDocumentRequired = model.ApplicantPersonalInfo.UnknownMaritalStatus == true,
                PensionAlbaniaDocumentRequired = true,
                SpouseAlbaniaDocumentRequired = model.ApplicantPersonalInfo.HasSpouse,
                IsModelStateValid = ModelState.IsValid,
                RemoteHostIP = NEECurrentUserContext.GetRemoteHostIPs().First()// pass the Remote Host IP for the IBAN Validation WS
            };

            FinalSubmitApplicationResponse resFinal = await _gsAppService.SetWebUIContext(context).FinalSubmitApplicationAsync(reqFinal);

            if (!resFinal._IsSuccessful)
            {
                ModelState.Clear();
                foreach (var item in resFinal.UIDisplayedErrors)
                {
                    ModelState.AddModelError("", item.Message);
                }

                if (!resFinal.UIDisplayedErrors.Any())
                {
                    ModelState.AddModelError("", "Υπήρξε κάποιο πρόβλημα με την αποθήκευση των αλλαγών");
                }

                return RedirectToEdit(model.Id, withModelState: true, returnUrl: model.ReturnUrl);
            }

            return RedirectToAction(nameof(ApplicationSubmitted), new { id = model.Id, returnUrl = model.ReturnUrl });
        }
    }
}