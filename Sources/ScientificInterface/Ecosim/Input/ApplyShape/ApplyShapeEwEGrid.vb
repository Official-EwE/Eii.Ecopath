#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Text
Imports EwECore
Imports SourceGrid2
Imports SourceLibrary
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class ApplyShapeEwEGrid
        Inherits EwEGrid

#Region " Private vars "

        Private m_RowColClick As New BehaviorModels.CustomEvents
        Private m_BehaviorClick As BehaviorModels.CustomEvents
        Private m_Cellfunc As DataModels.EditorTextBox
        Private m_PPIManager As cPPIManager
        Private m_applyTargetMode As eApplyTargetTypes = eApplyTargetTypes.NotSet
        Private m_applyShapeMode As eApplyShapeTypes = eApplyShapeTypes.NotSet

#End Region ' Private vars

        Public Sub New()
            MyBase.New()

            Me.m_Cellfunc = New DataModels.EditorTextBox(GetType(Integer))
            Me.m_BehaviorClick = New BehaviorModels.CustomEvents()
            AddHandler m_RowColClick.Click, New SourceGrid2.PositionEventHandler(AddressOf bm_RowColClick)
            AddHandler m_BehaviorClick.Click, AddressOf CellClick
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)

            If Me.m_Cellfunc IsNot Nothing Then
                RemoveHandler m_RowColClick.Click, New SourceGrid2.PositionEventHandler(AddressOf bm_RowColClick)
                RemoveHandler m_BehaviorClick.Click, AddressOf CellClick
                Me.m_Cellfunc = Nothing
                Me.m_BehaviorClick = Nothing
            End If

        End Sub

