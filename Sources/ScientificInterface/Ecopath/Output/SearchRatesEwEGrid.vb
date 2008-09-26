'==============================================================================
'
' $Log: SearchRatesEwEGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:34  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.15  2008/08/02 03:04:12  jeroens
' Renamed resources
'
' Revision 1.14  2008/07/29 13:06:44  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.13  2008/07/21 23:48:40  jeroens
' Simplified cell construction
'
' Revision 1.12  2008/06/02 00:01:27  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.11  2008/05/29 22:22:41  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.10  2007/10/10 02:59:13  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.9  2007/07/03 07:08:47  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.8  2007/06/21 23:57:21  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.7  2007/04/29 03:45:10  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.6  2006/09/21 01:00:24  jeroens
' * Updated to cCoreGroupBase
'
' Revision 1.5  2006/08/20 02:08:06  jeroens
' + Completed contents
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class SearchRatesEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing

            Me.Redim(core.nGroups + 1, 2)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_PREYPREDATOR)

            Dim columnIndex As Integer = 2

            For i As Integer = 1 To core.nGroups
                ' Column displays mixed consumer/producer groups ( PP < 1)
                source = core.EcoPathGroupOutputs(i)
                Me(i, 0) = New EwERowHeaderCell(i)
                ' # Group name row header cells
                Me(i, 1) = New EwERowHeaderCell(source.Name)

                If source.PP < 1 Then
                    Me.Columns.Insert(columnIndex)
                    Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
                    columnIndex = columnIndex + 1
                End If

            Next

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            Dim cell As PropertyCell = Nothing
            Dim columnIndex As Integer = 2

            Dim visDiagonal As New SourceGrid2.VisualModels.Common
            visDiagonal.BackColor = Color.LightGray
            visDiagonal.TextAlignment = ContentAlignment.MiddleCenter

            For groupIndex As Integer = 1 To core.nGroups

                'Get the group output
                source = core.EcoPathGroupOutputs(groupIndex)
                If source.PP < 1 Then
                    For rowIndex As Integer = 1 To core.nGroups
                        ' Get the group output
                        sourceSec = core.EcoPathGroupOutputs(rowIndex)
                        ' Get the indexed comsumption property by (rowIndex, columnIndex)
                        prop = pm.GetProperty(sourceSec, eVarNameFlags.SearchRate, source)
                        ' Add property to the cell
                        cell = New PropertyCell(prop)

                        If rowIndex = columnIndex - 1 Then
                            cell.VisualModel = visDiagonal
                        End If

                        ' Config cell
                        cell.SuppressZero = True
                        ' Plug cell into grid
                        Me(rowIndex, columnIndex) = cell
                    Next
                    columnIndex = columnIndex + 1
                End If
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

    End Class

End Namespace
