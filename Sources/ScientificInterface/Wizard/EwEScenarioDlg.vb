'==============================================================================
'
' $Log: EwEScenarioDlg.vb,v $
' Revision 1.1  2008/09/26 07:32:22  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.18  2008/08/15 17:21:24  jeroens
' Cannot delete a loaded scenario
'
' Revision 1.17  2008/07/28 22:28:53  jeroens
' Fixed issue 438
'
' Revision 1.16  2008/05/08 17:16:26  jeroens
' SaveAs mode does not update description, author, contact when selecting different scenarios
'
' Revision 1.15  2008/05/07 21:43:18  villyc
' Fixed crash on open ecospace without scenarios
'
' Revision 1.14  2008/04/26 10:21:55  sherman
' Select first item and set OK Cancel button
'
' Revision 1.13  2008/04/07 02:31:19  jeroens
' Cleaning up resources
'
' Revision 1.12  2008/02/13 03:55:13  jeroens
' Replaced author column with loaded status
'
' Revision 1.11  2008/02/01 03:40:19  jeroens
' Default scenario can be selected
'
' Revision 1.10  2008/01/30 02:57:53  jeroens
' Last saved string entirely formatted by system
'
' Revision 1.9  2008/01/28 04:38:18  jeroens
' Scenarios listed in a list view
'
' Revision 1.8  2008/01/11 12:46:30  jeroens
' Properly localized
'
' Revision 1.7  2007/12/17 14:07:29  jeroens
' * Fixed crash
'
' Revision 1.6  2007/12/08 00:54:38  jeroens
' * Debugged automated tab switching
'
' Revision 1.5  2007/12/07 17:46:24  jeroens
' + Organized
'
' Revision 1.4  2007/12/07 17:23:42  jeroens
' * Fixed startup load/create mode bug
'
' Revision 1.3  2007/12/07 16:49:44  jeroens
' * Hmpf, users allowed to plagiarize
'
' Revision 1.2  2007/12/07 16:46:26  jeroens
' * Simplified scenario dialog interface
' * Fixed interaction
'
' Revision 1.1  2007/12/06 16:29:00  jeroens
' Initial version
'
'==============================================================================

#Region " Imports Directive "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Ecopath.Input
Imports ScientificInterface.Ecopath.Output

#End Region ' Imports Directive

