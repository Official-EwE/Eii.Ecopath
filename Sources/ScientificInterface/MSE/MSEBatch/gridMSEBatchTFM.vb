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

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.Cells
Imports SourceGrid2.Cells.Real

#End Region

<CLSCompliant(False)> _
Public Class gridMSEBatchTFM
    Inherits cEwEGrid

    ' ToDo: Globalize this class 
    ' ToDo: Add XML comments

    Private m_iter As Integer

#Region " Internal defs "

    Private Enum eColumnTypes As Integer
        Index = 0
        Name
        RunType
        BLim
        BLimValue
        BLimLow
        BLimUp
        BBase
        BBaseValue
        BBaseLow
        BBaseUp
        FOpt
        FOptValue
        FOptLow
        FOptUp
    End Enum

#End Region ' Internal defs

    Public Sub New()
        MyBase.new()
        Me.m_iter = 1
    End Sub

#Region " Overrides "

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length

        Me.Redim(1, iNumCols)
        Dim limitStr As String = "%"
        If Me.UIContext IsNot Nothing Then
            'UIContext can be nothing in the development enviro
            If Me.UIContext.Core.MSEBatchManager.Parameters.IterCalcType = eMSEBatchIterCalcTypes.UpperLowerValues Then
                limitStr = "Value"
            End If
        End If

        Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
        Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        Me(0, eColumnTypes.RunType) = New cEwEColumnHeaderCell("Managed via TFM")

        Me(0, eColumnTypes.BLim) = New cEwEColumnHeaderCell("Biomass limit") 'B lim(-)
        Me(0, eColumnTypes.BLimValue) = New cEwEColumnHeaderCell("Iter.(" & Me.iCurIter.ToString & ")")
        Me(0, eColumnTypes.BLimLow) = New cEwEColumnHeaderCell("Lower " & limitStr) 'B lim(-)
        Me(0, eColumnTypes.BLimUp) = New cEwEColumnHeaderCell("Upper " & limitStr) 'B Lim(+)

        Me(0, eColumnTypes.BBase) = New cEwEColumnHeaderCell("Biomass base")
        Me(0, eColumnTypes.BBaseValue) = New cEwEColumnHeaderCell("Iter.(" & Me.iCurIter.ToString & ")")
        Me(0, eColumnTypes.BBaseLow) = New cEwEColumnHeaderCell("Lower " & limitStr)
        Me(0, eColumnTypes.BBaseUp) = New cEwEColumnHeaderCell("Upper " & limitStr)

        Me(0, eColumnTypes.FOpt) = New cEwEColumnHeaderCell("F max.")
        Me(0, eColumnTypes.FOptValue) = New cEwEColumnHeaderCell("Iter.(" & Me.iCurIter.ToString & ")")
        Me(0, eColumnTypes.FOptLow) = New cEwEColumnHeaderCell("Lower " & limitStr)
        Me(0, eColumnTypes.FOptUp) = New cEwEColumnHeaderCell("Upper " & limitStr)

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()

        Dim group As MSE.cMSEBatchTFMGroup = Nothing
        Dim RowStyle As cStyleGuide.eStyleFlags
        Dim iRow As Integer

        For iGroup As Integer = 1 To Me.Core.nLivingGroups

            'Get the group info
            group = Me.Core.MSEBatchManager.TFMGroups(iGroup)

            iRow = Me.AddRow()

            Debug.Assert(iGroup = iRow)

            RowStyle = DirectCast(group.GetStatus(eVarNameFlags.MSEBLim), cStyleGuide.eStyleFlags)
            Me(iGroup, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(iGroup))
            Me(iGroup, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

            Me(iGroup, eColumnTypes.RunType) = New cPropertyCheckboxCell(Me.PropertyManager, group, eVarNameFlags.MSEBatchTFMManaged)

            Me(iGroup, eColumnTypes.BLimLow) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBLimLower)
            Me(iGroup, eColumnTypes.BLim) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEBLim)

            Me(iGroup, eColumnTypes.BLimValue) = New cEwECell(group.BLimValue(Me.iCurIter), GetType(Single), RowStyle)
            Me(iGroup, eColumnTypes.BLimValue).Behaviors.Add(Me.EwEEditHandler)

            Me(iGroup, eColumnTypes.BLimUp) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBLimUpper)

            Me(iGroup, eColumnTypes.BBaseLow) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBBaseLower)
            Me(iGroup, eColumnTypes.BBase) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEBBase)

            Me(iGroup, eColumnTypes.BBaseValue) = New cEwECell(group.BBaseValue(Me.iCurIter), GetType(Single), RowStyle)
            Me(iGroup, eColumnTypes.BBaseValue).Behaviors.Add(Me.EwEEditHandler)

            Me(iGroup, eColumnTypes.BBaseUp) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMBBaseUpper)

            Me(iGroup, eColumnTypes.FOptLow) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMFOptLower)
            Me(iGroup, eColumnTypes.FOpt) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEFmax)

            Me(iGroup, eColumnTypes.FOptValue) = New cEwECell(group.FMaxValue(Me.iCurIter), GetType(Single), RowStyle)
            Me(iGroup, eColumnTypes.FOptValue).Behaviors.Add(Me.EwEEditHandler)

            Me(iGroup, eColumnTypes.FOptUp) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSETFMFOptUpper)

        Next iGroup

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.Selection.SelectionMode = GridSelectionMode.Row
    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.MSE
        End Get
    End Property


    Public Property iCurIter As Integer
        Get
            Return Me.m_iter
        End Get

        Set(value As Integer)

            If (Me.UIContext Is Nothing) Then Return ' Could assert here; should not happen

            If (value <= Me.UIContext.Core.MSEBatchManager.Parameters.nTFMIteration) Then
                Me.m_iter = value
                Me.RefreshContent()
            End If

        End Set

    End Property


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
    Protected Overrides Function OnCellEdited(p As Position, cell As Cells.ICellVirtual) As Boolean
        Dim iGrp As Integer
        Dim ColType As eColumnTypes

        Try

            Dim val As Object = Me(p.Row, p.Column).Value
            iGrp = p.Row
            ColType = CType(p.Column, eColumnTypes)

            Select Case ColType
                Case eColumnTypes.BBaseValue
                    Me.Core.MSEBatchManager.TFMGroups(iGrp).BBaseValue(Me.iCurIter) = CSng(val)
                Case eColumnTypes.BLimValue
                    Me.Core.MSEBatchManager.TFMGroups(iGrp).BLimValue(Me.iCurIter) = CSng(val)
                Case eColumnTypes.FOptValue
                    Me.Core.MSEBatchManager.TFMGroups(iGrp).FMaxValue(Me.iCurIter) = CSng(val)
            End Select

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".OnCellEdited() Exception " & ex.Message)
        End Try

        Return True

    End Function

#End Region ' Overrides


End Class
