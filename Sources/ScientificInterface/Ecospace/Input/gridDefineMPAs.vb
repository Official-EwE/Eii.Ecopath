' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Core

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridEditMPA
        : Inherits EwEGrid

        ''' <summary>A number representing the row that contains the first MPA</summary>
        Private Const iFIRSTMPAROW As Integer = 1

        ''' <summary>List of active MPAs.</summary>
        Private m_mpas As New List(Of cMPAInfo)
        ''' <summary>List of removed MPAs.</summary>
        Private m_mpasRemoved As New List(Of cMPAInfo)

        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        ''' <summary>Enumerated type defining the columns in this grid.</summary>
        Private Enum eColumnTypes
            Index = 0
            Name
            Status
        End Enum

#Region " Helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Administrative unit representing a <see cref="cEcospaceMPA">MPA</see>
        ''' in the EwE model.
        ''' </summary>
        ''' <remarks>
        ''' This class can represent existing and new MPAs.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Class cMPAInfo

            ''' <summary>The status of a MPA in the interface.</summary>
            Private m_status As eItemStatusTypes = eItemStatusTypes.Original

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="MPA">The <see cref="cEcospaceMPA">cEcospaceMPA</see> to
            ''' initialize this instance from. If set, this instance represents a
            ''' MPA currently active in the EwE model.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal MPA As cEcospaceMPA)
                Debug.Assert(MPA IsNot Nothing)
                Me.DBID = MPA.DBID
                Me.Index = MPA.Index
                Me.Name = MPA.Name
                Me.Status = eItemStatusTypes.Original
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="strName">Name to assign to this administrative unit.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal strName As String)
                Me.Name = strName
                Me.Status = eItemStatusTypes.Added
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set the name of this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Name() As String = ""

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="cEcospaceMPA.DBID"/> of an associated MPA, if any.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property DBID As Integer

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="cEcospaceMPA.Index"/> of an associated MPA, if any.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Index As Integer

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="eItemStatusTypes">item status</see> for the MPA 
            ''' object.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Status As eItemStatusTypes
                Get
                    Return Me.m_status
                End Get
                Private Set(value As eItemStatusTypes)
                    Me.m_status = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether the user has confirmed an action on this object.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Confirmed As Boolean = False

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' States whether the MPA has changed.
            ''' </summary>
            ''' <returns>
            ''' True when MPA <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsChanged(ByVal mpa As cEcospaceMPA) As Boolean
                If Me.IsNew Then Return False
                If (mpa.DBID <> Me.DBID) Then Return False
                Return (mpa.Name <> Me.Name)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' States whether the MPA is to be created.
            ''' </summary>
            ''' <returns>
            ''' True when MPA <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsNew() As Boolean
                Return (Me.DBID <= 0)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this MPA is flagged for deletion. Toggling this flag
            ''' will update the <see cref="Status">Status</see> of the item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property FlaggedForDeletion() As Boolean
                Get
                    Return Me.m_status = eItemStatusTypes.Removed
                End Get
                Set(ByVal bDelete As Boolean)
                    If Not Me.IsNew() Then
                        If bDelete Then
                            Me.Status = eItemStatusTypes.Removed
                        Else
                            Me.Status = eItemStatusTypes.Original
                        End If
                    Else
                        If bDelete Then
                            Me.Status = eItemStatusTypes.Invalid
                        Else
                            Me.Status = eItemStatusTypes.Added
                        End If
                    End If
                End Set
            End Property

        End Class

#End Region ' Helper classes

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create the grid
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

#Region " Grid interaction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the grid.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Selection.EnableMultiSelection = False

            Me.ContextMenu = Nothing

            ' Redim columns
            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell()
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_MPA)
            Me(0, eColumnTypes.Status) = New EwEColumnHeaderCell(SharedResources.HEADER_STATUS)

            ' Fix index column only; MPA name column cannot be fixed because it must be editable
            Me.FixedColumns = 1

            Me.Columns(eColumnTypes.Index).AutoSizeMode = SourceGrid2.AutoSizeMode.None
            Me.Columns(eColumnTypes.Name).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch
            Me.Columns(eColumnTypes.Status).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.AutoStretchColumnsToFitWidth = True

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to first create a snapshot of the MPA/stanza configuration
        ''' in the current EwE model. The grid will be populated from this local
        ''' administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FillData()

            Dim MPA As cEcospaceMPA = Nothing
            Dim mi As cMPAInfo = Nothing

            ' Populate local administration from a snapshot of the live data

            ' Make snapshot of MPA configuration 
            For iMPA As Integer = 1 To Me.Core.nMPAs
                MPA = Me.Core.EcospaceMPAs(iMPA)
                mi = New cMPAInfo(MPA)
                Me.m_mpas.Add(mi)
            Next

            ' Brute-force update grid
            UpdateGrid()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Brute-force resize the gird if necessary, and repopulate with data from 
        ''' the local administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub UpdateGrid()

            Dim mi As cMPAInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim cells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim ewec As EwECell = Nothing

            ' Create missing rows
            For iRow As Integer = Me.Rows.Count To Me.m_mpas.Count
                Me.AddRow()

                ewec = New EwECell(0, GetType(Integer))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.Index) = ewec

                Me(iRow, eColumnTypes.Name) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.Name).Behaviors.Add(Me.EwEEditHandler)

                Me(iRow, eColumnTypes.Status) = New EwEStatusCell(eItemStatusTypes.Original)
            Next

            ' Delete obsolete rows
            While Me.Rows.Count > Me.m_mpas.Count + 1
                Me.Rows.Remove(Me.Rows.Count - iFIRSTMPAROW)
            End While

            ' Sanity check whether grid can accomodate all MPAs + header
            Debug.Assert(Me.Rows.Count = Me.m_mpas.Count + 1)

            ' Populate rows
            For iRow As Integer = 1 To Me.m_mpas.Count
                UpdateRow(iRow)
            Next iRow

            Me.StretchColumnsToFitWidth()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the content of the Row with the given index.
        ''' </summary>
        ''' <param name="iRow">The index of the row to refresh.</param>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateRow(ByVal iRow As Integer)

            Dim mi As cMPAInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim aCells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing

            Me.AllowUpdates = False

            mi = DirectCast(Me.m_mpas(iRow - iFIRSTMPAROW), cMPAInfo)
            ri = Me.Rows(iRow)

            ri.Tag = mi
            aCells = ri.GetCells()

            pos = New Position(iRow, eColumnTypes.Index)
            aCells(eColumnTypes.Index).SetValue(pos, CInt(iRow))

            ' Set name
            pos = New Position(iRow, eColumnTypes.Name)
            aCells(eColumnTypes.Name).SetValue(pos, CStr(mi.Name))

            aCells(eColumnTypes.Status).SetValue(pos, mi.Status)

            Me.AllowUpdates = True

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Called when the user has finished editing a cell. Handled to update 
        ''' local admin based on cell value changes.
        ''' </summary>
        ''' <returns>
        ''' True if the edit operation is allowed, False to cancel the edit operation.
        ''' </returns>
        ''' <remarks>
        ''' This method differs from OnCellValueChanged; at the end of an edit
        ''' operation it is once again safe to alter the value of the cell that was
        ''' just edited for text and combo box controls. *sigh*
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Protected Overrides Function OnCellEdited(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

            If Not Me.AllowUpdates Then Return True

            Dim mi As cMPAInfo = DirectCast(Me.m_mpas(p.Row - 1), cMPAInfo)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.Index
                    ' Not possible

                Case eColumnTypes.Name
                    Dim strName As String = CStr(cell.GetValue(p))
                    ' Check if name is unique
                    For iMPA As Integer = 0 To Me.m_mpas.Count - 1
                        Dim giTemp As cMPAInfo = DirectCast(Me.m_mpas(iMPA), cMPAInfo)
                        ' Does name already exist?
                        If (Not ReferenceEquals(giTemp, mi)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                            ' Change is not allowed
                            Me.UpdateRow(p.Row)
                            ' Report failure
                            Return False
                        End If
                    Next
                    ' Allow name change
                    mi.Name = strName

            End Select

            Return True

        End Function

        '''' -----------------------------------------------------------------------
        '''' <summary>
        '''' Cell click handler, called in response to clicking button-like cells.
        '''' </summary>
        '''' -----------------------------------------------------------------------
        'Protected Overrides Sub OnCellClicked(ByVal p As Position, ByVal cell As Cells.ICellVirtual)

        '    Select Case DirectCast(p.Column, eColumnTypes)
        '    End Select

        'End Sub