Namespace Wizard

    ''' <summary>
    ''' Dialog to load, save or create an EwE scenario.
    ''' </summary>
    ''' <remarks>
    ''' <para>This dialog can be opened in four modes:</para>
    ''' <list type="bullet">
    ''' <item>
    ''' <term>
    ''' <see cref="EwEScenarioDlg.eDialogModeType.LoadScenario">LoadScenario</see>
    ''' </term>
    ''' <description>
    ''' Opens the dialog for loading an existing scenario.
    ''' </description>
    ''' </item>
    ''' <item>
    ''' <term>
    ''' <see cref="EwEScenarioDlg.eDialogModeType.CreateScenario">CreateScenario</see>
    ''' </term>
    ''' <description>
    ''' Opens the dialog for creating a new scenario.
    ''' </description>
    ''' </item>
    ''' <item>
    ''' <term>
    ''' <see cref="EwEScenarioDlg.eDialogModeType.SaveScenario">SaveScenario</see>
    ''' </term>
    ''' <description>
    ''' Opens the dialog for saving the current loaded scenario.
    ''' </description>
    ''' </item>
    ''' <item>
    ''' <term>
    ''' <see cref="EwEScenarioDlg.eDialogModeType.DeleteScenario">DeleteScenario</see>
    ''' </term>
    ''' <description>
    ''' Opens the dialog for deleting an existing scenario.
    ''' </description>
    ''' </item>
    ''' </list>
    ''' </remarks>
    Public Class EwEScenarioDlg

        ''' <summary>
        ''' <para>Enumerated type defining dialog interaction modes.</para>
        ''' </summary>
        Public Enum eDialogModeType
            ''' <summary>Use the dialog to support creating a new EwE scenario.</summary>
            CreateScenario
            ''' <summary>Use the dialog to support saving an EwE scenario.</summary>
            SaveScenario
            ''' <summary>Use the dialog to support loading an existing EwE scenario.</summary>
            LoadScenario
            ''' <summary>Use the dialog to deleting an EwE scenario.</summary>
            DeleteScenario
        End Enum

        Private Enum eColumnTypes
            Name
            Loaded
            LastSaved
        End Enum

#Region " Helper classes "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class for sorting <see cref="cEwEScenario">scenario</see>
        ''' list view items for specific <see cref="eColumnTypes">column type</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class ColumnSorter
            Implements IComparer

            ''' <summary><see cref="eColumnTypes">column type</see> to sort by.</summary>
            Private m_column As eColumnTypes

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set <see cref="eColumnTypes">column type</see> to sort by
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Column() As eColumnTypes
                Get
                    Return Me.m_column
                End Get
                Set(ByVal value As eColumnTypes)
                    Me.m_column = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Perform actual sort
            ''' </summary>
            ''' <param name="x"></param>
            ''' <param name="y"></param>
            ''' <returns></returns>
            ''' -------------------------------------------------------------------
            Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
                    Implements IComparer.Compare

                Dim lvi1 As ListViewItem = DirectCast(x, ListViewItem)
                Dim s1 As cEwEScenario = DirectCast(lvi1.Tag, cEwEScenario)
                Dim lvi2 As ListViewItem = DirectCast(y, ListViewItem)
                Dim s2 As cEwEScenario = DirectCast(lvi2.Tag, cEwEScenario)

                ' Assume both not null
                Select Case Me.m_column
                    Case eColumnTypes.Name
                        ' Sort names ascending
                        Return String.Compare(s1.Name, s2.Name, True)
                    Case eColumnTypes.Loaded
                        ' Sort loaded scenario first
                        If s1.IsLoaded Then Return 1
                        If s2.IsLoaded Then Return -1
                        Return 0
                    Case eColumnTypes.LastSaved
                        ' Sort save dates in descending order
                        If s1.LastSaved > s2.LastSaved Then Return -1
                        If s1.LastSaved = s2.LastSaved Then Return 0
                        Return 1

                End Select
                Return 0

            End Function

        End Class

#End Region ' Helper classes

#Region " Private vars "

        ''' <summary>Ref to the core.</summary>
        Protected m_core As cCore = cCore.GetInstance()
        ''' <summary>Ref to the main window.</summary>
        Private m_appl As AppLauncher = AppLauncher.GetInstance()

        ''' <summary>Dialog operation mode.</summary>
        Private m_mode As eDialogModeType = eDialogModeType.CreateScenario
        ''' <summary>Scenario that is selected in the dialog.</summary>
        Private m_scenario As cEwEScenario = Nothing
        ''' <summary>Scenario that the dialog was invoked for, if any.</summary>
        Private m_scenarioSource As cEwEScenario = Nothing
        ''' <summary>List of available scenarios.</summary>
        Private m_lScenarios As List(Of cEwEScenario)

#End Region ' Private vars

