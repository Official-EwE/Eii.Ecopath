; Inno Setup install script for Ecopath with Ecosim
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
#include <C:\Program Files (x86)\Inno Download Plugin\idp.iss>

; New in EwE 6.7: there will be no distinction between the regular and pro installer
; Adjust #defines in this section to select which components to include in an installer
#define Compile64Bit 0

; Optional features
#define RobertsBank 0
#define EcoOcean 0
#define FISHMIP 0
#define MSPTools 0
#define RandomizeMPAs 0
#define ExcludeDeadCells 0
#define enaR 0

#if Compile64Bit == 0
  #define MyAppVersion "6.7.0 α 32-bit"
  #define DefSrc "Sources\ScientificInterface\bin\x86\Release"
#else
  #define MyAppVersion "6.7.0 α 64-bit"
  #define DefSrc "Sources\ScientificInterface\bin\x64\Release"
#endif

; Standard stuff
#define MyAppName "Ecopath with Ecosim"
#define MyAppExeName "ewe6.exe"
#define MyAppPublisher "Ecopath International Initiative"
#define DefRoot "..\..\..\"
#define DefDB "Database"

[Setup]
; Automated build will provide file version as a command line parameter
; /DFileVersion=6.6.{minor release no}.{build no}
#ifdef FileVersion
  VersionInfoVersion={#FileVersion}
#else
  VersionInfoVersion=6.7.0.17922
#endif

; In Inno Setup UI, define Sign tool 'codesign' as:
;   <full path to signtool.exe> /f <cert file> /p <password> /t <path to timestamp server> $f
;   "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22000.0\x86\signtool.exe" sign /a /f "D:\Cloud\Dropbox\EII_cert.pfx" /p <muahaha> /t http://timestamp.comodoca.com/authenticode $f
SignTool=codesign /d $q{#MyAppName}$q $f
WizardImageFile=EwE5Logo.bmp
WizardSmallImageFile=EwE6Header.bmp
WizardImageStretch=False
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppCopyright={#MyAppPublisher}
AppId={{113d96bb-5c02-464c-a936-0813ce272e03}
SetupIconFile=Ecopath_install.ico
UninstallDisplayIcon={app}\{#MyAppName}
AllowNoIcons=True
AppPublisher={#MyAppPublisher}
AppPublisherURL=http://ecopathinternational.org
AppSupportURL=mailto:support@ecopath.org
MinVersion=0,6.0sp2
DefaultDirName={pf}\{#MyAppName} {#MyAppVersion}
DefaultGroupName={#MyAppName}\Release {#MyAppVersion}
AlwaysShowGroupOnReadyPage=True
AlwaysShowDirOnReadyPage=True
SolidCompression=True
Compression=zip 
UninstallDisplayName={#MyAppName} {#MyAppVersion}
OutputBaseFilename=ewe_{#MyAppVersion}

#if Compile64Bit == 1
  ; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
  ; done in "64-bit mode" on x64, meaning it should use the native
  ; 64-bit Program Files directory and the 64-bit view of the registry.
  ; On all other architectures it will install in "32-bit mode".
  ArchitecturesInstallIn64BitMode=x64
  ; Note: We don't set ProcessorsAllowed because we want this
  ; installation to run on all architectures (including Itanium,
  ; since it's capable of running 32-bit code too).
#endif
UsePreviousAppDir=False

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
Source: "gpl-2.0.txt"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwEUtils.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwEPlugin.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwECore.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\ZedGraph.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\WeifenLuo.WinFormsUI.Docking.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EPPlus.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\SourceLibrary.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\SourceGrid2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\ScientificInterfaceShared.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwE6.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\TreeksLicensingLibrary2.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwELicense.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwENetworkAnalysis.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\na
Source: "{#DefRoot}{#DefSrc}\EwEMultiSimPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\automation\multisim
Source: "{#DefRoot}{#DefSrc}\EwEPrebalPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\prebal
Source: "{#DefRoot}{#DefSrc}\EwERemarksPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\ui\remarks
Source: "{#DefRoot}{#DefSrc}\EwEResultsExtractorPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\output\resultextractor
Source: "{#DefRoot}{#DefSrc}\EwEShapeGridPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\ui\shapegrid
Source: "{#DefRoot}{#DefSrc}\EwEStepwiseFittingPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\automation\stepwisef
Source: "{#DefRoot}{#DefSrc}\EwEValueChainPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\valuechain
Source: "{#DefRoot}{#DefSrc}\EwEWoRMSPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\input\worms
Source: "{#DefRoot}{#DefSrc}\Interop.JRO.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Ionic.Zip.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Microsoft.GLEE.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Microsoft.Office.Interop.Access.Dao.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwEAquamapsEnvDataImporterPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\input\aquamaps
Source: "{#DefRoot}{#DefSrc}\EwEEcologicalIndicatorsPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\ecolind
Source: "{#DefRoot}{#DefSrc}\EwEEcoTrophPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\analysis\ecotroph
Source: "{#DefRoot}{#DefSrc}\EwEModelFromEcosimPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\output\modelfromsim
Source: "{#DefRoot}{#DefSrc}\Includes\LPSolve\win32\lpsolve55.dll"; DestDir: "{app}\Includes\LPSolve\win32\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\Includes\LPSolve\win64\lpsolve55.dll"; DestDir: "{app}\Includes\LPSolve\win64\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\LumenWorks.Framework.IO.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: plugin\automation\mse
Source: "{#DefRoot}{#DefSrc}\Troschuetz.Random.dll"; DestDir: "{app}"; Flags: ignoreversion; Components: plugin\automation\mse
Source: "{#DefRoot}{#DefSrc}\EwEMSEPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion; Components: plugin\automation\mse
Source: "{#DefRoot}{#DefSrc}\UserGuide\ChristensenValueChainMS.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\analysis\valuechain
Source: "{#DefRoot}{#DefSrc}\UserGuide\EwE model from time step.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\UserGuide\EwE6_userguide.chm"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: userguide
Source: "{#DefRoot}{#DefSrc}\UserGuide\EwEMultiSimPlugin.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\UserGuide\Link - 2010 - Adding rigor to ecological network models by evalu.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\analysis\prebal
Source: "{#DefRoot}{#DefSrc}\UserGuide\ResultsExtractorPlug.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\output\resultextractor
Source: "{#DefRoot}{#DefSrc}\Tools\code_for_plotting_dirichlets.R"; DestDir: "{app}\Tools\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EwEEcoSamplerPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\automation\sampler
Source: "{#DefRoot}{#DefSrc}\UserGuide\EcoSampler-user-manual.pdf"; DestDir: "{app}\UserGuide\"; Flags: ignoreversion; Components: plugin\automation\sampler
Source: "{#DefRoot}{#DefSrc}\EwETransectExtractionPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\output\transects
Source: "{#DefRoot}{#DefSrc}\EwEMPADynamicsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\mpadynamics
Source: "{#DefRoot}{#DefSrc}\EwEImportExportLayerDefinitionsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\layerimportexport
Source: "{#DefRoot}{#DefSrc}\EwEMergeSplitGroupsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\mergegroups
Source: "{#DefRoot}{#DefSrc}\EwEImportDietsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\szumadiets
Source: "{#DefRoot}{#DefSrc}\EwEDietMatrixToNetworkD3RPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\output\networkd3
Source: "{#DefRoot}{#DefSrc}\EwEEcoengineersPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\analysis\ecoengineers
Source: "{#DefRoot}{#DefSrc}\UserGuide\Ecoengineer user guide.pdf"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\analysis\ecoengineers
Source: "{#DefRoot}{#DefSrc}\EwEEcotracerEnvDriverPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\analysis\ecotracer

; - PRO FEATURES --
Source: "{#DefRoot}{#DefSrc}\EwEEcospaceSpinupPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\output\spinup
Source: "{#DefRoot}{#DefSrc}\EwESpatialAssetsPlugin.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
; -- Source: "{#DefRoot}{#DefSrc}\DotSpatial.Analysis.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Controls.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Data.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Data.Forms.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Extensions.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Modeling.Forms.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
; -- Source: "{#DefRoot}{#DefSrc}\DotSpatial.Positioning.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Projections.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Serialization.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Symbology.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Symbology.Forms.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
; -- Source: "{#DefRoot}{#DefSrc}\DotSpatial.Tools.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\DotSpatial.Topology.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\TreeksLicensingLibrary2.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
#if Compile64Bit == 0
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\cairo.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\cfitsio.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\DotSpatial.Data.Rasters.GdalExtension.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\freexl.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\fribidi.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\ftgl.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdal19.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalconst_csharp.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalconst_wrap.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdal_csharp.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdal_wrap.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\geos_c.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\hdf5dll.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\iconv.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libcurl.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libeay32.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libecwj2.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libexpat.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libfcgi.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libmap.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libmysql.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libpq.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libtiff.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\libxml2.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\lti_dsdk.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\lti_lidar_dsdk.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\msvcp100.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\msvcr100.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\netcdf.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\ogr_csharp.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\ogr_wrap.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\openjpeg.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\osr_csharp.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\osr_wrap.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\pdflib.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\proj.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\spatialite.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\sqlite3.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\ssleay32.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\xerces-c_2_8.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\zlib1.dll"; DestDir: "{app}\Includes\GDAL\win32\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_BAG.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_ECW_JP2ECW.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_FITS.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_GMT.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_HDF5.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_HDF5Image.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_MrSID.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win32\gdalplugins\gdal_netCDF.dll"; DestDir: "{app}\Includes\GDAL\win32\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
#else
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\cairo.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\cfitsio.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\DotSpatial.Data.Rasters.GdalExtension.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\freexl.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\fribidi.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\ftgl.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdal19.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalconst_csharp.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalconst_wrap.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdal_csharp.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdal_wrap.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\geos_c.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\hdf5dll.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\iconv.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libcurl.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libeay32.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libecwj2.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libexpat.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libfcgi.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libmap.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libmysql.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libpq.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libtiff.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\libxml2.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\lti_dsdk.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\lti_lidar_dsdk.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\msvcp100.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\msvcr100.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\netcdf.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\ogr_csharp.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\ogr_wrap.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\openjpeg.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\osr_csharp.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\osr_wrap.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\pdflib.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\proj.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\spatialite.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\sqlite3.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\ssleay32.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\xerces-c_2_8.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\zlib1.dll"; DestDir: "{app}\Includes\GDAL\win64\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_BAG.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_ECW_JP2ECW.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_FITS.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_GMT.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_HDF5.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_HDF5Image.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_MrSID.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
Source: "{#DefRoot}{#DefSrc}\Includes\GDAL\win64\gdalplugins\gdal_netCDF.dll"; DestDir: "{app}\Includes\GDAL\win64\gdalplugins\"; Flags: ignoreversion; Components: plugin\input\spattemp
#endif

; -- ExcludeDeadCells --
#if ExcludeDeadCells == 1
Source: "{#DefRoot}{#DefSrc}\EwEEcospaceExcludeIsolatedCellsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\excldeadcells
#endif

; -- RandomizeMPAs --
#if RandomizeMPAs == 1
Source: "{#DefRoot}{#DefSrc}\EwERandomizeMPAPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\input\randomizeMPAs
#endif

; -- enaR --
#if enaR == 1
Source: "{#DefRoot}{#DefSrc}\enaRPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\output\enaR
#endif

; -- MSPTools --
#if MSPTools == 1
Source: "{#DefRoot}{#DefSrc}\EwEShell.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\ui\msptools
Source: "{#DefRoot}{#DefSrc}\EwEMSPToolsPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\ui\msptools
#endif

; -- RBT --
#if RobertsBank == 1
Source: "{#DefRoot}{#DefSrc}\EwEDepthChangePlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\robertsbank
Source: "{#DefRoot}{#DefSrc}\EwEEcospaceMonteCarloPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion; Components: plugin\robertsbank
#endif

; -- EcoOcean --
#if EcoOcean == 1
Source: "{#DefRoot}{#DefSrc}\EcoOceanCellSpecificTempResponsesPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EcoOceanLMEEffortPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EcoOceanNativeRangesPlugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EcoOceanQ10Plugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion
Source: "{#DefRoot}{#DefSrc}\EcoOceanUtils.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion
#endif

; -- FISHMIP --
#if FISHMIP == 1
Source: "{#DefRoot}{#DefSrc}\FishMIPv3Plugin.dll"; DestDir: "{app}\Plugins\"; Flags: ignoreversion
#endif

; -- SAMPLE DATABASES --
Source: "{#DefRoot}{#DefDB}\Generic_37.EwEmdb"; DestDir: "{userdocs}\EwE sample databases"; Flags: ignoreversion; Components: databases
Source: "{#DefRoot}{#DefDB}\Anchovy Bay Spatial.ewemdb"; DestDir: "{userdocs}\EwE sample databases"; Flags: ignoreversion; Components: databases
Source: "{#DefRoot}{#DefDB}\Tampa_Bay.EwEmdb"; DestDir: "{userdocs}\EwE sample databases"; Flags: ignoreversion; Components: databases
Source: "{#DefRoot}{#DefDB}\Georgia_Strait.EwEmdb"; DestDir: "{userdocs}\EwE sample databases"; Flags: ignoreversion; Components: databases

[Components]
Name: "userguide"; Description: "EwE user guide (2008)"; Types: full custom
Name: "databases"; Description: "Sample EwE models"; Types: full custom
Name: "plugin"; Description: "Plug-ins"; Types: full custom
Name: "plugin\analysis"; Description: "Analysis"; Types: full custom
Name: "plugin\analysis\ecolind"; Description: "Ecological Indicators"; Types: full
Name: "plugin\analysis\ecotroph"; Description: "EcoTroph"; Types: custom full
Name: "plugin\analysis\na"; Description: "Network Analysis"; Types: compact custom full
Name: "plugin\analysis\prebal"; Description: "Pre-balance diagnostics"; Types: full custom
Name: "plugin\analysis\ecoengineers"; Description: "Eco-engineer dynamics"; Types: full
Name: "plugin\analysis\ecotracer"; Description: "Ecotracer impacts"; Types: full
Name: "plugin\analysis\valuechain"; Description: "Value chain"; Types: full
Name: "plugin\input"; Description: "Data retrieval"; Types: full custom
Name: "plugin\input\worms"; Description: "WoRMS taxonomy search"; Types: full
Name: "plugin\input\aquamaps"; Description: "Aquamaps functional response importer"; Types: full
Name: "plugin\input\szumadiets"; Description: "Diet import utility"; Types: full
Name: "plugin\input\layerimportexport"; Description: "Ecospace layer style import and export"; Types: full
Name: "plugin\output"; Description: "Data export"; Types: full
Name: "plugin\output\modelfromsim"; Description: "Ecopath model from Ecosim"; Types: full
Name: "plugin\output\resultextractor"; Description: "Results extractor"; Types: full
Name: "plugin\automation"; Description: "Automation"; Types: full custom
Name: "plugin\automation\multisim"; Description: "Multi-Sim"; Types: custom full
Name: "plugin\automation\stepwisef"; Description: "Stepwise Fitting"; Types: full
Name: "plugin\automation\mse"; Description: "Cefas MSE"; Types: custom full
Name: "plugin\automation\sampler"; Description: "Ecosampler"; Types: full
Name: "plugin\ui"; Description: "Usability"; Types: full custom
Name: "plugin\ui\remarks"; Description: "Remarks collector"; Types: full custom
Name: "plugin\ui\shapegrid"; Description: "Shape grids"; Types: full custom
Name: "plugin\input\mergegroups"; Description: "Merge groups"; Types: full
Name: "plugin\input\mpadynamics"; Description: "MPA dynamics"; Types: full
Name: "plugin\output\transects"; Description: "Transects extraction"; Types: full
Name: "plugin\output\networkD3"; Description: "Export diet matrix to NetworkD3"; Types: full
Name: "plugin\output\spinup"; Description: "Ecospace spin-up"; Types: full
Name: "plugin\input\spattemp"; Description: "Spatial-temporal GIS data exchange framework"; Types: full

#if RobertsBank == 1
Name: "plugin\robertsbank"; Description: "Roberts Bank utilities"; Types: full custom
#endif
#if EcoOcean == 1
Name: "plugin\ecoocean"; Description: "EcoOcean"; Types: full custom
#endif
#if FISHMIP == 1
Name: "plugin\fishmip"; Description: "FishMIP/TRIATLAS utilities"; Types: full custom
#endif
#if MSPTools == 1
Name: "plugin\ui\msptools"; Description: "MSP tools"; Types: full
#endif
#if ExcludeDeadCells == 1
Name: "plugin\input\excldeadcells"; Description: "Exclude isolated cells"; Types: full
#endif
#if RandomizeMPAs == 1
Name: "plugin\input\randomizeMPAs"; Description: "Randomize MPA cells"; Types: full
#endif
; -- enaR --
#if enaR == 1
Name: "plugin\output\enaR"; Description: "Ecospace enaR"; Types: full
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
        idpAddFile('http://go.microsoft.com/fwlink/?LinkId=863262', ExpandConstant('{tmp}\NetFrameworkInstaller.exe'));
        idpDownloadAfter(wpReady);
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
