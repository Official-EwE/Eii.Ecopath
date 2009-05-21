'==============================================================================
'
' $Log: NicheOverlapPreyEwEGrid.vb,v $
' Revision 1.4  2009/05/21 18:53:46  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.3  2009/01/16 18:30:09  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:55:37  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:33  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.14  2008/07/29 13:06:44  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.13  2008/06/02 00:01:27  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.12  2008/05/29 22:22:40  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.11  2008/04/07 02:31:07  jeroens
' Cleaning up resources
'
' Revision 1.10  2007/10/10 02:59:12  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.9  2007/07/03 07:08:45  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.8  2007/06/21 23:57:20  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.7  2007/04/29 03:45:10  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.6  2006/09/21 01:00:24  jeroens
' * Updated to cCoreGroupBase
'
' Revision 1.5  2006/08/22 04:06:40  jeroens
' + Populated
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports ScientificInterfaceShared.Properties
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class NicheOverlapPreyEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub


        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing

            ' Define grid dimensions
            Me.Redim(core.nLivingGroups + 1, 2)

            ' Set header cells
            ' # (0,0)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            Dim columnIndex As Integer = 2

            ' For every living groups
            For i As Integer = 1 To core.nLivingGroups
                'Get group output
                source = core.EcoPathGroupOutputs(i)
                ' Define column header cell
                Me.Columns.Insert(columnIndex)
                Me(0, columnIndex) = New PropertyColumnHeaderCell(source, eVarNameFlags.Index)
                ' Define row header cell
                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 1) = New EwERowHeaderCell(source.Name)
                columnIndex = columnIndex + 1
            Next

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreGroupBase = Nothing
            Dim sourceSec As cCoreGroupBase = Nothing
            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()

            For columnIndex As Integer = 2 To core.nLivingGroups + 1
                source = core.EcoPathGroupOutputs(columnIndex - 1)
                For rowIndex As Integer = 1 To core.nLivingGroups
                    ' Get the group output
                    sourceSec = core.EcoPathGroupOutputs(rowIndex)

                    If columnIndex <= rowIndex + 1 Then
                        If source.PP() <= 1 Then
                            Dim cell As PropertyCell = Nothing

                            ' Get the indexed property by (rowIndex, columnIndex)
                            prop = pm.GetProperty(sourceSec, eVarNameFlags.Plap, source)
                            ' Add property to the cell
                            cell = New PropertyCell(prop)
                            ' Config cell
                            cell.SuppressZero = True
                            ' Place cell into grid
                            Me(rowIndex, columnIndex) = cell
                        End If
                    Else
                        Dim cell As NichePropertyColourCell = Nothing

                        ' Get the indexed property by (rowIndex, columnIndex)
                        prop = pm.GetProperty(sourceSec, eVarNameFlags.Plap, source)
                        ' Add property to the cell
                        cell = New NichePropertyColourCell(prop)
                        ' Place cell into grid
                        Me(rowIndex, columnIndex) = cell
                    End If
                Next
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
