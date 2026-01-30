' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources


Public Class gridMSEBatchFixedF
    Inherits cEwEGrid

    ' ToDo: Globalize this class 
    ' ToDo: Add XML comments

    Private m_iter As Integer

#Region " Internal defs "

    Private Enum eColumnTypes As Integer
        Index = 0
        Name
        RunType
        FixedF
        FixedFValue
        FixedFLow
        FixedFUp
    End Enum

#End Region ' Internal defs

    Public Sub New()
        MyBase.New()
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
        Me(0, eColumnTypes.RunType) = New cEwEColumnHeaderCell("Managed via Fishing Mort.")

        Me(0, eColumnTypes.FixedF) = New cEwEColumnHeaderCell("Fixed F") 'B lim(-)
        Me(0, eColumnTypes.FixedFValue) = New cEwEColumnHeaderCell("Iter.(" & Me.iCurIter.ToString & ")")
        Me(0, eColumnTypes.FixedFLow) = New cEwEColumnHeaderCell("Lower " & limitStr) 'B lim(-)
        Me(0, eColumnTypes.FixedFUp) = New cEwEColumnHeaderCell("Upper " & limitStr) 'B Lim(+)


        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()

        Dim group As MSE.cMSEBatchFGroup
        Dim RowStyle As cStyleGuide.eStyleFlags

        For iGroup As Integer = 1 To Me.Core.nLivingGroups

            'Get the group info
            group = Me.Core.MSEBatchManager.FixedFGroups(iGroup)

            Me.AddRow()

            RowStyle = DirectCast(group.GetStatus(eVarNameFlags.MSEFixedF), cStyleGuide.eStyleFlags)
            Me(iGroup, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(iGroup))
            Me(iGroup, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

            ' ToDo: replace by property style
            Me(iGroup, eColumnTypes.RunType) = New cPropertyCheckboxCell(Me.PropertyManager, group, eVarNameFlags.MSEBatchFManaged)
            'Me(iGroup, eColumnTypes.RunType) = New EwECheckboxCell(group.isManaged, RowStyle)
            Me(iGroup, eColumnTypes.RunType).Behaviors.Add(Me.EwEEditHandler)

            Me(iGroup, eColumnTypes.FixedF) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEFixedF)

            Me(iGroup, eColumnTypes.FixedFValue) = New cEwECell(group.FixedFValue(Me.iCurIter), GetType(Single), RowStyle)
            Me(iGroup, eColumnTypes.FixedFValue).Behaviors.Add(Me.EwEEditHandler)

            Me(iGroup, eColumnTypes.FixedFLow) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEBatchFLower)
            Me(iGroup, eColumnTypes.FixedFUp) = New cPropertyCell(Me.PropertyManager, group, eVarNameFlags.MSEBatchFUpper)


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
            If Me.UIContext IsNot Nothing Then
                If value <= Me.UIContext.Core.MSEBatchManager.Parameters.nFixedFIteration Then
                    Me.m_iter = value
                    Me.RefreshContent()
                End If
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
                Case eColumnTypes.FixedFValue
                    Me.Core.MSEBatchManager.FixedFGroups(iGrp).FixedFValue(Me.iCurIter) = CSng(val)
            End Select

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".OnCellEdited() Exception " & ex.Message)
        End Try

        Return True

    End Function

    Protected Overrides Function OnCellValueChanged(p As SourceGrid2.Position, cell As SourceGrid2.Cells.ICellVirtual) As Boolean


    End Function

#End Region ' Overrides

End Class
