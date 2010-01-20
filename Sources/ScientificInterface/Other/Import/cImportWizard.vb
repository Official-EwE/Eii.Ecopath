#Region " Imports "

Option Strict On

Imports System.IO
Imports EwECore
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls.Wizard

#End Region ' Imports

Namespace Import

    ''' =======================================================================
    ''' <summary>
    ''' Wizard that guides users through the process of importing models of
    ''' previous EwE versions.
    ''' </summary>
    ''' =======================================================================
    Public Class cImportWizard
        Inherits cWizard

#Region " Private bits "

        ''' <summary>The actual EwE database importer.</summary>
        Private m_dbImp As cEwE6DatabaseImporter = Nothing
        ''' <summary>A setting for each EwE5 model.</summary>
        Private m_lImportSettings As New List(Of cImportSettings)
        ''' <summary>Folder to place imported models into.</summary>
        Private m_strOutputFolder As String = ""
        ''' <summary>Database being opened</summary>
        Private m_strDatabase As String = ""
        ''' <summary>Last imported file name.</summary>
        Private m_strFileName As String = ""

#End Region ' Private bits

#Region " Helper classes "

        ''' ===================================================================
        ''' <summary>
        ''' Helper class, maintains import settings for a single model.
        ''' </summary>
        ''' ===================================================================
        Public Class cImportSettings

#Region " Privates vars "

            ''' <summary>EwE5 model info.</summary>
            Private m_mi As cEwE6DatabaseImporter.cEwE5ModelInfo = Nothing
            ''' <summary>Flag stating whether this EwE5 model should be imported.</summary>
            Private m_bImport As Boolean = False
            ''' <summary>EwE6 name of the model to import into.</summary>
            Private m_strEwE6Name As String = ""
            ''' <summary>Path to the import log file once an import is completed.</summary>
            Private m_strLogFile As String = ""

#End Region ' Privates vars

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Create a new import setting for an EwE5 model.
            ''' </summary>
            ''' <param name="mi">The <see cref="cEwE6DatabaseImporter.cEwE5ModelInfo">
            ''' to create import settings for.</param>
            ''' -----------------------------------------------------------------------
            Public Sub New(ByVal mi As cEwE6DatabaseImporter.cEwE5ModelInfo)
                Me.m_mi = mi
                Me.m_bImport = False
                Me.m_strEwE6Name = mi.Name
            End Sub

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="cEwE6DatabaseImporter.cEwE5ModelInfo">EwE5 model 
            ''' information</see> associated with this import setting.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public ReadOnly Property ModelInfo() As cEwE6DatabaseImporter.cEwE5ModelInfo
                Get
                    Return Me.m_mi
                End Get
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this EwE5 model should be imported.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Property SelectedForImport() As Boolean
                Get
                    Return Me.m_bImport
                End Get
                Set(ByVal value As Boolean)
                    Me.m_bImport = value
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Get/set the name of the EwE6 model to import into.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Property EwE6ModelName() As String
                Get
                    Return Me.m_strEwE6Name
                End Get
                Set(ByVal value As String)
                    Me.m_strEwE6Name = Me.ToEwE6ModelName(value)
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Get/set the import log file.
            ''' </summary>
            ''' -----------------------------------------------------------------------
            Public Property LogFile() As String
                Get
                    Return Me.m_strLogFile
                End Get
                Set(ByVal value As String)
                    Me.m_strLogFile = value
                End Set
            End Property

            ''' -----------------------------------------------------------------------
            ''' <summary>
            ''' Convert a EwE5 model name to an EwE6 model name.
            ''' </summary>
            ''' <param name="strEwE5Model"></param>
            ''' <returns></returns>
            ''' -----------------------------------------------------------------------
            Private Function ToEwE6ModelName(ByVal strEwE5Model As String) As String
                Return FileUtilities.ToValidFileName(strEwE5Model, False)
            End Function

        End Class

