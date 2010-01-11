'==============================================================================
'
' $Log: DatabaseConversionWizard.vb,v $
' Revision 1.7  2009/05/11 01:50:51  jeroens
' Renamed command classes
'
' Revision 1.6  2009/04/21 19:42:29  jeroens
' Localized
'
' Revision 1.5  2009/01/19 18:07:25  jeroens
' MessageHandlers, CoreStateMonitor have sync objects
'
' Revision 1.4  2009/01/16 18:30:37  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.3  2008/12/10 02:12:32  jeroens
' Moved datasource types to EwEUtils
'
' Revision 1.2  2008/11/08 23:52:52  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:32:22  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports EwECore
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.Utilities
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Wizard

    Public Class DatabaseConversionWizard

        ''' <summary>The selected EwE5 database.</summary>
        Private m_strFileName As String = ""
        ''' <summary>The import log file.</summary>
        Private m_strLogFileName As String = ""
        ''' <summary>Importer.</summary>
        Private m_dbImp As cEwE6DatabaseImporter = Nothing
        ''' <summary>The newly converted EwE6 database.</summary>
        Private m_strImportedFileName As String = String.Empty
        ''' <summary>Reference to the core</summary>
        Private m_core As cCore = Nothing

        Private Enum eImportState
            Preparing
            Importing
            Completed
        End Enum

        Private m_importState As eImportState = eImportState.Preparing

#Region " Helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class, wraps <see cref="cEwE6DatabaseImporter.cEwE5ModelInfo">importer Model info</see>
        ''' in a practical class for display and usage in the model list box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class ModelListboxItem

            ''' <summary>The model info item that is wrapped</summary>
            Private m_mi As cEwE6DatabaseImporter.cEwE5ModelInfo = Nothing

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instance
            ''' </summary>
            ''' <param name="mi">The <see cref="cEwE6DatabaseImporter.cEwE5ModelInfo">importer Model info</see>
            ''' to wrap.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal mi As cEwE6DatabaseImporter.cEwE5ModelInfo)
                Me.m_mi = mi
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Returns a string representation of the model for display in the
            ''' model selection list box.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Overrides Function ToString() As String
                Return Me.m_mi.Name()
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get a reference to the wrapped <see cref="cEwE6DatabaseImporter.cEwE5ModelInfo">importer Model info</see>.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property ModelInfo() As cEwE6DatabaseImporter.cEwE5ModelInfo
                Get
                    Return Me.m_mi
                End Get
            End Property

        End Class

#End Region ' Helper classes

        Public Sub New(ByVal fileName As String, ByRef db As cEwEDatabase, ByRef core As cCore)

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.m_strFileName = fileName
            Me.m_dbImp = New cEwE6DatabaseImporter(core)
            Me.m_dbImp.Attach(db)
            Me.m_core = core

        End Sub

#Region " Public properties "

        Public ReadOnly Property ImportedFileName() As String
            Get
                Return Me.m_strImportedFileName
            End Get
        End Property

#End Region ' Public properties

        Private Property ImportState() As eImportState
            Get
                Return Me.m_importState
            End Get
            Set(ByVal value As eImportState)
                Me.m_importState = value
                Me.UpdateWizardButtons()
            End Set
        End Property

        Protected Overrides Sub ActivatePage(ByVal pageIndex As Integer)

            Dim lModels As List(Of cEwE6DatabaseImporter.cEwE5ModelInfo) = Nothing
            Dim model As cEwE6DatabaseImporter.cEwE5ModelInfo = Nothing
            Dim appl As AppLauncher = AppLauncher.GetInstance()
            Dim db As cEwEDatabase = Nothing
            Dim strFileName As String = ""

            Me.ImportState = eImportState.Preparing

            Select Case pageIndex
                Case 0 ' Welcome page, Nothing to do here.
                    txbSaveModelName.Text = String.Empty
                    lbModels.SelectedIndex = -1

                Case 1 ' Select model page
                    Me.lblDatabaseName.Text = Me.m_strFileName
                    Me.lbModels.Items.Clear()

                    ' Get list of models from the importer
                    lModels = Me.m_dbImp.GetModels()
                    ' For each model
                    For i As Integer = 0 To lModels.Count - 1
                        ' Get it
                        model = lModels(i)
                        ' Add model to the model list box, wrapped in an instance of
                        ' the handy-dandy ModelListBoxItem class
                        Me.lbModels.Items.Add(New ModelListboxItem(model))
                    Next

                Case 2 ' summary
                    Me.txbSummary.Text = String.Format(My.Resources.PROMPT_IMPORT_SUMMARY, vbNewLine, _
                        Me.m_strFileName, lbModels.SelectedItem.ToString, txbSaveModelName.Text)
                    Me.txbSummary.Select(1, 0)

                    ' Import the model to EwE6 format
                    Dim bSuccess As Boolean = False
                    Dim mh As cMessageHandler = New cMessageHandler(AddressOf Me.ProgressMessageHandler, eCoreComponentType.DataSource, eMessageType.Any, AppLauncher.GetInstance().SyncObject)
                    Dim mlbi As ModelListboxItem = Nothing

                    m_core.Messages.AddMessageHandler(mh)

                    Try
                        ' Set status text
                        cApplicationStatusNotifier.SetStatusText("Importing...", TriState.True)

                        mlbi = DirectCast(lbModels.SelectedItem, ModelListboxItem)
                        ' Request a database to import into
                        db = appl.CreateEcopathModel(txbSaveModelName.Text, mlbi.ModelInfo.ID)

                        If (db IsNot Nothing) Then
                            db.Open(txbSaveModelName.Text)
                            Me.ImportState = eImportState.Importing
                            If Me.m_dbImp.Import(mlbi.ModelInfo.ID, db, Me.m_strLogFileName) Then
                                Me.m_strImportedFileName = txbSaveModelName.Text
                                bSuccess = True
                            End If
                            db.Close()
                        End If
                    Finally
                        cApplicationStatusNotifier.SetStatusText("", TriState.False)
                    End Try

                    If Not bSuccess Then
                        MessageBox.Show(My.Resources.PROMPT_ERROR_IMPORTFAILED, My.Resources.IMPORT_ERROR_CAPTION, _
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Me.ImportState = eImportState.Preparing
                    Else
                        Me.ImportState = eImportState.Completed
                    End If

                    m_core.Messages.RemoveMessageHandler(mh)

            End Select
        End Sub

        Protected Overrides Function ValidatePage(ByVal pageIndex As Integer) As Boolean

            Dim bValidated As Boolean = MyBase.ValidatePage(pageIndex)
            Select Case pageIndex
                Case 1
                    bValidated = bValidated And Me.IsFileNameValid(txbSaveModelName.Text)
            End Select
            Return bValidated

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Standard Core messages handler where all property updates are triggered
        ''' </summary>
        ''' <param name="msg">An arriving message</param>
        ''' -------------------------------------------------------------------
        Private Sub ProgressMessageHandler(ByRef msg As cMessage)
            If Not TypeOf msg Is cProgressMessage Then Return

            Try
                Dim pmsg As cProgressMessage = DirectCast(msg, cProgressMessage)
                Me.m_pb.Maximum = 0
                Me.m_pb.Maximum = 100
                Me.m_pb.Value = Math.Max(0, Math.Min(100, CInt(100 * pmsg.Progress)))

                Me.m_lbProgress.Text = pmsg.Message
            Catch ex As Exception

            End Try

            Me.Refresh()

        End Sub

        Private Sub lbModels_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbModels.SelectedIndexChanged
            ' One model selected for conversion
            If lbModels.SelectedIndex <> -1 Then
                Dim strPath As String = My.Computer.FileSystem.CurrentDirectory
                Dim strFileName As String = String.Format("{0}.EwEmdb", lbModels.SelectedItem.ToString.Trim())

                strFileName = FileUtilities.ToValidFileName(strFileName, False)

                ' Use full path
                txbSaveModelName.Text = Path.Combine(strPath, strFileName)

            End If
            ' Trigger button updates
            UpdateWizardButtons()
        End Sub

        Private Function IsFileNameValid(ByVal strFileName As String) As Boolean

            Dim strPrompt As String = ""
            Dim strCaption As String = My.Resources.IMPORT_ERROR_CAPTION
            Dim bValid As Boolean = True

            ' Test if output directory exists
            If Not Directory.Exists(Path.GetDirectoryName(strFileName)) Then

                strPrompt = String.Format(My.Resources.GENERIC_PROMPT_INVALIDPATH, Path.GetDirectoryName(strFileName))
                MessageBox.Show(strPrompt, strCaption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                bValid = False

            ElseIf File.Exists(strFileName) Then

                strPrompt = String.Format(My.Resources.GENERIC_PROMPT_OVERWRITEFILE, strFileName)
                bValid = (MessageBox.Show(strPrompt, strCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes)

            End If

            Return bValid
        End Function

        Private Sub OnCheckSaveModelName(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles txbSaveModelName.Validated
            UpdateWizardButtons()
        End Sub

        Private Sub txbSaveModelName_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) _
                Handles txbSaveModelName.Validating
            UpdateWizardButtons()
        End Sub

        ''' <summary>
        ''' Model selection double-click handler, called to step to the next page in the wizard.
        ''' </summary>
        Private Sub lbModels_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbModels.DoubleClick
            Me.btnNext.PerformClick()
        End Sub

        Private Sub btnBrowseTargetDirectory_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseTargetDirectory.Click

            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmdFS As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)

            cmdFS.Invoke(Me.txbSaveModelName.Text, "", My.Resources.FILEFILTER_MODEL_SAVE)

            If cmdFS.Result = DialogResult.OK Then
                txbSaveModelName.Text = cmdFS.FileName
            End If

        End Sub

#Region " Wizard button updating "

        Protected Overrides Function OnUpdateBackButton() As Boolean
            Return MyBase.OnUpdateBackButton() And (Me.ImportState <> eImportState.Importing)
        End Function

        Protected Overrides Function OnUpdateNextButton() As Boolean
            Dim bEnableNext As Boolean = MyBase.OnUpdateNextButton()

            bEnableNext = bEnableNext And (Me.ImportState <> eImportState.Importing)

            ' Page specific logic
            Select Case Me.CurrentPage
                Case 0
                Case 1
                    ' Enable when a model to convert is selected
                    bEnableNext = bEnableNext And (lbModels.SelectedIndex > -1)
                    ' ..and when an output location has been specified
                    bEnableNext = bEnableNext And (cDataSourceFactory.GetSupportedType(txbSaveModelName.Text) <> eDataSourceTypes.NotSet)
                Case 2
            End Select

            Return bEnableNext
        End Function

        Protected Overrides Function OnUpdateCancelButton() As Boolean
            Return MyBase.OnUpdateCancelButton() And (Me.ImportState <> eImportState.Importing)
        End Function

        Protected Overrides Function OnUpdateFinishButton() As Boolean
            ' Can only finish when import completed
            Return (Me.ImportState = eImportState.Completed)
        End Function

#End Region ' Wizard button updating

    End Class

End Namespace