#Region " Constructor "

        ''' <summary>
        ''' Constructor, initializes a new instance of this dialog.
        ''' </summary>
        ''' <param name="mode"><see cref="eDialogModeType">Dialog interaction mode</see>.</param>
        ''' <param name="scenario">EwE scenario to save, if any.</param>
        Public Sub New(ByVal mode As eDialogModeType, _
                Optional ByVal scenario As cEwEScenario = Nothing)

            ' This call is required by the Windows Form Designer.
            Me.InitializeComponent()
            Me.m_mode = mode
            Me.m_scenario = scenario
            Me.m_scenarioSource = scenario
            Me.m_lScenarios = Me.GetAvailableScenarios()
            Me.UpdateScenarioListControls()

            ' Init create dialog
            Me.tbNameCreate.Text = Me.GetNewScenarioName()
            Me.tbDescriptionCreate.Text = String.Format(My.Resources.GENERIC_DEFAULT_DESCRIPTION, Date.Now().ToShortDateString(), Date.Now().ToShortTimeString())
            Me.tbAuthorCreate.Text = m_core.EwEModel.Author
            Me.tbContactCreate.Text = m_core.EwEModel.Contact

            Me.Icon = Me.GetIcon()
        End Sub

#End Region ' Constructor

#Region " Overridables "

        Protected Overridable Function GetIcon() As Icon
            Return Me.Icon
        End Function

        Protected Overridable Function GetNewScenarioName() As String
            Return ""
        End Function

        Protected Overridable Function GetDialogCaption(ByVal mode As eDialogModeType, ByVal strEwEModelName As String) As String
            Return ""
        End Function

        Protected Overridable Function DeleteScenario(ByVal scenario As cEwEScenario) As Boolean
            Return False
        End Function

        Protected Overridable Function GetAvailableScenarios() As List(Of cEwEScenario)
            Return Nothing
        End Function

#End Region ' Overridables

#Region " Internal implementation "

        Private Sub SwitchMode(ByVal mode As eDialogModeType)

            For iPage As Integer = 0 To Me.tabctrlModes.TabCount - 1
                Dim tp As TabPage = Me.tabctrlModes.TabPages(iPage)
                If (CInt(tp.Tag) = mode) Then
                    Me.tabctrlModes.SelectedTab = tp
                End If
            Next

            Me.m_mode = mode
            Me.InitControls()

        End Sub

        Private Sub InitControls()

            Me.Text = Me.GetDialogCaption(Me.m_mode, m_core.EwEModel.Name)

            Me.tsmCreate.Visible = (Me.m_mode = eDialogModeType.CreateScenario)
            Me.tsmLoad.Visible = (Me.m_mode = eDialogModeType.LoadScenario)
            Me.tsmDelete.Visible = (Me.m_mode = eDialogModeType.DeleteScenario)
            Me.tsmSave.Visible = (Me.m_mode = eDialogModeType.SaveScenario)
            Me.tsmRename.Visible = (Me.m_mode = eDialogModeType.CreateScenario Or Me.m_mode = eDialogModeType.DeleteScenario)

            ' JS 07may08: 'Save As' must use source scenario details
            If Me.m_scenarioSource IsNot Nothing Then
                Me.tbNameSaveAs.Text = Me.m_scenarioSource.Name
                Me.tbDescriptionSaveAs.Text = Me.m_scenarioSource.Description
                Me.tbAuthorSaveAs.Text = Me.m_scenarioSource.Author
                Me.tbContactSaveAs.Text = Me.m_scenarioSource.Contact
            End If

            Me.UpdateControls()

        End Sub

        Private Sub UpdateControls()

            Select Case Me.m_mode

                Case eDialogModeType.CreateScenario
                    Me.btnCreate.Enabled = Me.CanCreateScenario()
                    Me.tsmCreate.Enabled = Me.CanCreateScenario()
                    Me.tsmRename.Enabled = Me.CanRenameScenario()
                    Me.AcceptButton = Me.btnCreate
                    Me.CancelButton = Me.btnCancelCreate

                    ' Do not sync any of the fields

                Case eDialogModeType.LoadScenario
                    Me.btnLoad.Enabled = Me.CanLoadScenario()
                    Me.tsmLoad.Enabled = Me.CanLoadScenario()
                    Me.tsmRename.Enabled = False
                    Me.AcceptButton = Me.btnLoad
                    Me.CancelButton = Me.btnCancelLoad

                    ' Sync all with selection
                    If Me.Scenario IsNot Nothing Then
                        Me.tbDescriptionLoad.Text = Scenario.Description
                        Me.tbAuthorLoad.Text = Scenario.Author
                        Me.tbContactLoad.Text = Scenario.Contact
                    End If

                Case eDialogModeType.SaveScenario
                    Me.btnSave.Enabled = Me.CanSaveScenario()
                    Me.tsmSave.Enabled = Me.CanSaveScenario()
                    Me.tsmRename.Enabled = Me.CanRenameScenario()
                    Me.AcceptButton = Me.btnSave
                    Me.CancelButton = Me.btnCancelSave

                    ' Sync name with selection
                    If Me.Scenario IsNot Nothing Then
                        Me.tbNameSaveAs.Text = Scenario.Name
                    End If

                Case eDialogModeType.DeleteScenario
                    Me.btnDelete.Enabled = Me.CanDeleteScenario()
                    Me.tsmDelete.Enabled = Me.CanDeleteScenario()
                    Me.tsmRename.Enabled = False
                    Me.AcceptButton = Me.btnDelete
                    Me.CancelButton = Me.btnCancelDelete

                    ' Sync all with selection
                    If Me.Scenario IsNot Nothing Then
                        Me.tbDescriptionDelete.Text = Scenario.Description
                        Me.tbAuthorDelete.Text = Scenario.Author
                        Me.tbContactDelete.Text = Scenario.Contact
                    End If

            End Select

        End Sub

        Private Function GetScenarioListViewItem(ByVal scenario As cEwEScenario) As ListViewItem

            Dim lvi As ListViewItem = Nothing
            Dim astrColumns([Enum].GetValues(GetType(eColumnTypes)).Length - 1) As String

            ' Pop columns
            ' - name
            astrColumns(eColumnTypes.Name) = scenario.Name
            ' - Loaded
            If scenario.IsLoaded Then
                astrColumns(eColumnTypes.Loaded) = My.Resources.HEADER_YES
            Else
                astrColumns(eColumnTypes.Loaded) = ""
            End If
            ' - last saved date
            If (scenario.LastSaved > 0) Then
                Dim dtDate As Date = Date.FromOADate(CDbl(scenario.LastSaved))
                astrColumns(eColumnTypes.LastSaved) = String.Format("{0:g}", dtDate)
            Else
                astrColumns(eColumnTypes.LastSaved) = ""
            End If

            ' Prep item
            lvi = New ListViewItem(astrColumns)
            lvi.Tag = scenario

            Return lvi

        End Function

        ''' <summary>
        ''' Repopulate the scenario list boxes, preserving the selection if possible.
        ''' </summary>
        Private Sub UpdateScenarioListControls()

            ' Clear the list first
            Me.lvCreate.Items.Clear()
            Me.lvLoad.Items.Clear()
            Me.lvDelete.Items.Clear()
            Me.lvSaveAs.Items.Clear()

            ' Add the list of scenarios
            For i As Integer = 0 To Me.m_lScenarios.Count - 1

                Me.lvCreate.Items.Add(Me.GetScenarioListViewItem(m_lScenarios(i)))
                Me.lvLoad.Items.Add(Me.GetScenarioListViewItem(m_lScenarios(i)))
                Me.lvDelete.Items.Add(Me.GetScenarioListViewItem(m_lScenarios(i)))
                Me.lvSaveAs.Items.Add(Me.GetScenarioListViewItem(m_lScenarios(i)))
            Next

            ' Set the selected index
            If Me.m_lScenarios.Count > 0 Then
                Me.lvCreate.TopItem.Selected = True
                Me.lvLoad.TopItem.Selected = True
                Me.lvDelete.TopItem.Selected = True
                Me.lvSaveAs.TopItem.Selected = True
            End If

            ' Update selection
            Me.Scenario = Me.m_scenario

        End Sub

        Private Function CanCreateScenario() As Boolean
            Dim bHasName As Boolean = Not (String.IsNullOrEmpty(Me.tbNameCreate.Text))
            Dim bHasUniqueName As Boolean = bHasName And (Me.Scenario Is Nothing)
            Dim bIsCorrectMode As Boolean = (Me.m_mode = eDialogModeType.CreateScenario)
            Return bHasUniqueName And bIsCorrectMode
        End Function

        Private Function CanLoadScenario() As Boolean
            Dim bHasSelection As Boolean = (Me.Scenario IsNot Nothing)
            Dim bIsCorrectMode As Boolean = (Me.m_mode = eDialogModeType.LoadScenario)
            Return bHasSelection And bIsCorrectMode
        End Function

        Private Function CanSaveScenario() As Boolean
            Dim bHasName As Boolean = Not (String.IsNullOrEmpty(Me.tbNameSaveAs.Text))
            Dim bIsCorrectMode As Boolean = (Me.m_mode = eDialogModeType.SaveScenario)
            Return bHasName And bIsCorrectMode
        End Function

        Private Function CanDeleteScenario() As Boolean
            Dim bHasSelection As Boolean = (Me.Scenario IsNot Nothing)
            Dim bIsCorrectMode As Boolean = (Me.m_mode = eDialogModeType.DeleteScenario)
            Dim bIsLoaded As Boolean = (Me.Scenario.IsLoaded)
            Return bHasSelection And bIsCorrectMode And Not bIsLoaded
        End Function

        Private Function CanRenameScenario() As Boolean
            Dim bHasSelection As Boolean = (Me.lvCreate.SelectedIndices.Count = 1)
            Return bHasSelection
        End Function

        Private Function FindScenarioByName(ByVal strScenarioName As String) As cEwEScenario
            For iScenario As Integer = 0 To Me.m_lScenarios.Count - 1
                If (String.Compare(Me.m_lScenarios(iScenario).Name, strScenarioName, True) = 0) Then
                    Return Me.m_lScenarios(iScenario)
                End If
            Next
            Return Nothing
        End Function

        Private Property SelectedScenario(ByVal lv As ListView) As cEwEScenario
            Get
                If (lv.SelectedItems.Count <> 1) Then Return Nothing
                Return DirectCast(lv.SelectedItems(0).Tag, cEwEScenario)
            End Get
            Set(ByVal value As cEwEScenario)
                For Each item As ListViewItem In lv.Items
                    item.Selected = Object.ReferenceEquals(item.Tag, value)
                Next
            End Set
        End Property

