#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterface.Ecopath.Input
Imports EwEUtils.Core
Imports System.Globalization
Imports System.Threading
Imports System.Windows.Forms
Imports EwEUtils.Commands
Imports System.IO
Imports SourceGrid2.Cells

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' EwEForm that contains an <see cref="EwEGrid">EwEGrid</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmEwEGrid
    : Inherits frmEwE

#Region " Helper classes "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Utility class; adds a toolstrip-contained text box to a form from which 
    ''' all currently selected EwE variables can be modified. Conditions apply.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Private Class QuickEditHandler

#Region " Private variables "

        ''' <summary>The form that this handler is connected to.</summary>
        Private m_form As frmEwEGrid = Nothing
        ''' <summary>The grid whose selection is monitored.</summary>
        Private m_grid As EwEGrid = Nothing
        ''' <summary>The toolstrip that is managed by this handler.</summary>
        Private m_ts As ToolStrip = Nothing
        ''' <summary>Flag stating whether a toolstrip was created by this handler (true), or whether an existing toolstrip was hijacked (false).</summary>
        Private m_bToolStripCreated As Boolean = False
        ''' <summary>The value edit box that is managed by this handler.</summary>
        Private m_ttbValue As ToolStripTextBox = Nothing
        ''' <summary>The edit box label that is managed by this handler.</summary>
        Private m_lblSet As ToolStripLabel = Nothing
        ''' <summary>Set button that is managed by this handler.</summary>
        Private m_btnSet As ToolStripButton = Nothing
        ''' <summary>Flag stating whether handler is attached.</summary>
        Private m_bAttached As Boolean = False

        ' Import Export
        Private m_sep As ToolStripSeparator = Nothing
        Private m_btnImport As ToolStripButton = Nothing
        Private m_btnExport As ToolStripButton = Nothing

#End Region ' Private variables

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <para>Connect the QuickEditHandler to a <see cref="frmEwEGrid">frmEwEGrid</see>.
        ''' Call <see cref="Detach">Detach</see> to disconnect a Quick Edit handler
        ''' from a form it was previously attached to.</para>
        ''' <para>A toolstrip is created if not available, and Quick Edit toolstrip 
        ''' items will be added to the toolstrip.</para>
        ''' </summary>
        ''' <param name="frm">The GridContentPanel to connect to.</param>
        ''' -------------------------------------------------------------------
        Public Sub Attach(ByVal frm As frmEwEGrid)

            Dim ci As CultureInfo = Thread.CurrentThread.CurrentUICulture

            ' Sanity check
            Debug.Assert(frm IsNot Nothing)

            If Me.m_bAttached Then Me.Detach()

            ' Store ref to form
            Me.m_form = frm
            ' Store ref to grid
            Me.m_grid = frm.Grid
            AddHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectioChanged

            ' Init
            Me.m_bToolStripCreated = False

            ' Attempt to find toolstrip
            If Me.m_ts Is Nothing Then
                For Each c As Control In Me.m_form.Controls
                    If TypeOf c Is ToolStrip Then
                        m_ts = DirectCast(c, ToolStrip)
                    End If
                Next

                ' Not found?
                If m_ts Is Nothing Then
                    ' #Yes: create toolstrip
                    Me.m_ts = New ToolStrip()
                    Me.m_ts.Name = "tsQuickEdit"
                    Me.m_form.Controls.Add(Me.m_ts)
                    Me.m_bToolStripCreated = True
                End If
            End If

            Me.m_sep = New ToolStripSeparator()

            ' Create quick edit label
            Me.m_lblSet = New ToolStripLabel(My.Resources.LABEL_SET)

            ' Create quick edit text box
            Me.m_ttbValue = New ToolStripTextBox("tsQuickEdit")
            Me.m_ttbValue.AcceptsReturn = True
            AddHandler Me.m_ttbValue.KeyDown, AddressOf OnTextBoxKeyDown

            ' Create quick edit set button
            Me.m_btnSet = New ToolStripButton(My.Resources.NavForward)
            Me.m_btnSet.ToolTipText = My.Resources.TOOLTIP_GRID_SETVALUE
            AddHandler Me.m_btnSet.Click, AddressOf OnBtnSetClick

            ' Create import button (input grids only)
            If Not frmEwE.IsOutputForm(frm.CoreExecutionState) Then
                Me.m_btnImport = New ToolStripButton(My.Resources.ImportXMLHS)
                Me.m_btnImport.ToolTipText = My.Resources.TOOLTIP_GRID_LOADFROMCSV
                AddHandler Me.m_btnImport.Click, AddressOf OnBtnImportClick
            End If

            ' Create export button
            Me.m_btnExport = New ToolStripButton(My.Resources.ExportXMLHS)
            Me.m_btnExport.ToolTipText = My.Resources.TOOLTIP_GRID_SAVETOCSV
            AddHandler Me.m_btnExport.Click, AddressOf OnBtnExportClick

            ' Add items to the toolstrip
            If (ci.TextInfo.IsRightToLeft) Then
                If (Me.m_btnImport IsNot Nothing) Then
                    Me.m_btnImport.Alignment = ToolStripItemAlignment.Left
                    Me.m_ts.Items.Add(Me.m_btnImport)
                End If
                Me.m_btnExport.Alignment = ToolStripItemAlignment.Left
                Me.m_sep.Alignment = ToolStripItemAlignment.Left
                Me.m_lblSet.Alignment = ToolStripItemAlignment.Left
                Me.m_ttbValue.Alignment = ToolStripItemAlignment.Left
                Me.m_btnSet.Alignment = ToolStripItemAlignment.Left
                Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_btnExport, Me.m_sep, Me.m_lblSet, Me.m_ttbValue, Me.m_btnSet})
            Else
                If (Me.m_btnImport IsNot Nothing) Then
                    Me.m_btnImport.Alignment = ToolStripItemAlignment.Right
                    Me.m_ts.Items.Add(Me.m_btnImport)
                End If
                Me.m_btnExport.Alignment = ToolStripItemAlignment.Right
                Me.m_sep.Alignment = ToolStripItemAlignment.Right
                Me.m_lblSet.Alignment = ToolStripItemAlignment.Right
                Me.m_ttbValue.Alignment = ToolStripItemAlignment.Right
                Me.m_btnSet.Alignment = ToolStripItemAlignment.Right
                Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_btnExport, Me.m_sep, Me.m_btnSet, Me.m_ttbValue, Me.m_lblSet})
            End If

            ' Set attached flag
            Me.m_bAttached = True

            ' Re-align content
            Me.m_form.PerformAutoScale()

            ' Set initial state
            Me.UpdateControls()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <para>Detach the Quick Edit handler from its current form that is was
        ''' previously connected to with the <see cref="Attach">Attach</see> method.</para>
        ''' <para>This will also clean up any toolstrips and toolstrip items
        ''' that an instance created when it was attached to a form.</para>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Detach()

            If Not m_bAttached Then Return

            Me.m_ts.Items.Remove(Me.m_ttbValue)
            Me.m_ts.Items.Remove(Me.m_lblSet)

            If Me.m_bToolStripCreated Then
                Me.m_form.Controls.Remove(Me.m_ts)
                Me.m_ts.Dispose()
                Me.m_bToolStripCreated = False
            End If

            If Me.m_btnImport IsNot Nothing Then
                RemoveHandler Me.m_btnImport.Click, AddressOf OnBtnImportClick
                Me.m_btnImport.Dispose()
                Me.m_btnImport = Nothing
            End If

            RemoveHandler Me.m_btnExport.Click, AddressOf OnBtnExportClick
            Me.m_btnExport.Dispose()
            Me.m_btnExport = Nothing

            RemoveHandler Me.m_ttbValue.KeyDown, AddressOf OnTextBoxKeyDown
            Me.m_ttbValue.Dispose()
            Me.m_ttbValue = Nothing

            RemoveHandler Me.m_btnSet.Click, AddressOf OnBtnSetClick
            Me.m_btnSet.Dispose()
            Me.m_btnSet = Nothing

            RemoveHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectioChanged
            Me.m_grid = Nothing

            Me.m_ts = Nothing

            Me.m_bAttached = False

            ' Re-align content
            Me.m_form.PerformAutoScale()

        End Sub

#End Region ' Public interfaces

#Region " Control events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to an [ENTER] key press to apply entered text
        ''' to the grid selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnTextBoxKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
            ' Is [ENTER]?
            If e.KeyCode = Keys.Enter Then Me.ApplyValueToSelection(Me.m_ttbValue.Text)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to Set button press to apply entered text
        ''' to the grid selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnBtnSetClick(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.ApplyValueToSelection(Me.m_ttbValue.Text)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to Import button press to import grid content
        ''' from a CSV file.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnBtnImportClick(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.ImportFromCSV()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to Export button press to export grid content
        ''' to a CSV file.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnBtnExportClick(ByVal sender As Object, ByVal e As System.EventArgs)
            Me.ExportToCSV()
        End Sub

#End Region ' Control events

#Region " Grid events "

        Private Sub OnGridSelectioChanged(ByVal cells As SourceGrid2.CellVirtualCollection)
            Me.UpdateControls()
        End Sub

#End Region ' Command events

#Region " Internal implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the state of the edit box based on the content of a grid selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub UpdateControls()

            If Me.m_form Is Nothing Then Return

            Dim sel As SourceGrid2.Selection = Me.m_grid.Selection
            Dim bIsInputForm As Boolean = False
            Dim bHasEditableCells As Boolean = False
            ' Flag stating that the selection contains a mix of variable names
            Dim bIsMixedSelection As Boolean = False
            ' The last found variable name
            Dim vn As eVarNameFlags = eVarNameFlags.NotSet

            ' Flag stating that the selection contains different values
            Dim bIsMixedValue As Boolean = False
            Dim objValue As Object = Nothing

            bIsInputForm = frmEwE.IsInputForm(Me.m_form.CoreExecutionState)

            ' Iterate through cells
            For Each cell As SourceGrid2.Cells.ICell In sel.GetCells()
                ' Is this cell editable?
                If cell.DataModel.EnableEdit Then
                    ' #Yes: explore the variable this cell represents by checking an attached property
                    ' Is a property cell?
                    If TypeOf cell Is PropertyCell Then
                        ' #Yes: get the property
                        Dim p As cProperty = DirectCast(cell, PropertyCell).GetProperty()
                        ' Does this property refer to a variable other than found earlier?
                        If ((vn <> eVarNameFlags.NotSet) And (p.VarName <> vn)) Then
                            ' #Yes: this is a mixed selection.
                            bIsMixedSelection = True
                        End If

                        ' Does this property hold a value other than found earlier?
                        If (objValue IsNot Nothing) Then
                            If (Not objValue.Equals(p.GetValue())) Then
                                ' #Yes: this is mixed value
                                bIsMixedValue = True
                            End If
                        End If

                        ' Update varname
                        vn = p.VarName
                        ' Update value
                        objValue = p.GetValue()
                    End If
                    ' There was at least one editable cell
                    bHasEditableCells = True
                End If
            Next

            ' Enable set label if the grid has editable cells that represent only one type of variable.
            If Not Object.ReferenceEquals(Me.m_lblSet, Nothing) Then
                Me.m_lblSet.Enabled = bHasEditableCells And Not bIsMixedSelection
                Me.m_lblSet.Visible = bIsInputForm
            End If

            ' Enable edit control if the grid has editable cells that represent only one type of variable.
            If Not Object.ReferenceEquals(Me.m_ttbValue, Nothing) Then
                Me.m_ttbValue.Enabled = bHasEditableCells And Not bIsMixedSelection
                Me.m_ttbValue.Visible = bIsInputForm
                Me.m_ttbValue.Text = ""
                If ((objValue IsNot Nothing) And (bIsMixedValue = False)) Then
                    If TypeOf objValue Is String Then
                        Me.m_ttbValue.Text = CStr(objValue)
                    ElseIf (TypeOf objValue Is Single) Or (TypeOf objValue Is Double) Or (TypeOf objValue Is Integer) Then
                        Try
                            Me.m_ttbValue.Text = cStyleGuide.GetInstance().FormatNumber(CSng(objValue))
                        Catch ex As Exception
                        End Try
                    ElseIf TypeOf objValue Is Boolean Then
                        Me.m_ttbValue.Text = CStr(IIf(CBool(objValue) = True, "1", "0"))
                    End If
                End If
            End If

            ' Enable set button if the grid has editable cells that represent only one type of variable.
            If Not Object.ReferenceEquals(Me.m_btnSet, Nothing) Then
                Me.m_btnSet.Enabled = bHasEditableCells And Not bIsMixedSelection
                Me.m_btnSet.Visible = bIsInputForm
            End If

            ' Enable import button only for input forms
            If Not Object.ReferenceEquals(Me.m_btnImport, Nothing) Then
                Me.m_btnImport.Visible = bIsInputForm
            End If

            cToolstripUtils.HideRepeatingSeparators(Me.m_ts)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Apply a string to all selected cells that are 
        ''' <see cref="SourceGrid2.DataModels.DataModelBase.EnableEdit">Editable</see>.
        ''' </summary>
        ''' <param name="strValue">The value to apply.</param>
        ''' -------------------------------------------------------------------
        Private Sub ApplyValueToSelection(ByVal strValue As String)

            ' Get grid selection
            Dim sel As SourceGrid2.Selection = Me.m_grid.Selection
            Dim core As cCore = cCore.GetInstance()
            Dim appl As AppLauncher = AppLauncher.GetInstance()

            If Not core.SetBatchLock(cCore.eBatchLockType.Update) Then Return

            appl.SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)
            For Each cell As SourceGrid2.Cells.ICell In sel.GetCells()
                If TypeOf cell Is PropertyCell Then
                    Dim pcell As PropertyCell = DirectCast(cell, PropertyCell)
                    If (pcell.Style And cStyleGuide.eStyleFlags.NotEditable) = 0 Then
                        pcell.GetProperty().SetValue(strValue)
                    End If
                Else
                    If cell.DataModel.EnableEdit Then
                        cell.Value = strValue
                    End If
                End If
            Next
            appl.SetStatusText("", TriState.False)

            core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.NotSet)

        End Sub

        Private Function GetCSVFileName() As String
            Return Me.m_form.Text
        End Function

        Private Sub ImportFromCSV()

            Dim cmdh As cCommandHandler = cCommandHandler.getinstance()
            Dim cmdOF As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)
            Dim fs As Stream = Nothing
            Dim sr As StreamReader = Nothing

            cmdOF.Invoke(Me.GetCSVFileName(), ".\", My.Resources.FILEFILTER_CSV)
            If cmdOF.Result <> Windows.Forms.DialogResult.OK Then Return

            Try
                fs = New FileStream(cmdOF.FileName, _
                                    FileMode.Open, _
                                    FileAccess.Read, _
                                    FileShare.ReadWrite Or FileShare.Delete Or FileShare.Inheritable)
            Catch ex As Exception
                Return
            End Try

            sr = New StreamReader(fs)
            Me.m_grid.ReadContent(sr)
            sr.Close()
            fs.Close()

        End Sub

        Private Sub ExportToCSV()

            Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
            Dim cmdSF As cFileSaveCommand = DirectCast(cmdh.GetCommand(cFileSaveCommand.COMMAND_NAME), cFileSaveCommand)
            Dim fs As Stream = Nothing
            Dim sw As StreamWriter = Nothing

            cmdSF.Invoke(Me.GetCSVFileName(), ".\", My.Resources.FILEFILTER_CSV)
            If cmdSF.Result <> Windows.Forms.DialogResult.OK Then Return

            Try
                'Create the file
                fs = New FileStream(cmdSF.FileName, FileMode.Create, FileAccess.Write, FileShare.None)
            Catch ex As Exception
                ' Woops! Send message?
                Return
            End Try
            sw = New StreamWriter(fs)
            Me.m_grid.WriteContent(sw)
            sw.Close()
            fs.Close()

        End Sub

#End Region 'Internal implementation

    End Class

#End Region ' Helper classes

#Region " Variables "

    ''' <summary>The grid in this form.</summary>
    Private m_grid As EwEGrid = Nothing
    ''' <summary><see cref="QuickEditHandler">Quick Edit Handler</see> for this form.</summary>
    Private m_qeHandler As QuickEditHandler = Nothing

#End Region ' Variables

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Default constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        MyBase.New(My.Resources.HEADER_EMPTY_PANEL)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fancy constructor.
    ''' </summary>
    ''' <param name="strText">Caption and tab text for this form.</param>
    ''' <param name="grid">The grid that this form contains.</param>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Sub New(ByVal strText As String, ByVal grid As EwEGrid)

        MyBase.New(strText)

        Debug.Assert(grid IsNot Nothing)

        Me.m_grid = grid
        Me.m_grid.Dock = DockStyle.Fill
        Me.Controls.Add(m_grid)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a persistent name for instances of this class. Instances are 
    ''' identified by the class name of the attached grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function GetPersistString() As String

        ' Has a grid?
        If Me.m_grid IsNot Nothing Then
            ' #Yes: return grid class name
            Return Me.m_grid.GetType().ToString()
        Else
            ' #No: return the default persistent string
            Return MyBase.GetPersistString()
        End If

    End Function

#End Region ' Constructors

#Region " Obligatory overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to pass the message to the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)
        Me.m_grid.OnCoreMessage(msg)
    End Sub

#End Region ' Obligatory overrides

#Region " Form overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; handles the Load event to finalize this form for usage.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)

        MyBase.OnLoad(e)

        ' Designer crap
        If (Me.m_grid Is Nothing) Then Return
        ' Connect to message sources
        Me.CoreComponents = Me.m_grid.MessageSources

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; handles the Disposed event to clear this form after usage.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        ' Release any quick edit handler
        Me.SetQuickEditHandler(False)
        ' Clear any message source links
        Me.CoreComponents = Nothing
        ' Kill the grid
        If (Me.m_grid IsNot Nothing) Then
            Me.m_grid.Dispose()
            Me.m_grid = Nothing
        End If

        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Form overrides

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set core execution state for the form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Overrides Property CoreExecutionState() As eCoreExecutionState

        Get
            Return MyBase.CoreExecutionState
        End Get

        Set(ByVal value As eCoreExecutionState)
            MyBase.CoreExecutionState = value
            ' Use a quick edit handler on all grids
            ' JS 05Sep09: QEbar was Input grid only. Now, CSV interaction is available for all grids
            Me.SetQuickEditHandler(True)
        End Set

    End Property

#End Region ' Public interfaces

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a reference to the Grid
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Protected Function Grid() As EwEGrid
        Return m_grid
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set or removes a grid quick edit handler.
    ''' </summary>
    ''' <param name="bSet">Flag stating whether the q.e.handler should be set
    ''' (true) or released (false).</param>
    ''' <remarks>
    ''' This code is pretty robust, do not worry about calling it too much.
    ''' Note that it's important to release any handler when the form 
    ''' gets destroyed.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub SetQuickEditHandler(ByVal bSet As Boolean)
        If bSet Then
            If (Me.m_qeHandler Is Nothing) Then
                Me.m_qeHandler = New QuickEditHandler()
                Me.m_qeHandler.Attach(Me)
            End If
        Else
            If (Me.m_qeHandler IsNot Nothing) Then
                Me.m_qeHandler.Detach()
                Me.m_qeHandler = Nothing
            End If
        End If
    End Sub

#End Region ' Internals

End Class