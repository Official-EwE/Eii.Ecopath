#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceLibrary

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class ApplyPredPreyShapeGrid
        Inherits ApplyShapeGrid

#Region " Private vars "

        Private m_applyTargetMode As eApplyTargetTypes = eApplyTargetTypes.NotSet
        Private m_applyShapeMode As eApplyShapeTypes = eApplyShapeTypes.NotSet

#End Region ' Private vars

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)
        End Sub

#Region " Public access "

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

        Public Overrides Sub ClearAllPairs()

            Dim interaction As cMediatedInteraction = Nothing
            Dim application As eForcingFunctionApplication
            Dim ff As cForcingFunction = Nothing

            cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.STATUS_APPLYVALUES)
            Me.Core.SetBatchLock(cCore.eBatchLockType.Update)

            ' For each column (groupIndex - Predator)
            For iPred As Integer = 1 To Core.nLivingGroups
                ' For each row (rowIndex - Prey)
                For iPrey As Integer = 1 To Core.nGroups

                    ' Can assign FF at this spot in the matrix?
                    If m_InteractionManager.isPredPrey(iPred, iPrey) Then

                        interaction = m_InteractionManager.PredPreyInteraction(iPred, iPrey)
                        interaction.LockUpdates = True

                        For i As Integer = 1 To Me.m_InteractionManager.MaxNShapes
                            interaction.getShape(i, ff, application)

                            ' Only delete pairs of current type
                            If (TypeOf ff Is cMediationBaseFunction) And _
                               (Me.m_applyShapeMode = eApplyShapeTypes.Mediation) Then
                                interaction.setShape(i, Nothing)
                            End If

                            If (TypeOf ff Is cForcingFunction) And _
                               (Me.m_applyShapeMode = eApplyShapeTypes.Forcing) Then
                                interaction.setShape(i, Nothing)
                            End If
                        Next

                        interaction.LockUpdates = False

                    End If
                Next
            Next

            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, True)
            cApplicationStatusNotifier.EndProgress(Me.Core)

        End Sub

        Public Overrides Sub SetAllPairs()
            Dim dlg As New dlgApplyPredPreyShape(Me.UIContext, Me.m_applyShapeMode, Me.m_applyTargetMode)
            dlg.ShowDialog()
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
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To Core.nGroups
                source = Core.EcoPathGroupInputs(i)
                ' # Group name row header cells
                Me(i, 0) = New EwERowHeaderCell(CStr(i))
                Me(i, 0).Behaviors.Add(m_RowColClick)

                ' # Group name row header cells
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                Me(i, 1).Behaviors.Add(m_RowColClick)

                If ((Me.m_applyTargetMode And eApplyTargetTypes.Consumer) = eApplyTargetTypes.Consumer) Then
                    If source.PP < 1 Then
                        Me.InsertColumn(source, columnIndex)
                        columnIndex = columnIndex + 1
                    End If
                ElseIf ((Me.m_applyTargetMode And eApplyTargetTypes.PrimaryProducer) = eApplyTargetTypes.PrimaryProducer) Then
                    If source.PP = 1 Then
                        Me.InsertColumn(source, columnIndex)
                        columnIndex = columnIndex + 1
                    End If
                End If
            Next


        End Sub

        Protected Overrides Sub FillData()

            Dim cellDefault As EwECell = Nothing
            Dim ff As cForcingFunction = Nothing
            Dim PPI As cMediatedInteraction = Nothing

            If (Me.m_InteractionManager Is Nothing) Then Return
            If (Me.m_applyShapeMode = eApplyShapeTypes.NotSet) Then Return
            If (Me.m_applyTargetMode = eApplyTargetTypes.NotSet) Then Return

            Dim iCol As Integer = 2
            ' For each column  (groupIndex - Predator)
            For groupIndex As Integer = 1 To Me.Columns.Count - 2
                ' For each row (rowIndex - Prey)
                For rowIndex As Integer = 1 To Core.nGroups

                    Dim iGroup As Integer = CInt(Me(0, groupIndex + 1).Value)

                    ' Can assign FF at this spot in the matrix?
                    If m_InteractionManager.isPredPrey(iGroup, rowIndex) Then

                        PPI = m_InteractionManager.PredPreyInteraction(iGroup, rowIndex)
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
                                        sb.Append(String.Format(My.Resources.ECOSIM_APPLYFF_FFTYPE_MEDIATION, shape.Index))
                                    End If
                                Else
                                    If ((Me.m_applyShapeMode And eApplyShapeTypes.Forcing) = eApplyShapeTypes.Forcing) Then
                                        If sb.Length > 0 Then sb.Append(" ")
                                        sb.Append(String.Format(My.Resources.ECOSIM_APPLYFF_FFTYPE_FORCING, shape.Index))
                                    End If
                                End If
                            Next
                        Else
                            ' This should NOT occur; this indicates that the PPI manager is not up to date!
                            sb.Append("X")
                        End If

                        Me(rowIndex, iCol) = New Cells.Real.Cell(sb.ToString)
                        Me(rowIndex, iCol).DataModel = m_editor
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

                iCol += 1

            Next groupIndex

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumnWidths = False
        End Sub

#End Region ' Overrides 

#Region " Internals "

        Protected Overrides Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)

            'Row num, column num starts from one, which is consistent with group index scheme (from one)
            Dim iPred As Integer = CInt(Me(0, e.Position.Column).Value)
            Dim dlg As New dlgApplyPredPreyShape(Me.UIContext, _
                                         e.Position.Row, iPred, _
                                         Me.m_applyShapeMode, Me.m_applyTargetMode)

            dlg.ShowDialog()

        End Sub

        Protected Overrides Sub OnRowColClicked(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)

            Dim iRow As Integer = e.Position.Row
            Dim iCol As Integer = e.Position.Column
            Dim dlg As dlgApplyPredPreyShape = Nothing

            ' --------------
            ' Prepare dialog
            ' --------------

            ' Column header clicked?
            If iRow = 0 Then
                ' #Yes: Predator column clicked?
                If iCol > 1 Then
                    ' #Yes: launch dialog for all diets of this predator
                    Dim iPred As Integer = CInt(Me(0, iCol).Value)
                    dlg = New dlgApplyPredPreyShape(Me.UIContext, iPred, dlgApplyPredPreyShape.eEditMode.Predator, Me.m_applyShapeMode, Me.m_applyTargetMode)
                End If
            Else
                ' #No: Prey row header clicked?
                If iCol < Me.FixedColumns Then
                    ' #Yes: Prey row clicked?
                    If iRow > 0 Then
                        ' #Yes: launch dialog for all predation of this prey
                        dlg = New dlgApplyPredPreyShape(Me.UIContext, iRow, dlgApplyPredPreyShape.eEditMode.Prey, Me.m_applyShapeMode, Me.m_applyTargetMode)
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

        Protected Sub InsertColumn(ByRef source As cCoreGroupBase, ByVal columnIndex As Integer)
            Me.Columns.Insert(columnIndex)
            ' # Group name column header cells
            Me(0, columnIndex) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            Me(0, columnIndex).Behaviors.Add(m_RowColClick)
        End Sub

#End Region ' Internals

    End Class

End Namespace
