; Inno Setup install script for Ecopath with Ecosim
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
; #include <C:\Program Files (x86)\Inno Download Plugin\idp.iss>

; New in EwE 6.7: there will be no distinction between the regular and pro installer
; Adjust #defines in this section to select which components to include in an installer
#ifndef Compile64Bit
  #define Compile64Bit "0"                    ; set to 0 to compile 32 bit
#endif

#define CodeSigning 1                      ; set to 0 to disable code signing

; Optional features
#define RobertsBank 0
#define RandomizeMPAs 0
#define ExcludeDeadCells 0

; Automated build will provide file version as a command line parameter
; /DFileVersion=6.6.{minor release no}.{build no}
#ifndef FileVersion
  #define FileVersion "6.7.0.19540"
#endif
; VersionInfoVersion={#FileVersion}

#if Compile64Bit == "0"
  #define MyAppVersion FileVersion + "_32-bit"
  #define DefSrc "Sources\ScientificInterface\bin\x86\Release\net48"
#else
  #define MyAppVersion FileVersion + "_64-bit"
  #define DefSrc "Sources\ScientificInterface\bin\x64\Release\net48"
#endif

; Standard stuff
#define MyAppName "Ecopath with Ecosim"
#define MyAppExeName "ewe6.exe"
#define MyAppPublisher "Ecopath International Initiative"
#define DefRoot "..\"
#define DefDB "Database"

[Setup]
; Code signing, fundamental for distributing installers and executables:
; - EII 2020 .pfx code signing certificate expired 2 Dec 2023. 
; - EII 2024 .cer code signing certificate will expire 10 Jan 2027
;
; In Inno Setup UI, define Sign tool 'codesign' depending on code cert type:
;   .pfx:   "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\signtool.exe" sign /a /f "D:\Cloud\Dropbox\EII_cert.pfx" /p muahaha /t http://timestamp.comodoca.com/authenticode $f
;   .cer    "C:\Program Files (x86)\Windows Kits\10\bin\10.0.20348.0\x64\signtool.exe" sign  /f ".\EII_cert_2024.cer" /csp "eToken Base Cryptographic Provider"  /k  "[SafeNet Token JC 0{{muahaha}}]=Sectigo_20240110133725" /t "http://timestamp.sectigo.com" /fd sha256 $f
; - Replace "muahaha" with the certificate password
AllowNoIcons=True
AlwaysShowGroupOnReadyPage=True
AlwaysShowDirOnReadyPage=True
AppName={#MyAppName}
AppCopyright={#MyAppPublisher}
AppId={{113d96bb-5c02-464c-a936-0813ce272e03}}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://ecopathinternational.org
AppSupportURL=mailto:support@ecopath.org
AppVersion={#MyAppVersion}
DefaultDirName={commonpf}\{#MyAppName} {#MyAppVersion}
DefaultGroupName={#MyAppName}\Release {#MyAppVersion}
MinVersion=0,6.1sp1
SetupIconFile=Ecopath_install.ico
#if CodeSigning == 1
; SignTool=codesign /d $q{#MyAppName}$q $f
#else
; NOP
#endif
UninstallDisplayIcon={app}\{#MyAppName}
WizardImageFile=EwE5Logo.bmp
WizardSmallImageFile=EwE6Header.bmp
WizardImageStretch=False
WizardStyle=modern
WizardResizable=True
WizardSizePercent=120,120
SolidCompression=True
Compression=lzma2/max 
UninstallDisplayName={#MyAppName} {#MyAppVersion}
ChangesAssociations=True

#if Compile64Bit == "1"
  ; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
  ; done in "64-bit mode" on x64, meaning it should use the native
  ; 64-bit Program Files directory and the 64-bit view of the registry.
  ; On all other architectures it will install in "32-bit mode".
  ArchitecturesInstallIn64BitMode=x64compatible
  ; Note: We don't set ProcessorsAllowed because we want this
  ; installation to run on all architectures (including Itanium,
  ; since it's capable of running 32-bit code too).
#endif
UsePreviousAppDir=False
TimeStampsInUTC=True
VersionInfoCompany={#MyAppPublisher}
VersionInfoCopyright=(c) {#MyAppPublisher}
OutputBaseFilename=ewe_{#MyAppVersion}_setup

[Dirs]
Name: "{app}\Includes\LPSolve\"
Name: "{app}\Includes\LPSolve\win32\"
Name: "{app}\Includes\LPSolve\win64\"
Name: "{app}\Resources\"
Name: "{app}\Tools\"
Name: "{app}\UserGuide\"
Name: "{app}\Plugins\"
Name: "{app}\Includes\GDAL\"
Name: "{app}\Includes\GDAL\win32\"
Name: "{app}\Includes\GDAL\win32\gdalplugins\"
Name: "{app}\Includes\GDAL\win64\"
Name: "{app}\Includes\GDAL\win64\gdalplugins\"
Name: "{app}\Includes\LPSolve\"
Name: "{app}\Includes\LPSolve\win32\"
Name: "{app}\Includes\LPSolve\win64\"

[Files]
Source: "..\LICENSE.txt"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwEUtils.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwEPlugin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwECore.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\ZedGraph.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\WeifenLuo.WinFormsUI.Docking.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EPPlus.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\SourceLibrary.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\SourceGrid2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\ScientificInterfaceShared.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwE6.exe"; DestDir: "{app}"; DestName: "{#MyAppExeName}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\TreeksLicensingLibrary2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwELicense.dll"; DestDir: "{app}"; Flags: ignoreversion
; Source: "{#DefRoot}{#DefSrc}\Interop.JRO.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Ionic.Zip.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Microsoft.GLEE.dll"; DestDir: "{app}"; Flags: ignoreversion
; Source: "{#DefRoot}{#DefSrc}\Microsoft.Office.Interop.Access.Dao.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Includes\LPSolve\win32\lpsolve55.dll"; DestDir: "{app}\Includes\LPSolve\win32\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Includes\LPSolve\win64\lpsolve55.dll"; DestDir: "{app}\Includes\LPSolve\win64\"; Flags: ignoreversion

; - Strange For some reason the Json library is not included with the .net version
; - Put it in the app directory
; Source: "{#DefRoot}{#DefSrc}\System.Text.Json.dll"; DestDir: "{app}"; Flags: ignoreversion

; - User guide
Source: "{#DefRoot}{#DefSrc}\UserGuide\EwE6_userguide.chm"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: userguide
; - Tools
Source: "{#DefRoot}{#DefSrc}\Tools\code_for_plotting_dirichlets.R"; DestDir: "{app}\Tools\"; Flags: ignoreversion

; - Plugins --
; Analysis
Source: "{#DefRoot}{#DefSrc}\EwENetworkAnalysisPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\na
Source: "{#DefRoot}{#DefSrc}\EwEPrebalPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\prebal
Source: "{#DefRoot}{#DefSrc}\UserGuide\Link - 2010 - Adding rigor to ecological network models by evalu.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\analysis\prebal
Source: "{#DefRoot}{#DefSrc}\EwEValueChainv2Plugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\valuechain
Source: "{#DefRoot}{#DefSrc}\UserGuide\ChristensenValueChainMS.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\analysis\valuechain
Source: "{#DefRoot}{#DefSrc}\EwEEcologicalIndicatorsPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\ecolind
Source: "{#DefRoot}{#DefSrc}\EwEEcoTrophPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\ecotroph
Source: "{#DefRoot}{#DefSrc}\EwEFleetTradeoffsPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\fleetTO

; Input
Source: "{#DefRoot}{#DefSrc}\EwEWoRMSPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\input\worms
Source: "{#DefRoot}{#DefSrc}\EwEMergeSplitGroupsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\mergegroups
Source: "{#DefRoot}{#DefSrc}\EwEAquamapsEnvDataImporterPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\input\aquamaps
Source: "{#DefRoot}{#DefSrc}\EwEImportExportLayerDefinitionsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\layerimportexport
Source: "{#DefRoot}{#DefSrc}\EwEEcoengineersPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\ecoengineers
Source: "{#DefRoot}{#DefSrc}\UserGuide\Ecoengineer user guide.pdf"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\ecoengineers
Source: "{#DefRoot}{#DefSrc}\EwEMPADynamicsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\mpadynamics
Source: "{#DefRoot}{#DefSrc}\EwEBiomassEmitterPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\biomassemitter
Source: "{#DefRoot}{#DefSrc}\UserGuide\Biomass-emitter-guide.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\input\biomassemitter
Source: "{#DefRoot}{#DefSrc}\EwEImportDietsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\szumadiets
Source: "{#DefRoot}{#DefSrc}\EwEEcotracerEnvDriverPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\ecotracer
#if RandomizeMPAs == 1
Source: "{#DefRoot}{#DefSrc}\EwERandomizeMPAPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\randomizeMPAs
#endif
#if ExcludeDeadCells == 1
Source: "{#DefRoot}{#DefSrc}\EwEEcospaceExcludeIsolatedCellsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\excldeadcells
#endif

; Output
Source: "{#DefRoot}{#DefSrc}\EwEResultsExtractorPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\output\resultextractor
Source: "{#DefRoot}{#DefSrc}\UserGuide\ResultsExtractorPlug.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\output\resultextractor
Source: "{#DefRoot}{#DefSrc}\EwEModelFromEcosimPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\output\modelfromsim
Source: "{#DefRoot}{#DefSrc}\UserGuide\EwE model from time step.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\output\modelfromsim
Source: "{#DefRoot}{#DefSrc}\EwETransectExtractionPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\output\transects
Source: "{#DefRoot}{#DefSrc}\EwEDietMatrixToNetworkD3RPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\output\networkd3
Source: "{#DefRoot}{#DefSrc}\EwEIBMAgeStructureResultsWriterPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\output\IBMwriter
Source: "{#DefRoot}{#DefSrc}\EwEenaRPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\output\enaR

; Automation
Source: "{#DefRoot}{#DefSrc}\EwEMultiSimPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\automation\multisim
Source: "{#DefRoot}{#DefSrc}\UserGuide\EwEMultiSimPlugin.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\automation\multisim
Source: "{#DefRoot}{#DefSrc}\EwEStepwiseFittingPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\automation\stepwisef
Source: "{#DefRoot}{#DefSrc}\EwEMSEPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\automation\mse
Source: "{#DefRoot}{#DefSrc}\LumenWorks.Framework.IO.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: plugin\automation\mse
Source: "{#DefRoot}{#DefSrc}\Troschuetz.Random.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: plugin\automation\mse
Source: "{#DefRoot}{#DefSrc}\EwEEcoSamplerPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\automation\sampler
Source: "{#DefRoot}{#DefSrc}\UserGuide\EcoSampler-user-manual.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\automation\sampler

; UI
Source: "{#DefRoot}{#DefSrc}\EwERemarksPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\ui\remarks
Source: "{#DefRoot}{#DefSrc}\EwEShapeGridPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\ui\shapegrid
Source: "{#DefRoot}{#DefSrc}\EwEWindowsIntegrationPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\ui\winintegration

; Pro
Source: "{#DefRoot}{#DefSrc}\EwEEcospaceSpinupPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spinup
Source: "{#DefRoot}{#DefSrc}\EwESpatialAssetsPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\pro\spattemp
; -- Source: "{#DefRoot}{#DefSrc}\DotSpatial.Analysis.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Controls.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Data.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Data.Forms.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Extensions.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Modeling.Forms.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
; -- Source: "{#DefRoot}{#DefSrc}\DotSpatial.Positioning.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Projections.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Serialization.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Symbology.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Symbology.Forms.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
; -- Source: "{#DefRoot}{#DefSrc}\DotSpatial.Tools.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Topology.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\TreeksLicensingLibrary2.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\pro\spattemp
Source: "{#DefRoot}{#DefSrc}\EwEMSPLinkPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\projects\msptools
Source: "{#DefRoot}{#DefSrc}\EwEMSPPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\projects\msptools



; -- RBT --
#if RobertsBank == 1
Source: "{#DefRoot}{#DefSrc}\EwEDepthChangePlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\robertsbank
Source: "{#DefRoot}{#DefSrc}\EwEEcospaceMonteCarloPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\robertsbank
#endif

; -- SAMPLE DATABASES --
Source: "{#DefRoot}{#DefDB}\Anchovy Bay Spatial.ewemdb"; DestDir: "{userdocs}\EwE sample databases"; Flags: ignoreversion; Components: databases

[Components]
Name: "userguide"; Description: "EwE user guide (2008)"; Types: full custom
Name: "databases"; Description: "Sample EwE models"; Types: full custom
Name: "plugin"; Description: "Plug-ins"; Types: full custom
Name: "plugin\analysis"; Description: "Analysis"; Types: full custom
Name: "plugin\analysis\ecolind"; Description: "Ecological Indicators"; Types: full
Name: "plugin\analysis\ecotroph"; Description: "EcoTroph"; Types: custom full
Name: "plugin\analysis\na"; Description: "Network Analysis"; Types: compact custom full
Name: "plugin\analysis\prebal"; Description: "Pre-balance diagnostics"; Types: full custom
Name: "plugin\analysis\valuechain"; Description: "Value chain"; Types: full
Name: "plugin\analysis\fleetTO"; Description: "Fleet trade-offs"; Types: full
Name: "plugin\input"; Description: "Data retrieval"; Types: full custom
Name: "plugin\input\worms"; Description: "WoRMS taxonomy search"; Types: full
Name: "plugin\input\mergegroups"; Description: "Merge groups"; Types: full
Name: "plugin\input\mpadynamics"; Description: "MPA dynamics"; Types: full
Name: "plugin\input\aquamaps"; Description: "Aquamaps functional response importer"; Types: full
Name: "plugin\input\ecoengineers"; Description: "Eco-engineer dynamics"; Types: full
Name: "plugin\input\szumadiets"; Description: "Diet import utility"; Types: full
Name: "plugin\input\layerimportexport"; Description: "Ecospace layer style import and export"; Types: full
Name: "plugin\input\biomassemitter"; Description: "Biomass emitter"; Types: full
Name: "plugin\input\ecotracer"; Description: "Ecotracer impacts"; Types: full
#if ExcludeDeadCells == 1
Name: "plugin\input\excldeadcells"; Description: "Exclude isolated cells"; Types: full
#endif
#if RandomizeMPAs == 1
Name: "plugin\input\randomizeMPAs"; Description: "Randomize MPA cells"; Types: full
#endif
Name: "plugin\output"; Description: "Data export"; Types: full
Name: "plugin\output\modelfromsim"; Description: "Ecopath model from Ecosim"; Types: full
Name: "plugin\output\resultextractor"; Description: "Results extractor"; Types: full
Name: "plugin\output\transects"; Description: "Transects extraction"; Types: full
Name: "plugin\output\networkD3"; Description: "Export diet matrix to NetworkD3"; Types: full
Name: "plugin\output\IBMwriter"; Description: "Ecospace IBM age structure autosave"; Types: full
Name: "plugin\output\enaR"; Description: "Ecospace enaR"; Types: full
Name: "plugin\automation"; Description: "Automation"; Types: full custom
Name: "plugin\automation\multisim"; Description: "Multi-Sim"; Types: custom full
Name: "plugin\automation\stepwisef"; Description: "Stepwise Fitting"; Types: full
Name: "plugin\automation\mse"; Description: "Cefas MSE"; Types: custom full
Name: "plugin\automation\sampler"; Description: "Ecosampler"; Types: full
Name: "plugin\ui"; Description: "Usability"; Types: full custom
Name: "plugin\ui\remarks"; Description: "Remarks collector"; Types: full custom
Name: "plugin\ui\shapegrid"; Description: "Shape grids"; Types: full custom
Name: "plugin\ui\winintegration"; Description: "Windows integration"; Types: full
Name: "plugin\pro"; Description: "Professional features"; Types: full custom
Name: "plugin\pro\spattemp"; Description: "Spatial-temporal GIS data exchange framework"; Types: full
Name: "plugin\pro\spinup"; Description: "Ecospace spin-up"; Types: full
Name: "plugin\projects"; Description: "Project specific plugins"; Types: full custom
Name: "plugin\projects\msptools"; Description: "MSP Challenge tools"; Types: full
#if RobertsBank == 1
Name: "plugin\robertsbank"; Description: "Roberts Bank utilities"; Types: full custom
#endif

[Tasks]
Name: "desktopicon"; Description: "Add desktop icon"
Name: "quicklaunchicon"; Description: "Add quick launch icon"
Name: "associatefiles"; Description: "Open EwE models and web links in this version by default"; GroupDescription: "File associations"

[Icons]
Name: "{userdesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon
Name: "{group}\{#MyAppName} {#MyAppVersion}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"; IconFilename:  "{app}\{#MyAppExeName}"; 
Name: "{group}\User guide"; Filename: "{app}\UserGuide\EwE6_userguide.chm"; WorkingDir: "{app}\UserGuide"; IconFilename: "{app}\UserGuide\EwE6_userguide.chm"; 
Name: "{group}\User guide"; Filename: "{app}\UserGuide\EwEMultiSimPlugin.pdf"; WorkingDir: "{app}\UserGuide"; IconFilename: "{#DefRoot}{#DefSrc}\UserGuide\EwEMultiSimPlugin.pdf"; 
Name: "{group}\Links\Ecopath website"; Filename: "http://www.ecopath.org"
Name: "{group}\Links\Ecopath on Facebook"; Filename: "http://www.facebook.com/eweconsortium"
Name: "{group}\Links\User support"; Filename: "http://www.ecopath.org/support"

[ThirdParty]
UseRelativePaths=True

[Run]
Filename: "{app}\{#MyAppExeName}"; Flags: postinstall skipifsilent; Description: "Run {#MyAppName}"

[Code]
// https://stackoverflow.com/questions/4104011/inno-setup-verify-that-net-4-0-is-installed
// https://blogs.msdn.microsoft.com/davidrickard/2015/07/17/installing-net-framework-4-5-automatically-with-inno-setup/
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//    'v4.6.1'        .NET Framework 4.6.1
//    'v4.6.2'        .NET Framework 4.6.2
//    'v4.7'          .NET Framework 4.7
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // 393297 on Windows 8.1 and older
          'v4.6.1': versionRelease := 394254; // 394271 before Win10 November Update
          'v4.6.2': versionRelease := 394802; // 394806 before Win10 Anniversary Update
          'v4.7':   versionRelease := 460798; // 460805 before Win10 Creators Update
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

procedure InitializeWizard();
begin
    if not IsDotNetDetected('v4.7', 0) then 
    begin
        // 4.0 full: https://go.microsoft.com/fwlink/?LinkId=181013
        // 4.5 full: https://go.microsoft.com/fwlink/?LinkId=225702
        // idpAddFile('http://go.microsoft.com/fwlink/?LinkId=863262', ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
        // idpDownloadAfter(wpReady);
     end
end;

procedure InstallFramework;
var
    StatusText: string;
    ResultCode: Integer;
    Installer: string;
begin

    Installer := ExpandConstant('{tmp}\NetFrameworkInstaller.exe');
    
    if FileExists(Installer) then
    begin
        try
            StatusText := WizardForm.StatusLabel.Caption;
            WizardForm.StatusLabel.Caption := 'Installing .NET Framework. This might take a few minutes...';
            WizardForm.ProgressGauge.Style := npbstMarquee;
            if not Exec(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
            begin
                MsgBox('.NET installation failed with code: ' + IntToStr(ResultCode) + '.', mbError, MB_OK);
            end;    
        finally
            WizardForm.StatusLabel.Caption := StatusText;
            WizardForm.ProgressGauge.Style := npbstNormal;
            DeleteFile(ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
        end;
    end;
end;


// Generate and return a SHA 256 checksum for a file
// Blueprint served up by ChatGPT
function GenerateSHA256Checksum(FileName: String): String;
var
  ResultCode: Integer;
  Command: String;
  OutFile: String;
begin
  // Build the command to call the batch file
  Outfile := ExtractFileDir(FileName) + '\' + ChangeFileExt(FileName, 'SHA256');
  Command := 'cmd.exe /C "GenerateChecksum.bat "' + FileName + '" "' + OutFile + '"';

  // Run the batch file
  if Exec('cmd.exe', '/C ' + Command, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if ResultCode = 0 then
      Result := 'Checksum generated successfully: ' + OutFile
    else
      Result := 'Error generating checksum. Batch file returned code: ' + IntToStr(ResultCode);
  end
  else
    Result := 'Error launching batch file.';
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
    case CurStep of
        ssPostInstall:
        begin
            if not IsDotNetDetected('v4.', 0) then
            begin
                InstallFramework();
            end;
 
        end;
    end;
end;

[Registry]
; ewefile
Root: "HKCR"; Subkey: "ewefile\"; ValueType: string; ValueData: "Ecopath with Ecosim model"; Flags: uninsdeletekey; Tasks: associatefiles
Root: "HKCR"; Subkey: "ewefile\Shell\Open\Command\"; ValueType: string; ValueData: """{app}\EwE6.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: associatefiles
Root: "HKCR"; Subkey: "ewefile\DefaultIcon\"; ValueType: string; ValueData: "{app}\EwE6.exe,0"; Flags: uninsdeletekey; Tasks: associatefiles
; ewefile types
Root: "HKCR"; Subkey: ".ewemdb\"; ValueType: string; ValueData: "ewefile"; Flags: uninsdeletekey; Tasks: associatefiles
Root: "HKCR"; Subkey: ".eweaccdb\"; ValueType: string; ValueData: "ewefile"; Flags: uninsdeletekey; Tasks: associatefiles
Root: "HKCR"; Subkey: ".eiixml\"; ValueType: string; ValueData: "ewefile"; Flags: uninsdeletekey; Tasks: associatefiles
; EcoBase URL protocol handler
Root: "HKCR"; Subkey: "ewe-ecobase\"; ValueType: string; ValueData: "URL:ewe-ecobase"; Flags: uninsdeletekey; Tasks: associatefiles
Root: "HKCR"; Subkey: "ewe-ecobase\FriendlyTypeName"; ValueType: string; ValueData: "Ecopath with Ecosim Ecobase importer"; Flags: uninsdeletekey; Tasks: associatefiles
Root: "HKCR"; Subkey: "ewe-ecobase\URL Protocol"; Flags: uninsdeletekeyifempty; Tasks: associatefiles
Root: "HKCR"; Subkey: "ewe-ecobase\DefaultIcon\"; ValueType: string; ValueData: "{app}\EwE6.exe,0"; Flags: uninsdeletekey; Tasks: associatefiles
Root: "HKCR"; Subkey: "ewe-ecobase\Shell\Open\Command\"; ValueType: string; ValueData: """{app}\EwE6.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: associatefiles
; Iexplore rendering mode for start page
; Inno setup automatically redirects to wow6432node where needed
Root: "HKLM"; Subkey: "SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"; ValueType: dword; ValueName: "{#MyAppExeName}"; ValueData: "10000"; Flags: createvalueifdoesntexist uninsdeletekey
Root: "HKCR"; Subkey: "SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"; ValueType: dword; ValueName: "{#MyAppExeName}"; ValueData: "10000"; Flags: createvalueifdoesntexist uninsdeletekey
; Misc settings
Root: "HKCU"; Subkey: "Control Panel\Desktop\AutoEndTasks"; ValueType: string; ValueData: "0"