#End Region ' Implementation

#Region " Event handlers "

        ''' <summary>
        ''' Event handler when dialog is being loaded.
        ''' </summary>
        Private Sub DoLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' Set up possible modes correctly
            With Me.tabctrlModes
                .TabPages(0).Tag = eDialogModeType.CreateScenario
                .TabPages(1).Tag = eDialogModeType.LoadScenario
                .TabPages(2).Tag = eDialogModeType.DeleteScenario
                .TabPages(3).Tag = eDialogModeType.SaveScenario
            End With

            Select Case Mode

                Case eDialogModeType.CreateScenario
                    ' Cannot save as
                    Me.tabctrlModes.TabPages.RemoveAt(3)
                    Me.tabctrlModes.SelectedIndex = 0

                Case eDialogModeType.LoadScenario
                    ' Cannot save as
                    Me.tabctrlModes.TabPages.RemoveAt(3)
                    Me.tabctrlModes.SelectedIndex = 1

                Case eDialogModeType.SaveScenario
                    ' Cannot create, cannot load, cannot delete
                    Me.tabctrlModes.TabPages.RemoveAt(0)
                    Me.tabctrlModes.TabPages.RemoveAt(0)
                    Me.tabctrlModes.TabPages.RemoveAt(0)
                    Me.tabctrlModes.SelectedIndex = 0

                Case eDialogModeType.DeleteScenario
                    ' Cannot save as
                    Me.tabctrlModes.TabPages.RemoveAt(3)
                    Me.tabctrlModes.SelectedIndex = 3

            End Select

            Me.lvCreate.ListViewItemSorter = New ColumnSorter()
            Me.lvLoad.ListViewItemSorter = New ColumnSorter()
            Me.lvDelete.ListViewItemSorter = New ColumnSorter()
            Me.lvSaveAs.ListViewItemSorter = New ColumnSorter()

            ' Get scenarios
            Me.UpdateScenarioListControls()

            ' In load mode and nothing to load?
            If ((Me.m_mode = eDialogModeType.LoadScenario) And (Me.m_lScenarios.Count = 0)) Then
                ' #Yes: switch to create mode
                Me.m_mode = eDialogModeType.CreateScenario
            End If

            Me.SwitchMode(Me.m_mode)

        End Sub

        Private Sub tabctrlModes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabctrlModes.SelectedIndexChanged
            ' Sanity check
            Debug.Assert(Object.ReferenceEquals(sender, Me.tabctrlModes))
            Me.Mode = DirectCast(Me.tabctrlModes.TabPages(Me.tabctrlModes.SelectedIndex).Tag, eDialogModeType)
        End Sub

        Private Sub OnCreateScenario(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles btnCreate.Click, lvCreate.DoubleClick

            ' Sanity check
            Debug.Assert(Me.m_mode = eDialogModeType.CreateScenario)
            ' Validation
            If Not Me.CanCreateScenario() Then Return
            ' Acutal create does not happen here. This dialog is just the messenger
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnLoadScenario(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles btnLoad.Click, lvLoad.DoubleClick, tsmLoad.Click

            ' Sanity check
            Debug.Assert(Me.m_mode = eDialogModeType.LoadScenario)
            ' Validation
            If Not Me.CanLoadScenario() Then Return
            ' Acutal load does not happen here. This dialog is just the messenger
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnSaveScenarioAs(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles btnSave.Click, lvSaveAs.DoubleClick, tsmSave.Click

            ' Sanity check
            Debug.Assert(Me.m_mode = eDialogModeType.SaveScenario)
            ' Validation
            If Not Me.CanSaveScenario() Then Return
            ' Acutal save does not happen here. This dialog is just the messenger
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        ''' <summary>
        ''' Event handler to delete a EwE scenario.
        ''' </summary>
        Private Sub OnDeleteScenario(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles btnDelete.Click, lvDelete.DoubleClick, tsmDelete.Click

            If Not Me.CanDeleteScenario() Then Return

            Dim scenario As cEwEScenario = Me.Scenario
            Dim strMessage As String = ""

            ' Sanity check
            If Object.ReferenceEquals(scenario, Nothing) Then Return

            ' Ask for confirmation
            strMessage = String.Format(My.Resources.SCENARIO_CONFIRMDELETE_PROMPT, scenario.Name)
            If MsgBox(strMessage, MsgBoxStyle.YesNo Or MsgBoxStyle.Exclamation, My.Resources.SCENARIO_CONFIRMDELETE_CAPTION) <> MsgBoxResult.Yes Then
                Return
            End If

            ' Remove successful?
            If Me.DeleteScenario(scenario) Then
                Me.m_scenario = Nothing
                Me.m_lScenarios = Me.GetAvailableScenarios()
                Me.UpdateScenarioListControls()
            End If

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancelCreate.Click, btnCancelLoad.Click, btnCancelSave.Click, btnCancelDelete.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        ''' <summary>
        ''' Event handler...
        ''' </summary>
        Private Sub OnScenarioCreateNameChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbNameCreate.TextChanged
            Me.Scenario = Me.FindScenarioByName(tbNameCreate.Text)
        End Sub

        ''' <summary>
        ''' Event handler...
        ''' </summary>
        Private Sub OnScenarioSaveAsNameChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbNameSaveAs.TextChanged
            Me.Scenario = Me.FindScenarioByName(tbNameSaveAs.Text)
        End Sub

        Private Sub OnRenameScenario(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles tsmRename.Click

            Dim lv As ListView = Nothing
            Dim lvi As ListViewItem = Nothing

            Select Case Me.m_mode
                Case eDialogModeType.CreateScenario
                    lv = Me.lvCreate
                Case eDialogModeType.SaveScenario
                    lv = Me.lvSaveAs
            End Select

            If (Not Object.ReferenceEquals(lv, Nothing)) Then
                If (lv.SelectedItems.Count = 1) Then
                    lvi = lv.SelectedItems(0)
                    lvi.BeginEdit()
                End If
            End If

        End Sub

        Private Sub OnLVBeforeLabelEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.LabelEditEventArgs) _
                Handles lvCreate.BeforeLabelEdit, lvSaveAs.BeforeLabelEdit

            Dim lv As ListView = DirectCast(sender, ListView)
            Dim lvi As ListViewItem = lv.Items(e.Item)
            Dim scenario As cEwEScenario = DirectCast(lvi.Tag, cEwEScenario)

            e.CancelEdit = (scenario Is Nothing)
        End Sub

        Private Sub OnLVAfterLabelEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.LabelEditEventArgs) _
                Handles lvCreate.AfterLabelEdit, lvSaveAs.AfterLabelEdit

            ' Reject empty names
            If String.IsNullOrEmpty(e.Label) Then
                e.CancelEdit = True
                Return
            End If

            Dim lv As ListView = DirectCast(sender, ListView)
            Dim lvi As ListViewItem = lv.Items(e.Item)
            Dim scenario As cEwEScenario = DirectCast(lvi.Tag, cEwEScenario)

            If (scenario IsNot Nothing) Then
                ' Apply new scenario name
                scenario.Name = e.Label
                Me.UpdateScenarioListControls()
            End If

        End Sub

        Private Sub OnLVColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) _
                Handles lvCreate.ColumnClick, lvDelete.ColumnClick, lvLoad.ColumnClick, lvSaveAs.ColumnClick

            Dim lv As ListView = Nothing
            Dim comparer As ColumnSorter = Nothing

            lv = DirectCast(sender, ListView)
            If (TypeOf lv.ListViewItemSorter Is ColumnSorter) Then
                comparer = DirectCast(lv.ListViewItemSorter, ColumnSorter)
                comparer.Column = DirectCast(e.Column, eColumnTypes)
            End If

            lv.Sort()

        End Sub

        Private Sub OnLVSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles lvCreate.SelectedIndexChanged, lvDelete.SelectedIndexChanged, lvLoad.SelectedIndexChanged, lvSaveAs.SelectedIndexChanged

            Dim lv As ListView = DirectCast(sender, ListView)
            Dim lvi As ListViewItem = Nothing

            If (lv.SelectedItems.Count <> 1) Then
                Me.Scenario = Nothing
            Else
                lvi = lv.SelectedItems(0)
                Me.Scenario = DirectCast(lvi.Tag, cEwEScenario)
            End If

        End Sub

