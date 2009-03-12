'==============================================================================
'
' $Log: frmEwEGrid.vb,v $
' Revision 1.4  2009/03/12 14:10:30  jeroens
' Core message rerouted to the grid
'
' Revision 1.3  2009/02/05 17:48:41  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.2  2008/12/15 15:55:33  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:32:08  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterface.Ecopath.Input
Imports EwEUtils.Core
Imports System.Globalization
Imports System.Threading

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
        Private WithEvents m_grid As EwEGrid = Nothing
        ''' <summary>The toolstrip that is managed by this handler.</summary>
        Private m_ts As ToolStrip = Nothing
        ''' <summary>Flag stating whether a toolstrip was created by this handler (true), or whether an existing toolstrip was hijacked (false).</summary>
        Private m_bToolStripCreated As Boolean = False
        ''' <summary>The value edit box that is managed by this handler.</summary>
        Private WithEvents m_ttbValue As ToolStripTextBox = Nothing
        ''' <summary>The edit box label that is managed by this handler.</summary>
        Private WithEvents m_lblSet As ToolStripLabel = Nothing
        ''' <summary>Set button that is managed by this handler.</summary>
        Private WithEvents m_btnSet As ToolStripButton = Nothing
        ''' <summary>Flag stating whether handler is attached.</summary>
        Private m_bAttached As Boolean = False

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

            ' Create quick edit label
            Me.m_lblSet = New ToolStripLabel(My.Resources.LABEL_SET)
            ' Create quick edit text box
            Me.m_ttbValue = New ToolStripTextBox("tsQuickEdit")
            Me.m_ttbValue.AcceptsReturn = True
            ' Create quick edit set button
            Me.m_btnSet = New ToolStripButton(My.Resources.NavForward)
            ' Add items to the toolstrip
            If (ci.TextInfo.IsRightToLeft) Then
                Me.m_lblSet.Alignment = ToolStripItemAlignment.Left
                Me.m_ttbValue.Alignment = ToolStripItemAlignment.Left
                Me.m_btnSet.Alignment = ToolStripItemAlignment.Left
                Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_lblSet, Me.m_ttbValue, Me.m_btnSet})
            Else
                Me.m_lblSet.Alignment = ToolStripItemAlignment.Right
                Me.m_ttbValue.Alignment = ToolStripItemAlignment.Right
                Me.m_btnSet.Alignment = ToolStripItemAlignment.Right
                Me.m_ts.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_btnSet, Me.m_ttbValue, Me.m_lblSet})
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

            Me.m_ts = Nothing
            Me.m_ttbValue = Nothing
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
        Private Sub OnTextBoxKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles m_ttbValue.KeyDown
            ' Is [ENTER]?
            If e.KeyCode = Keys.Enter Then Me.ApplyValueToSelection(Me.m_ttbValue.Text)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; responds to Set button press to apply entered text
        ''' to the grid selection.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnBtnSetClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_btnSet.Click
            Me.ApplyValueToSelection(Me.m_ttbValue.Text)
        End Sub

#End Region ' Control events

#Region " Grid events "

        Private Sub OnGridSelectioChanged(ByVal cells As SourceGrid2.CellVirtualCollection) Handles m_grid.OnSelectionChanged
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

            Dim sel As SourceGrid2.Selection = Me.m_grid.Selection
            ' Flag stating that the selection is not empty
            Dim bHasEditableCells As Boolean = False

            ' Flag stating that the selection contains a mix of variable names
            Dim bIsMixedSelection As Boolean = False
            ' The last found variable name
            Dim vn As eVarNameFlags = eVarNameFlags.NotSet

            ' Flag stating that the selection contains different values
            Dim bIsMixedValue As Boolean = False
            Dim objValue As Object = Nothing

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
            End If

            ' Enable edit control if the grid has editable cells that represent only one type of variable.
            If Not Object.ReferenceEquals(Me.m_ttbValue, Nothing) Then
                Me.m_ttbValue.Enabled = bHasEditableCells And Not bIsMixedSelection
                Me.m_ttbValue.Text = ""
                If ((objValue IsNot Nothing) And (bIsMixedValue = False)) Then
                    If TypeOf objValue Is String Then
                        Me.m_ttbValue.Text = CStr(objValue)
                    ElseIf (TypeOf objValue Is Single) Or (TypeOf objValue Is Double) Or (TypeOf objValue Is Integer) Then
                        Try
                            Me.m_ttbValue.Text = StyleGuide.GetInstance().FormatNumber(CSng(objValue))
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
            End If

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
                    If (pcell.Style And StyleGuide.eStyleFlags.NotEditable) = 0 Then
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

#Region " Form event handlers "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; handles the Load event to finalize this form for usage.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnGridFormLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
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
    Private Sub OnGridFormDisposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        ' Release any quick edit handler
        Me.SetQuickEditHandler(False)
        ' Clear any message source links
        Me.CoreComponents = Nothing
    End Sub

#End Region ' Form event handlers

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set Input state flag.
    ''' </summary>
    ''' <remarks>
    ''' EwE Input grids all have a Quick Edit bar. This method will ensure the
    ''' bar is there for input grids, and is missing for output grids.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Overrides Property CoreExecutionState() As eCoreExecutionState

        Get
            Return MyBase.CoreExecutionState
        End Get

        Set(ByVal value As eCoreExecutionState)
            MyBase.CoreExecutionState = value
            ' Use a quick edit handler on input grids
            Me.SetQuickEditHandler(frmEwE.IsInputForm(value))
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