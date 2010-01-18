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

        Public Class cImportSettings

            Private m_mi As cEwE6DatabaseImporter.cEwE5ModelInfo = Nothing
            Private m_bImport As Boolean = False
            Private m_strEwE6Name As String = ""

            Public Sub New(ByVal mi As cEwE6DatabaseImporter.cEwE5ModelInfo)
                Me.m_mi = mi
                Me.m_bImport = False
                Me.m_strEwE6Name = mi.Name
            End Sub

            Public ReadOnly Property ModelInfo() As cEwE6DatabaseImporter.cEwE5ModelInfo
                Get
                    Return Me.m_mi
                End Get
            End Property

            Public Property Import() As Boolean
                Get
                    Return Me.m_bImport
                End Get
                Set(ByVal value As Boolean)
                    Me.m_bImport = value
                End Set
            End Property

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
        ''' States whether the wizard is able to import with its current
        ''' import settings.
        ''' </summary>
        ''' <returns>True if able to import.</returns>
        ''' -------------------------------------------------------------------
        Public Function CanImport() As Boolean

            Dim di As DirectoryInfo = Nothing

            Try
                di = New DirectoryInfo(Me.m_strOutputFolder)
            Catch ex As Exception
                Return False
            End Try

            If (Not di.Exists) Then Return False

            For Each setting As cImportWizard.cImportSettings In Me.ImportSettings
                If setting.Import Then Return True
            Next

            Return False

        End Function

        Private Function EwE6ModelName(ByVal setting As cImportSettings) As String
            Dim strModel As String = Path.Combine(Me.m_strOutputFolder, setting.EwE6ModelName)
            strModel += cDataSourceFactory.GetDefaultExtension(EwEUtils.Core.eDataSourceTypes.ACCDB)
            Return strModel
        End Function

        Public Function Import(ByVal setting As cImportSettings) As Boolean

            Dim appl As AppLauncher = AppLauncher.GetInstance()
            Dim db As cEwEDatabase = Nothing
            Dim strModel As String = Me.EwE6ModelName(setting)
            Dim strLogFileName As String = ""
            Dim bSucces As Boolean = False

            If Not setting.Import Then Return bSucces

            ' Request a database to import into
            db = appl.CreateEcopathModel(strModel, setting.ModelInfo.ID)

            If (db IsNot Nothing) Then
                db.Open(strModel)
                If Me.m_dbImp.Import(setting.ModelInfo.ID, db, strLogFileName) Then
                    Me.m_strFileName = strModel
                    bSucces = True
                End If
                db.Close()
            End If

            Return bSucces

        End Function

    End Class

End Namespace