#End Region ' Event handlers

#Region " Properties "

        Private m_bInUpdate As Boolean = False

        Public Overridable Property Scenario() As cEwEScenario
            Get
                Return Me.m_scenario
            End Get
            Set(ByVal scenario As cEwEScenario)
                If Me.m_bInUpdate Then Return

                ' Lock down
                Me.m_bInUpdate = True

                Me.SelectedScenario(Me.lvLoad) = scenario
                Me.SelectedScenario(Me.lvDelete) = scenario
                Me.SelectedScenario(Me.lvSaveAs) = scenario

                Me.m_scenario = scenario

                Me.UpdateControls()

                Me.m_bInUpdate = False

            End Set
        End Property

        Public ReadOnly Property ScenarioName() As String
            Get
                Select Case Me.m_mode
                    Case eDialogModeType.CreateScenario
                        Return Me.tbNameCreate.Text
                    Case eDialogModeType.SaveScenario
                        Return Me.tbNameSaveAs.Text
                End Select
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property ScenarioDescription() As String
            Get
                Select Case Me.m_mode
                    Case eDialogModeType.CreateScenario
                        Return Me.tbDescriptionCreate.Text
                    Case eDialogModeType.SaveScenario
                        Return Me.tbDescriptionSaveAs.Text
                End Select
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property ScenarioAuthor() As String
            Get
                Select Case Me.m_mode
                    Case eDialogModeType.CreateScenario
                        Return Me.tbAuthorCreate.Text
                    Case eDialogModeType.SaveScenario
                        Return Me.tbAuthorSaveAs.Text
                End Select
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property ScenarioContact() As String
            Get
                Select Case Me.m_mode
                    Case eDialogModeType.CreateScenario
                        Return Me.tbContactCreate.Text
                    Case eDialogModeType.SaveScenario
                        Return Me.tbContactSaveAs.Text
                End Select
                Return Nothing
            End Get
        End Property

        Public Property Mode() As eDialogModeType
            Get
                Return Me.m_mode
            End Get
            Set(ByVal value As eDialogModeType)
                Me.SwitchMode(value)
            End Set
        End Property

#End Region ' Properties

    End Class

End Namespace