@echo off

If '%1' == '' Goto DoAll
If '%1' == '-B' Goto DoAll
Goto DoOne

:DoAll

	Set	CleanSolution_Cleaned=0

	If not '%1' == '-B' Echo Clean Solution?
	If not '%1' == '-B' pause


	Rem ==================== 0. Other ====================

	Call	%0	"0. Other\Documentation\obj"
	Call	%0	"0. Other\Documentation\bin"

	Call	%0	"0. Other\VariousFiles\obj"
	Call	%0	"0. Other\VariousFiles\bin"

	Rem ==================== 1. General ====================

	Call	%0	"1. General\GMI.Core\obj"
	Call	%0	"1. General\GMI.Core\bin"

	Call	%0	"1. General\GMI.Core.PerformanceTests\obj"
	Call	%0	"1. General\GMI.Core.PerformanceTests\bin"

	Call	%0	"1. General\GMI.Core.Tests\obj"
	Call	%0	"1. General\GMI.Core.Tests\bin"

	Call	%0	"1. General\GMI.DAL\obj"
	Call	%0	"1. General\GMI.DAL\bin"

	Call	%0	"1. General\GMI.Database\obj"
	Call	%0	"1. General\GMI.Database\bin"

	Call	%0	"1. General\GMI.Logging\obj"
	Call	%0	"1. General\GMI.Logging\bin"

	Call	%0	"1. General\GMI.PerfCounters\obj"
	Call	%0	"1. General\GMI.PerfCounters\bin"

	Call	%0	"1. General\GMI.PerfCounters.Setup\obj"
	Call	%0	"1. General\GMI.PerfCounters.Setup\bin"

	Call	%0	"1. General\GMI.RefDB.DAL\obj"
	Call	%0	"1. General\GMI.RefDB.DAL\bin"

	Rem ==================== 3. Idika ====================

	Call	%0	"3. Idika\Idika.ApplicationInsights\obj"
	Call	%0	"3. Idika\Idika.ApplicationInsights\bin"

	Call	%0	"3. Idika\Idika.Feeds\obj"
	Call	%0	"3. Idika\Idika.Feeds\bin"

	Call	%0	"3. Idika\Idika.Notifications\obj"
	Call	%0	"3. Idika\Idika.Notifications\bin"

	Call	%0	"3. Idika\Idika.Notifications.GMI\obj"
	Call	%0	"3. Idika\Idika.Notifications.GMI\bin"

	Call	%0	"3. Idika\Idika.Notifications.Service\obj"
	Call	%0	"3. Idika\Idika.Notifications.Service\bin"

	Call	%0	"3. Idika\Idika.Notifications.Tests\obj"
	Call	%0	"3. Idika\Idika.Notifications.Tests\bin"

	Call	%0	"3. Idika\Idika.Notifications.Tool\obj"
	Call	%0	"3. Idika\Idika.Notifications.Tool\bin"

	Call	%0	"3. Idika\Idika.SMS\obj"
	Call	%0	"3. Idika\Idika.SMS\bin"

	Call	%0	"3. Idika\Idika.SMS.SmsBox\obj"
	Call	%0	"3. Idika\Idika.SMS.SmsBox\bin"

	Call	%0	"3. Idika\Idika.SMS.Web2SMS\obj"
	Call	%0	"3. Idika\Idika.SMS.Web2SMS\bin"

	Rem ==================== 4. XS ====================

	Call	%0	"4. XS\XS.AllServices.Mock\obj"
	Call	%0	"4. XS\XS.AllServices.Mock\bin"

	Call	%0	"4. XS\XS.Contract\obj"
	Call	%0	"4. XS\XS.Contract\bin"

	Call	%0	"4. XS\XS.GRNet.Contract\obj"
	Call	%0	"4. XS\XS.GRNet.Contract\bin"

	Call	%0	"4. XS\XS.GRNet.Service\obj"
	Call	%0	"4. XS\XS.GRNet.Service\bin"

	Call	%0	"4. XS\XS.GRNet.Service.Tests\obj"
	Call	%0	"4. XS\XS.GRNet.Service.Tests\bin"

	Call	%0	"4. XS\XS.DEDDIE.Contract\obj"
	Call	%0	"4. XS\XS.DEDDIE.Contract\bin"

	Call	%0	"4. XS\XS.DEDDIE.Service\obj"
	Call	%0	"4. XS\XS.DEDDIE.Service\bin"

	Call	%0	"4. XS\XS.DEDDIE.Service.Tests\obj"
	Call	%0	"4. XS\XS.DEDDIE.Service.Tests\bin"

	Call	%0	"4. XS\XS.GSIS.Contract\obj"
	Call	%0	"4. XS\XS.GSIS.Contract\bin"

	Call	%0	"4. XS\XS.GSIS.Service\obj"
	Call	%0	"4. XS\XS.GSIS.Service\bin"

	Call	%0	"4. XS\XS.GSIS.Service.Tests\obj"
	Call	%0	"4. XS\XS.GSIS.Service.Tests\bin"

	Call	%0	"4. XS\XS.IDIKA.Contract\obj"
	Call	%0	"4. XS\XS.IDIKA.Contract\bin"

	Call	%0	"4. XS\XS.IDIKA.Service\obj"
	Call	%0	"4. XS\XS.IDIKA.Service\bin"

	Call	%0	"4. XS\XS.IDIKA.Service.Tests\obj"
	Call	%0	"4. XS\XS.IDIKA.Service.Tests\bin"

	Call	%0	"4. XS\XS.IKA.Contract\obj"
	Call	%0	"4. XS\XS.IKA.Contract\bin"

	Call	%0	"4. XS\XS.IKA.Service\obj"
	Call	%0	"4. XS\XS.IKA.Service\bin"

	Call	%0	"4. XS\XS.IKA.Service.Bridge\obj"
	Call	%0	"4. XS\XS.IKA.Service.Bridge\bin"

	Call	%0	"4. XS\XS.IKA.Service.Tests\obj"
	Call	%0	"4. XS\XS.IKA.Service.Tests\bin"

	Call	%0	"4. XS\XS.MMP.Contract\obj"
	Call	%0	"4. XS\XS.MMP.Contract\bin"

	Call	%0	"4. XS\XS.MMP.Service\obj"
	Call	%0	"4. XS\XS.MMP.Service\bin"

	Call	%0	"4. XS\XS.MySchool.Contract\obj"
	Call	%0	"4. XS\XS.MySchool.Contract\bin"

	Call	%0	"4. XS\XS.MySchool.Service\obj"
	Call	%0	"4. XS\XS.MySchool.Service\bin"

	Call	%0	"4. XS\XS.MySchool.Service.Tests\obj"
	Call	%0	"4. XS\XS.MySchool.Service.Tests\bin"

	Call	%0	"4. XS\XS.OAED.Contract\obj"
	Call	%0	"4. XS\XS.OAED.Contract\bin"

	Call	%0	"4. XS\XS.OAED.Service\obj"
	Call	%0	"4. XS\XS.OAED.Service\bin"

	Call	%0	"4. XS\XS.OAED.Service.Tests\obj"
	Call	%0	"4. XS\XS.OAED.Service.Tests\bin"

	Call	%0	"4. XS\XS.WebApi\obj"
	Call	%0	"4. XS\XS.WebApi\bin"

	Rem ==================== 5. GS ====================

	Call	%0	"5. GS\GS.Contract\obj"
	Call	%0	"5. GS\GS.Contract\bin"

	Call	%0	"5. GS\GS.Service\obj"
	Call	%0	"5. GS\GS.Service\bin"

	Call	%0	"5. GS\GS.Service.Tests\obj"
	Call	%0	"5. GS\GS.Service.Tests\bin"

	Call	%0	"5. GS\GS.WebApi\obj"
	Call	%0	"5. GS\GS.WebApi\bin"

	Rem ==================== 6. TestApps ====================

	Call	%0	"6. TestApps\GMI.GrayListUpdater\obj"
	Call	%0	"6. TestApps\GMI.GrayListUpdater\bin"

	Call	%0	"6. TestApps\GMI_EYDAP_ExternalWebApiClientDemo1\obj"
	Call	%0	"6. TestApps\GMI_EYDAP_ExternalWebApiClientDemo1\bin"

	Rem ==================== 7. GMI ====================
	
	Call	%0	"7. GMI\GMI.Audit\obj"
	Call	%0	"7. GMI\GMI.Audit\bin"

	Call	%0	"7. GMI\GMI.Audit.CLI\obj"
	Call	%0	"7. GMI\GMI.Audit.CLI\bin"

	Call	%0	"7. GMI\GMI.Audit.ReferenceData\obj"
	Call	%0	"7. GMI\GMI.Audit.ReferenceData\bin"

	Call	%0	"7. GMI\GMI.Audit.Rules\obj"
	Call	%0	"7. GMI\GMI.Audit.Rules\bin"

	Call	%0	"7. GMI\GMI.Audit.Storage\obj"
	Call	%0	"7. GMI\GMI.Audit.Storage\bin"

	Call	%0	"7. GMI\GMI.Audit.Tests\obj"
	Call	%0	"7. GMI\GMI.Audit.Tests\bin"

	Call	%0	"7. GMI\GMI.Audit.Web\obj"
	Call	%0	"7. GMI\GMI.Audit.Web\bin"

	Call	%0	"7. GMI\GMI.DW2.Scheduling\obj"
	Call	%0	"7. GMI\GMI.DW2.Scheduling\bin"

	Call	%0	"7. GMI\GMI.Notifications\obj"
	Call	%0	"7. GMI\GMI.Notifications\bin"

	Call	%0	"7. GMI\GMI.Web.PerformanceTests\obj"
	Call	%0	"7. GMI\GMI.Web.PerformanceTests\bin"

	Call	%0	"7. GMI\GMI.Web.Tests\obj"
	Call	%0	"7. GMI\GMI.Web.Tests\bin"

	Rem ==================== 8. GMI.DW ====================

	Call	%0	"8. GMI.DW\GMI.Data.Common\obj"
	Call	%0	"8. GMI.DW\GMI.Data.Common\bin"

	Call	%0	"8. GMI.DW\GMI.DW.Application\obj"
	Call	%0	"8. GMI.DW\GMI.DW.Application\bin"

	Call	%0	"8. GMI.DW\GMI.DW.Data\obj"
	Call	%0	"8. GMI.DW\GMI.DW.Data\bin"

	Call	%0	"8. GMI.DW\GMI.DW.Services\obj"
	Call	%0	"8. GMI.DW\GMI.DW.Services\bin"

	Call	%0	"8. GMI.DW\GMI.DW.Types\obj"
	Call	%0	"8. GMI.DW\GMI.DW.Types\bin"

	Call	%0	"8. GMI.DW\GMI.DW.UnitTests\obj"
	Call	%0	"8. GMI.DW\GMI.DW.UnitTests\bin"

	Rem ==================== 9. GMI.Payments ====================

	Call	%0	"9. GMI.Payments\GMI.Payments.Controllers\obj"
	Call	%0	"9. GMI.Payments\GMI.Payments.Controllers\bin"

	Call	%0	"9. GMI.Payments\GMI.Payments.Data\obj"
	Call	%0	"9. GMI.Payments\GMI.Payments.Data\bin"

	Call	%0	"9. GMI.Payments\GMI.Payments.Interfaces\obj"
	Call	%0	"9. GMI.Payments\GMI.Payments.Interfaces\bin"

	Call	%0	"9. GMI.Payments\GMI.Payments.Services\obj"
	Call	%0	"9. GMI.Payments\GMI.Payments.Services\bin"

	Call	%0	"9. GMI.Payments\GMI.Payments.Types\obj"
	Call	%0	"9. GMI.Payments\GMI.Payments.Types\bin"

	Call	%0	"9. GMI.Payments\GMI.Payments.UnitTests\obj"
	Call	%0	"9. GMI.Payments\GMI.Payments.UnitTests\bin"

	Rem ==================== 10. GMI.Report ====================

	Call	%0	"10. GMI.Report\GMI.Report.Controllers\obj"
	Call	%0	"10. GMI.Report\GMI.Report.Controllers\bin"

	Call	%0	"10. GMI.Report\GMI.Report.Data\obj"
	Call	%0	"10. GMI.Report\GMI.Report.Data\bin"

	Call	%0	"10. GMI.Report\GMI.Report.Interfaces\obj"
	Call	%0	"10. GMI.Report\GMI.Report.Interfaces\bin"

	Call	%0	"10. GMI.Report\GMI.Report.SchemaCreator\obj"
	Call	%0	"10. GMI.Report\GMI.Report.SchemaCreator\bin"

	Call	%0	"10. GMI.Report\GMI.Report.Services\obj"
	Call	%0	"10. GMI.Report\GMI.Report.Services\bin"

	Call	%0	"10. GMI.Report\GMI.Report.Types\obj"
	Call	%0	"10. GMI.Report\GMI.Report.Types\bin"

	Call	%0	"10. GMI.Report\GMI.Report.UnitTests\obj"
	Call	%0	"10. GMI.Report\GMI.Report.UnitTests\bin"

	Rem ==================== Root ====================

	Call	%0	"GMI.Importer\obj"
	Call	%0	"GMI.Importer\bin"

	Call	%0	"GMI.ApplicantChecker\obj"
	Call	%0	"GMI.ApplicantChecker\bin"

	Call	%0	"GMI.Pub.Web\obj"
	Call	%0	"GMI.Pub.Web\bin"

	Call	%0	"GMI.Web\obj"
	Call	%0	"GMI.Web\bin"


	Rem Goto SkipFull

	Call	%0	"GMI.Reporting\obj"
	Call	%0	"GMI.Reporting\bin"
	Call	%0	"GMI.Reporting\node_modules"

	Call	%0	"(deploy)"
	Call	%0	"TestResults"
	Call	%0	"packages"

:SkipFull

	Rem ==================== End ====================

	If     '%1' == '-B' Goto Exit

	Echo . Clean Solution Complete!
	Echo .
	If     '%CleanSolution_Cleaned%' == '0' Echo .	--------------------
	If     '%CleanSolution_Cleaned%' == '0' Echo .	  OK: all-skipped!
	If     '%CleanSolution_Cleaned%' == '0' Echo .	--------------------
	If     '%CleanSolution_Cleaned%' == '1' Echo .	================================================
	If     '%CleanSolution_Cleaned%' == '1' Echo .	PLEASE RE-RUN (At least one cleaned) - RE-RUN !!
	If     '%CleanSolution_Cleaned%' == '1' Echo .	================================================
	Echo .
	pause

	Goto Exit

:DoOne

	If Not	Exist %1 	Echo .      skip: %1
	If 	Exist %1	Echo .  CLEANING: %1 ...
	If 	Exist %1	Set	CleanSolution_Cleaned=1
	If 	Exist %1 	rmdir /s /q %1
	Goto Exit

:Exit