#End Region ' Grid interaction

#Region " Row manipulation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Delete a row from the grid
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub ToggleDeleteRow()

            For Each iRow As Integer In Me.SelectedRows

                Dim iMPA As Integer = iRow - iFIRSTMPAROW
                Dim mi As cMPAInfo = Nothing

                mi = DirectCast(Me.m_mpas(iMPA), cMPAInfo)
                ' Toggle 'flagged for deletion' flag
                mi.FlaggedForDeletion = Not mi.FlaggedForDeletion

                ' Check to see what is to happen to the MPA now
                Select Case mi.Status

                    Case eItemStatusTypes.Original
                        ' Clear removed status of the MPA
                        Me.m_mpasRemoved.Remove(Me.m_mpas(iMPA))

                    Case eItemStatusTypes.Added
                        ' Clear removed status of the MPA
                        Me.m_mpasRemoved.Remove(Me.m_mpas(iMPA))

                    Case eItemStatusTypes.Removed
                        ' Set removed status
                        Me.m_mpasRemoved.Add(Me.m_mpas(iMPA))

                    Case eItemStatusTypes.Invalid
                        ' Set removed status
                        Me.m_mpas.RemoveAt(iMPA)

                End Select
            Next

            Me.UpdateGrid()

        End Sub

        ''' <summary>
        ''' States whether a row holds a MPA.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <returns></returns>
        Public Function IsMPARow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (iRow >= iFIRSTMPAROW) And (iRow < Me.RowsCount)
        End Function

        ''' <summary>
        ''' States whether the MPA on a row is flagged for deletion.
        ''' </summary>
        Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not IsMPARow(iRow) Then Return False

            Dim iMPA As Integer = iRow - iFIRSTMPAROW
            Dim mi As cMPAInfo = DirectCast(Me.m_mpas(iMPA), cMPAInfo)

            Return mi.FlaggedForDeletion
        End Function

        ''' <summary>
        ''' Add a row by creating a new MPA.
        ''' </summary>
        Public Sub InsertRow()
            If Not Me.CanAddRow() Then Return
            Me.CreateMPA()
        End Sub

        ''' <summary>
        ''' Create a new MPA.
        ''' </summary>
        Private Sub CreateMPA()
            Dim iRow As Integer = -1
            Dim iMPA As Integer = -1
            Dim mi As cMPAInfo = Nothing
            Dim mpas As New List(Of String)

            ' Make fit
            iRow = Math.Max(iFIRSTMPAROW, Me.RowsCount)
            iMPA = iRow - iFIRSTMPAROW

            ' Validate
            If iMPA < 0 Then Return

            ' Collect all current MPA names
            For Each mi In Me.m_mpas
                mpas.Add(mi.Name)
            Next

            mi = New cMPAInfo(cStringUtils.Localize(SharedResources.DEFAULT_NEWMPA_NUM,
                    cStringUtils.GetNextNumber(mpas.ToArray(), SharedResources.DEFAULT_NEWMPA_NUM)))
            Me.m_mpas.Insert(iMPA, mi)

            Me.UpdateGrid()
            Me.SelectRow(mi)
        End Sub

        ''' <summary>
        ''' States whether a row can be inserted at the indicated position.
        ''' </summary>
        Public Function CanAddRow() As Boolean
            Return True
        End Function

        ''' <summary>
        ''' Move row up, switching positions with the row above it.
        ''' </summary>
        Public Sub MoveRowUp(Optional ByVal iRow As Integer = -1)
            Dim bMoveSelection As Boolean = (iRow = -1)

            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not CanMoveRowUp(iRow) Then Return
            Me.MoveRow(iRow, iRow - 1)

            If bMoveSelection Then
                Me.SelectRow(iRow - 1)
            End If
        End Sub

        ''' <summary>
        ''' States whether a row can be moved up.
        ''' </summary>
        Public Function CanMoveRowUp(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (Me.RowsCount > (iFIRSTMPAROW + 1)) And (iRow > iFIRSTMPAROW)
        End Function

        ''' <summary>
        ''' Move row down, switching positions with the row below it.
        ''' </summary>
        Public Sub MoveRowDown(Optional ByVal iRow As Integer = -1)
            Dim bMoveSelection As Boolean = (iRow = -1)

            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not CanMoveRowDown(iRow) Then Return
            Me.MoveRow(iRow, iRow + 1)

            If bMoveSelection Then
                Me.SelectRow(iRow + 1)
            End If
        End Sub

        ''' <summary>
        ''' States whether a row can be moved down.
        ''' </summary>
        Public Function CanMoveRowDown(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (Me.RowsCount > (iFIRSTMPAROW + 1)) And (iRow >= iFIRSTMPAROW) And (iRow < Me.RowsCount - 1)
        End Function

        ''' <summary>
        ''' Move one row to another position.
        ''' </summary>
        Private Sub MoveRow(ByVal iFromRow As Integer, ByVal iToRow As Integer)

            Dim t As cMPAInfo = Nothing
            Dim iStep As Integer = 1
            Dim iFrom As Integer = iFromRow - iFIRSTMPAROW
            Dim iTo As Integer = iToRow - iFIRSTMPAROW

            ' Truncate
            iFrom = Math.Max(0, Math.Min(Me.m_mpas.Count - 1, iFrom))
            iTo = Math.Max(0, Math.Min(Me.m_mpas.Count - 1, iTo))

            ' Nothing to do? abort
            If iFrom = iTo Then Return
            ' Determine direction of movement
            If iFrom < iTo Then iStep = 1 Else iStep = -1

            ' Swap  
            For i As Integer = iFrom To iTo - iStep Step iStep
                t = Me.m_mpas(i + iStep)
                Me.m_mpas(i + iStep) = Me.m_mpas(i)
                Me.m_mpas(i) = t
                Me.UpdateRow(i + iFIRSTMPAROW)
                Me.UpdateRow(i + iFIRSTMPAROW + iStep)
            Next i

        End Sub

#End Region ' Row manipulation 

#Region " Admin "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update lock, should be set when modifying cell values from the code
        ''' to prevent recursive update/notification loops.
        ''' </summary>
        ''' <returns>True when no update lock is active.</returns>
        ''' <remarks>
        ''' Update locks are cumulative: setting this lock twice will require 
        ''' clearing it twice to allow updates to happen.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Property AllowUpdates() As Boolean
            Get
                Return (Me.m_iUpdateLock = 0)
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    Me.m_iUpdateLock += 1
                Else
                    Me.m_iUpdateLock -= 1
                End If
            End Set
        End Property

#Region " Selection extension "

        Private Overloads Sub SelectRow(ByVal mi As cMPAInfo)
            For iMPA As Integer = 0 To Me.m_mpas.Count - 1
                If ReferenceEquals(Me.m_mpas(iMPA), mi) Then
                    Me.SelectRow(iMPA + iFIRSTMPAROW)
                End If
            Next
        End Sub

#End Region ' Selection extension

#End Region ' Admin

#Region " Validation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; validates the content of the grid.
        ''' </summary>
        ''' <returns>True when the content of the grid depicts a valid
        ''' Habitat configuration for a model.</returns>
        ''' -----------------------------------------------------------------------
        Public Function ValidateContent() As Boolean

            Return Me.ValidateNames

        End Function

        Private Function ValidateNames() As Boolean

            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_DUPLICATE_NAMES, eCoreComponentType.External, eMessageType.DataValidation, eMessageImportance.Question, eMessageReplyStyle.YES_NO, eDataTypes.NotSet, eMessageReply.NO)
            Dim bHasDuplicates As Boolean = False
            Dim bHasBlank As Boolean = False
            Dim handled As New List(Of String)

            For Each hi As cMPAInfo In Me.m_mpas
                If String.IsNullOrEmpty(hi.Name) Then
                    bHasBlank = True
                ElseIf Not Me.IsNameUnique(hi.Name, hi) Then
                    If Not handled.Contains(hi.Name) Then
                        fmsg.AddVariable(New cVariableStatus(eStatusFlags.FailedValidation,
                                                             cStringUtils.Localize(My.Resources.PROMPT_DUPLICATE_NAME, hi.Name),
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, cCore.NULL_VALUE))
                        handled.Add(hi.Name)
                    End If
                    bHasDuplicates = True
                End If
            Next

            If bHasBlank Then
                Me.Core.Messages.SendMessage(New cMessage(My.Resources.PROMPT_EMPTY_NAMES, eMessageType.DataValidation, eCoreComponentType.External, eMessageImportance.Warning))
                Return False
            End If

            If bHasDuplicates Then
                Me.Core.Messages.SendMessage(fmsg)
                Return fmsg.Reply = eMessageReply.YES
            End If

            Return True

        End Function

        Private Function IsNameUnique(ByVal strName As String, ByVal info As cMPAInfo) As Boolean

            ' Check if name is unique
            For i As Integer = 0 To Me.m_mpas.Count - 1
                Dim infoTmp As cMPAInfo = DirectCast(Me.m_mpas(i), cMPAInfo)
                ' Only compare new items
                If (infoTmp.Status <> eItemStatusTypes.Removed And info.Status <> eItemStatusTypes.Removed) Then
                    ' Does name already exist?
                    If (Not ReferenceEquals(infoTmp, info)) And (String.Compare(strName, infoTmp.Name, True) = 0) Then
                        ' Report failure
                        Return False
                    End If
                End If
            Next
            Return True

        End Function

#End Region ' Validation

#Region " Apply changes "

        Public Function Apply() As Boolean

            Dim strPrompt As String = ""
            Dim bConfigurationChanged As Boolean = False
            Dim bMPAsChanged As Boolean = False
            Dim info As cMPAInfo = Nothing
            Dim iDBID As Integer = Nothing
            Dim mpa As cEcospaceMPA = Nothing
            Dim i As Integer = 0
            Dim MPAMonths(cCore.N_MONTHS) As Boolean
            Dim bSuccess As Boolean = True

            For i = 1 To cCore.N_MONTHS
                MPAMonths(i) = False
            Next

            ' Validate content of the grid
            If Not Me.ValidateContent() Then Return False

            ' Assess MPA changes
            For i = 0 To Me.m_mpas.Count - 1
                info = DirectCast(Me.m_mpas(i), cMPAInfo)

                If info.IsNew Then
                    bConfigurationChanged = True
                Else
                    If ((i + 1) <> info.Index) Then bConfigurationChanged = True
                    bMPAsChanged = bMPAsChanged Or info.IsChanged(Me.Core.EcospaceMPAs(info.Index))
                End If
            Next i

            If (Me.m_mpasRemoved.Count > 1) Then

                strPrompt = cStringUtils.Localize(My.Resources.ECOSPACE_EDITMPA_CONFIRMDELETENUM_PROMPT, Me.m_mpasRemoved.Count)
                Dim fmsg As New cFeedbackMessage(strPrompt, eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO_CANCEL)
                Me.UIContext.Core.Messages.SendMessage(fmsg)

                Select Case fmsg.Reply
                    Case eMessageReply.CANCEL
                        ' Abort Apply process
                        Return False
                    Case eMessageReply.YES
                        ' Confirm all regions
                        For Each info In Me.m_mpasRemoved
                            info.Confirmed = True
                        Next
                        bConfigurationChanged = True
                    Case eMessageReply.NO
                        ' NOP
                    Case Else
                        ' Unexpected answer: assert
                        Debug.Assert(False)
                End Select

            Else
                ' Assess MPAs to remove
                For i = 0 To Me.m_mpasRemoved.Count - 1
                    info = Me.m_mpasRemoved(i)
                    If (Not info.IsNew()) Then

                        strPrompt = cStringUtils.Localize(My.Resources.ECOSPACE_EDITHABITAT_CONFIRMDELETE_PROMPT, info.Name)
                        Dim fmsg As New cFeedbackMessage(strPrompt, eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO_CANCEL)
                        Me.UIContext.Core.Messages.SendMessage(fmsg)

                        Select Case fmsg.Reply
                            Case eMessageReply.CANCEL
                                ' Abort Apply process
                                Return False
                            Case eMessageReply.NO
                                ' Do not delete this Habitat
                                info.Confirmed = False
                            Case eMessageReply.YES
                                ' Delete this Habitat
                                info.Confirmed = True
                                bConfigurationChanged = True
                            Case Else
                                ' Unexpected answer: assert
                                Debug.Assert(False)
                        End Select

                    End If
                Next i
            End If

            ' Handle added and removed items
            If (bConfigurationChanged) Then

                If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False
                cApplicationStatusNotifier.StartProgress(Me.Core, SharedResources.GENERIC_STATUS_APPLYCHANGES)

                ' Add new MPAs
                For i = 0 To Me.m_mpas.Count - 1
                    info = Me.m_mpas(i)
                    If (info.IsNew) Then
                        bSuccess = bSuccess And Me.Core.AddEcospaceMPA(info.Name, i, MPAMonths, iDBID)
                    Else
                        If ((i + 1) <> info.Index) Then
                            bSuccess = bSuccess And Me.Core.MoveEcospaceMPA(info.DBID, i + 1)
                        End If
                    End If
                Next

                ' Remove MPAs
                For i = 0 To Me.m_mpasRemoved.Count - 1
                    info = Me.m_mpasRemoved(i)

                    ' Sanity check
                    Debug.Assert(Not info.IsNew())

                    If (info.Confirmed) Then
                        If (Not Me.Core.RemoveEcospaceMPA(info.DBID)) Then
                            bSuccess = False
                        End If
                    End If
                Next i
                If bSuccess Then Me.m_mpasRemoved.Clear()

                ' The core will reload now
                bSuccess = Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace, bSuccess)
                cApplicationStatusNotifier.EndProgress(Me.Core)

            End If

            ' Update core objects
            If (bMPAsChanged) Then

                ' Build quick local lookup for locating MPAs by DBID
                Dim dtMPAs As New Dictionary(Of Integer, cEcospaceMPA)
                For i = 1 To Me.Core.nMPAs
                    mpa = Me.Core.EcospaceMPAs(i)
                    dtMPAs(mpa.DBID) = mpa
                Next

                ' For each local MPA admin unit
                For i = 0 To Me.m_mpas.Count - 1
                    ' Get local admin unit
                    info = DirectCast(Me.m_mpas(i), cMPAInfo)
                    ' Is associated w existing MPA, e.g. could be changed?
                    If Not info.IsNew Then
                        ' Get MPA
                        mpa = dtMPAs(info.DBID)
                        ' Has user changed the MPA?
                        If info.IsChanged(mpa) Then
                            ' #Yes: update MPA
                            mpa.Name = info.Name
                        End If
                    End If
                Next
            End If

            Return bSuccess

        End Function


#End Region ' Apply changes

    End Class

End Namespace