#Region " Public access "

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As cUIContext)
                If value IsNot Nothing Then
                    ' First set crucial properties
                    Me.m_PPIManager = value.Core.PPInteractionManager
                    ' Refresh the grid
                    MyBase.UIContext = value
                End If
            End Set
        End Property

        Public Property ApplyShapeMode() As eApplyShapeTypes
            Get
                Return Me.m_applyShapeMode
            End Get
            Set(ByVal value As eApplyShapeTypes)
                If (Me.m_applyShapeMode <> value) Then
                    Me.m_applyShapeMode = value
                    Me.RefreshContent()
                End If
            End Set
        End Property

        Public Property ApplyTargetMode() As eApplyTargetTypes
            Get
                Return Me.m_applyTargetMode
            End Get
            Set(ByVal value As eApplyTargetTypes)
                If (value <> Me.m_applyTargetMode) Then
                    Me.m_applyTargetMode = value
                    Me.RefreshContent()
                End If
            End Set
        End Property

        Public Sub ClearAllPairs()

            Dim PPI As cPredPreyInteraction = Nothing
            Dim ff As cForcingFunction = Nothing
            Dim application As eForcingFunctionApplication

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)
            Me.Core.SetBatchLock(cCore.eBatchLockType.Update)

            ' For each column  (groupIndex - Predator)
            For groupIndex As Integer = 1 To Core.nLivingGroups
                ' For each row (rowIndex - Prey)
                For rowIndex As Integer = 1 To Core.nGroups

                    ' Can assign FF at this spot in the matrix?
                    If m_PPIManager.isPredPrey(groupIndex, rowIndex) Then

                        PPI = m_PPIManager.Interaction(groupIndex, rowIndex)
                        PPI.LockUpdates = True

                        For i As Integer = 1 To PPI.MaxNumShapes
                            ' Only delete pairs of current type
                            PPI.getShape(i, ff, application)

                            If (TypeOf ff Is cMediationFunction) Then
                                If ((Me.m_applyShapeMode And eApplyShapeTypes.Mediation) = eApplyShapeTypes.Mediation) Then
                                    PPI.setShape(i, Nothing)
                                End If
                            Else
                                If ((Me.m_applyShapeMode And eApplyShapeTypes.Forcing) = eApplyShapeTypes.Forcing) Then
                                    PPI.setShape(i, Nothing)
                                End If
                            End If
                        Next

                        PPI.LockUpdates = False

                    End If
                Next
            Next

            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, True)
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

        End Sub

        Public Sub SetAllPairs()
            Dim dlg As New dlgApplyShape(Me.UIContext, Me.m_applyShapeMode, Me.m_applyTargetMode)
            dlg.ShowDialog()
        End Sub

        ''' <summary>
        ''' Repopulate content without redimensioning
        ''' </summary>
        Public Sub UpdateContent()
            Me.FillData()
        End Sub

#End Region ' Public properties

#Region " Overrides "

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_applyShapeMode = eApplyShapeTypes.NotSet) Then Return
            If (Me.m_applyTargetMode = eApplyTargetTypes.NotSet) Then Return

            Dim source As cCoreGroupBase = Nothing

            ' Define grid dimensions
            Me.Redim(Core.nGroups + 1, 2)

            ' Set header cells  'Prey \Predator '
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To Core.nGroups
                source = Core.EcoPathGroupInputs(i)
                ' # Group name row header cells
                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 0).Behaviors.Add(m_RowColClick)

                ' # Group name row header cells
                Me(i, 1) = New EwERowHeaderCell(source.Name)
                Me(i, 1).Behaviors.Add(m_RowColClick)

                If ((Me.m_applyTargetMode And eApplyTargetTypes.Consumer) = eApplyTargetTypes.Consumer) Then
                    If source.PP < 1 Then
                        InsertColumn(source, columnIndex)
                        columnIndex = columnIndex + 1
                    End If
                ElseIf ((Me.m_applyTargetMode And eApplyTargetTypes.PrimaryProducer) = eApplyTargetTypes.PrimaryProducer) Then
                    If source.PP = 1 Then
                        InsertColumn(source, columnIndex)
                        columnIndex = columnIndex + 1
                    End If
                End If
            Next


        End Sub

        Protected Overrides Sub FillData()

            Dim cellDefault As EwECell = Nothing
            Dim ff As cForcingFunction = Nothing
            Dim PPI As cPredPreyInteraction = Nothing

            If (Me.m_PPIManager Is Nothing) Then Return
            If (Me.m_applyShapeMode = eApplyShapeTypes.NotSet) Then Return
            If (Me.m_applyTargetMode = eApplyTargetTypes.NotSet) Then Return

            Dim iCol As Integer = 2
            ' For each column  (groupIndex - Predator)
            For groupIndex As Integer = 1 To Me.Columns.Count - 2
                ' For each row (rowIndex - Prey)
                For rowIndex As Integer = 1 To Core.nGroups

                    Dim iGroup As Integer = CInt(Me(0, groupIndex + 1).Value)

                    ' Can assign FF at this spot in the matrix?
                    If m_PPIManager.isPredPrey(iGroup, rowIndex) Then

                        PPI = m_PPIManager.Interaction(iGroup, rowIndex)
                        Dim shape As cForcingFunction = Nothing
                        Dim aplType As eForcingFunctionApplication
                        Dim sb As New StringBuilder()

                        If PPI IsNot Nothing Then
                            For i As Integer = 1 To PPI.NAppliedShapes
                                PPI.getShape(i, shape, aplType)

                                ' Is med?
                                If (TypeOf shape Is cMediationFunction) Then
                                    If ((Me.m_applyShapeMode And eApplyShapeTypes.Mediation) = eApplyShapeTypes.Mediation) Then
                                        If sb.Length > 0 Then sb.Append(" ")
                                        sb.Append(String.Format(My.Resources.ECOSIM_APPLYFF_CELLFORMAT, _
                                                My.Resources.ECOSIM_APPLYFF_FFTYPE_MEDIATION, _
                                                shape.Index))
                                    End If
                                Else
                                    If ((Me.m_applyShapeMode And eApplyShapeTypes.Forcing) = eApplyShapeTypes.Forcing) Then
                                        If sb.Length > 0 Then sb.Append(" ")
                                        sb.Append(String.Format(My.Resources.ECOSIM_APPLYFF_CELLFORMAT, _
                                                My.Resources.ECOSIM_APPLYFF_FFTYPE_FORCING, _
                                                shape.Index))
                                    End If
                                End If
                            Next
                        Else
                            ' This should NOT occur; this indicates that the PPI manager is not up to date!
                            sb.Append("X")
                        End If

                        Me(rowIndex, iCol) = New Cells.Real.Cell(sb.ToString)
                        Me(rowIndex, iCol).DataModel = m_Cellfunc
                        Me(rowIndex, iCol).Behaviors.Add(m_BehaviorClick)

                    Else
                        ' #No: cannot assign FF to this pred/prey combo
                        cellDefault = New EwECell(Nothing, GetType(Single))
                        '  Setup default cell
                        cellDefault.Style = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null)
                        ' Apply cell to the grid
                        Me(rowIndex, iCol) = cellDefault
                    End If

                Next rowIndex

                iCol = iCol + 1

            Next groupIndex

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumnWidths = False
        End Sub

#End Region ' Overrides 

#Region " Internals "

        Private Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)

            'Row num, column num starts from one, which is consistent with group index scheme (from one)
            Dim iPred As Integer = CInt(Me(0, e.Position.Column).Value)
            Dim dlg As New dlgApplyShape(Me.UIContext, _
                                         e.Position.Row, iPred, _
                                         Me.m_applyShapeMode, Me.m_applyTargetMode)

            dlg.ShowDialog()

        End Sub

        Protected Overridable Sub bm_RowColClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)

            Dim iRow As Integer = e.Position.Row
            Dim iCol As Integer = e.Position.Column
            Dim dlg As dlgApplyShape = Nothing

            ' --------------
            ' Prepare dialog
            ' --------------

            ' Column header clicked?
            If iRow = 0 Then
                ' #Yes: Predator column clicked?
                If iCol > 1 Then
                    ' #Yes: launch dialog for all diets of this predator
                    Dim iPred As Integer = CInt(Me(0, iCol).Value)
                    dlg = New dlgApplyShape(Me.UIContext, iPred, dlgApplyShape.eEditMode.Predator, Me.m_applyShapeMode, Me.m_applyTargetMode)
                End If
            Else
                ' #No: Prey row header clicked?
                If iCol < Me.FixedColumns Then
                    ' #Yes: Prey row clicked?
                    If iRow > 0 Then
                        ' #Yes: launch dialog for all predation of this prey
                        dlg = New dlgApplyShape(Me.UIContext, iRow, dlgApplyShape.eEditMode.Prey, Me.m_applyShapeMode, Me.m_applyTargetMode)
                    End If
                End If
            End If

            ' --------------
            ' Invoke dialog
            ' --------------

            If dlg IsNot Nothing Then
                dlg.ShowDialog()
            End If

        End Sub

        Private Sub InsertColumn(ByRef source As cCoreGroupBase, ByVal columnIndex As Integer)
            Me.Columns.Insert(columnIndex)
            ' # Group name column header cells
            Me(0, columnIndex) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            Me(0, columnIndex).Behaviors.Add(m_RowColClick)
        End Sub

#End Region ' Internals

    End Class

End Namespace