#End Region ' Helper classes

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Construct a new import wizard.
        ''' </summary>
        ''' <param name="core">The EwE core to operate on.</param>
        ''' <param name="db">The EwE5 database to import from.</param>
        ''' <param name="parent">The form hosting the wizard UI.</param>
        ''' <param name="content">The panel where this wizard can display its pages.</param>
        ''' <param name="nav">The navigation that controls this wizard.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
                       ByRef db As cEwEDatabase, _
                       ByVal parent As Form, _
                       ByVal content As Panel, _
                       ByVal nav As IWizardNavigation)

            MyBase.New(core, parent, content, nav)

            ' Hook up with data
            Me.m_dbImp = New cEwE6DatabaseImporter(core)
            Me.m_dbImp.Attach(db)

            Me.m_strDatabase = db.Name
            Me.m_strOutputFolder = Path.GetDirectoryName(db.Name)

            ' Prepare import settings
            For Each mi As cEwE6DatabaseImporter.cEwE5ModelInfo In Me.m_dbImp.GetModels
                Me.m_lImportSettings.Add(New cImportSettings(mi))
            Next

            ' Add pages
            Me.AddPage(GetType(ucImportPageWelcome))
            Me.AddPage(GetType(ucImportPageModels))
            Me.AddPage(GetType(ucImportPageProgress))

        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the <see cref="cImportSettings">import settings</see> for
        ''' the current selected EwE5 database.
        ''' </summary>
        ''' <returns>
        ''' An array of <see cref="cImportSettings">import settings</see>.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function ImportSettings() As cImportSettings()
            Return Me.m_lImportSettings.ToArray
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the database being opened.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Database() As String
            Get
                Return Me.m_strDatabase
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the database imported if only one database was imported.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Filename() As String
            Get
                Return Me.m_strFileName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the output folder for placing imported models.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property OutputFolder() As String
            Get
                Return Me.m_strOutputFolder
            End Get
            Set(ByVal value As String)
                Me.m_strOutputFolder = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the wizard has a valid output path.
        ''' </summary>
        ''' <returns>
        ''' True if the wizard has a valid output path.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function HasValidOutputPath() As Boolean

            Dim di As DirectoryInfo = Nothing
            Try
                di = New DirectoryInfo(Me.m_strOutputFolder)
            Catch ex As Exception
                Return False
            End Try
            ' ToDo: include checking of directory write access?
            Return di.Exists

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the wizard has at least ONE model selected for import.
        ''' </summary>
        ''' <returns>
        ''' True if the wizard has at least ONE model selected for import.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function HasModelSelectedForImport() As Boolean

            For Each setting As cImportWizard.cImportSettings In Me.ImportSettings
                If setting.SelectedForImport Then Return True
            Next

            Return False

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform a model import.
        ''' </summary>
        ''' <param name="setting">The <see cref="cImportSettings">model</see>
        ''' to import.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Import(ByVal setting As cImportSettings) As Boolean

            Dim appl As AppLauncher = AppLauncher.GetInstance()
            Dim db As cEwEDatabase = Nothing
            Dim strModel As String = Me.EwE6ModelName(setting)
            Dim bSucces As Boolean = False

            ' Only import models selected for import
            If (Not setting.SelectedForImport) Then Return bSucces

            ' Request a database to import into
            db = appl.CreateEcopathModel(strModel, setting.ModelInfo.ID)

            ' Able to create target model?
            If (db IsNot Nothing) Then
                ' #Yes: Open target model
                db.Open(strModel)
                ' Able to import?
                If Me.m_dbImp.Import(setting.ModelInfo.ID, db, setting.LogFile) Then
                    ' #Yes: remember last imported model file
                    Me.m_strFileName = strModel
                    ' Succes
                    bSucces = True
                End If
                ' Clean up
                db.Close()
            End If

            ' Report succes
            Return bSucces

        End Function

#End Region ' Public access

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a valid EwE6 model path for an import setting.
        ''' </summary>
        ''' <param name="setting">The setting to create an EwE6 model path for.</param>
        ''' <returns>A valid EwE6 model path for an import setting.</returns>
        ''' -------------------------------------------------------------------
        Private Function EwE6ModelName(ByVal setting As cImportSettings) As String
            Dim strModel As String = Path.Combine(Me.m_strOutputFolder, setting.EwE6ModelName)
            strModel += cDataSourceFactory.GetDefaultExtension(EwEUtils.Core.eDataSourceTypes.ACCDB)
            Return strModel
        End Function

#End Region ' Internals
    End Class

End Namespace
