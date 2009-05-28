'==============================================================================
'
' $Log: ApplyShapeEwEGrid.vb,v $
' Revision 1.2  2009/05/28 12:36:53  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.1  2008/12/15 19:54:04  jeroens
' *** empty log message ***
'
' Revision 1.3  2008/12/15 16:01:58  jeroens
' no message
'
' Revision 1.2  2008/10/06 21:32:04  jeroens
' Unassignable cells are shown as blank
'
' Revision 1.1  2008/09/26 07:31:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/08/02 03:04:08  jeroens
' Renamed resources
'
' Revision 1.5  2008/07/31 21:03:28  jeroens
' Fixed issue 181
'
' Revision 1.4  2008/07/29 13:06:41  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.3  2008/06/02 00:07:45  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.2  2008/05/29 22:22:38  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.1  2008/05/23 15:54:36  jeroens
' Moved
'
' Revision 1.24  2008/01/22 02:41:40  jeroens
' Properly fixed grid apply mode
'
' Revision 1.23  2007/10/16 15:08:46  jeroens
' * Properly responds to core messages
'
' Revision 1.22  2007/10/16 14:22:57  jeroens
' * Implemented as EwEForm
'
' Revision 1.21  2007/10/15 16:50:08  joeb
' destoryed the core message handler in Disposed(). This needs to be changed to use the EwEForm as a base class
'
' Revision 1.20  2007/09/10 18:07:31  jeroens
' + Added update lock to improve performance
'
' Revision 1.19  2007/09/06 18:18:20  fgao
' update to apply FF to support both primary producer and consumer
'
' Revision 1.18  2007/09/05 21:39:03  fgao
' Update ApplyFF to ApplyFF primary producer and consumer...
'
' Revision 1.17  2007/08/31 18:13:58  fgao
' Update to uppercase..!!! suggested by Robyn..
'
' Revision 1.16  2007/08/30 23:52:04  fgao
' distinguish between ff index and mf index...
'
' Revision 1.15  2007/07/13 17:24:32  jeroens
' - Removed Forcing namespace
'
' Revision 1.14  2007/07/05 20:29:13  fgao
' no message
'
' Revision 1.13  2007/07/03 07:08:44  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.12  2007/06/28 23:27:30  fgao
' Apply FF by Row, by Col, By All, and etc..Some small bug fixes..
'
' Revision 1.11  2007/06/27 22:53:40  fgao
' Start to add more functionality to this grid
'
' Revision 1.10  2007/06/22 18:15:53  fgao
' Make up its look, autosize etc...
'
' Revision 1.9  2007/05/31 13:11:33  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.8  2007/01/17 22:59:20  fgao
' New ApplyFF UI
'
' Revision 1.7  2007/01/16 19:18:53  fgao
' Updated to combine into the new ApplyFF scheme.. Like getting rid of AppliesToMatrix, No applyFF listbox used any more..
'
' Revision 1.6  2007/01/16 00:58:56  fgao
' New ongoing ApplyFF prototype
'
' Revision 1.5  2007/01/10 22:44:33  fgao
' Removed combo cell. Started to migrate to new FF UI.
'
'==============================================================================

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

        Private m_RowColClick As New BehaviorModels.CustomEvents
        Private m_BehaviorClick As BehaviorModels.CustomEvents
        Private m_Cellfunc As DataModels.EditorTextBox
        Private m_Core As cCore
        Private m_PPIManager As cPPIManager
        Private m_applyTargetMode As eApplyTargetTypes = eApplyTargetTypes.NotSet
        Private m_applyShapeMode As eApplyShapeTypes = eApplyShapeTypes.NotSet

        Public Sub New(ByVal shapeType As eApplyShapeTypes, ByVal targetType As eApplyTargetTypes)

            MyBase.New()

            Me.m_Core = cCore.GetInstance()
            Me.m_PPIManager = m_Core.PPInteractionManager
            Me.m_applyShapeMode = shapeType
            Me.m_applyTargetMode = targetType

            Me.m_Cellfunc = New DataModels.EditorTextBox(GetType(Integer))
            Me.m_BehaviorClick = New BehaviorModels.CustomEvents()
            AddHandler m_RowColClick.Click, New SourceGrid2.PositionEventHandler(AddressOf bm_RowColClick)
            AddHandler m_BehaviorClick.Click, AddressOf CellClick

            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing

            ' Define grid dimensions
            Me.Redim(core.nGroups + 1, 2)

            ' Set header cells  'Prey \Predator '
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To core.nGroups
                source = core.EcoPathGroupInputs(i)
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

        Private Sub InsertColumn(ByRef source As cCoreGroupBase, ByVal columnIndex As Integer)
            Me.Columns.Insert(columnIndex)
            ' # Group name column header cells
            Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
            Me(0, columnIndex).Behaviors.Add(m_RowColClick)
        End Sub

        Protected Overrides Sub FillData()

            Dim cellDefault As EwECell = Nothing
            Dim ff As cForcingFunction = Nothing
            Dim PPI As cPredPreyInteraction = Nothing

            Dim iCol As Integer = 2
            ' For each column  (groupIndex - Predator)
            For groupIndex As Integer = 1 To Me.Columns.Count - 2
                ' For each row (rowIndex - Prey)
                For rowIndex As Integer = 1 To m_Core.nGroups

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

        Private Sub CellClick(ByVal sender As Object, ByVal e As PositionEventArgs)

            'Row num, column num starts from one, which is consistent with group index scheme (from one)
            Dim iPred As Integer = CInt(Me(0, e.Position.Column).Value)
            Dim dlg As New dlgApplyShape(e.Position.Row, iPred, Me.m_applyShapeMode, Me.m_applyTargetMode)

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
                    dlg = New dlgApplyShape(iPred, dlgApplyShape.eEditMode.Predator, Me.m_applyShapeMode, Me.m_applyTargetMode)
                End If
            Else
                ' #No: Prey row header clicked?
                If iCol < Me.FixedColumns Then
                    ' #Yes: Prey row clicked?
                    If iRow > 0 Then
                        ' #Yes: launch dialog for all predation of this prey
                        dlg = New dlgApplyShape(iRow, dlgApplyShape.eEditMode.Prey, Me.m_applyShapeMode, Me.m_applyTargetMode)
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

        Public Sub ClearAllPairs()

            Dim appl As AppLauncher = AppLauncher.GetInstance()
            Dim PPI As cPredPreyInteraction = Nothing
            Dim ff As cForcingFunction = Nothing
            Dim application As eForcingFunctionApplication

            appl.SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)
            Me.m_Core.SetBatchLock(cCore.eBatchLockType.Update)

            ' For each column  (groupIndex - Predator)
            For groupIndex As Integer = 1 To m_Core.nLivingGroups
                ' For each row (rowIndex - Prey)
                For rowIndex As Integer = 1 To m_Core.nGroups

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

            Me.m_Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecosim, True)
            appl.SetStatusText("", TriState.False)

        End Sub

        Public Sub SetAllPairs()
            Dim dlg As New dlgApplyShape(Me.m_applyShapeMode, Me.m_applyTargetMode)
            dlg.ShowDialog()
        End Sub

        ''' <summary>
        ''' Repopulate content without redimensioning
        ''' </summary>
        Public Sub UpdateContent()
            Me.FillData()
        End Sub

    End Class

End Namespace
