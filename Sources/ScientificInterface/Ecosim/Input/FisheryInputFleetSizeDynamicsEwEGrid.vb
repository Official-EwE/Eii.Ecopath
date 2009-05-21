'==============================================================================
'
' $Log: FisheryInputFleetSizeDynamicsEwEGrid.vb,v $
' Revision 1.4  2009/05/21 18:53:42  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.3  2009/01/16 18:30:38  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:53:40  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:35  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.7  2008/08/02 03:04:15  jeroens
' Renamed resources
'
' Revision 1.6  2008/06/02 00:01:34  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.5  2008/05/29 22:22:54  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.4  2008/04/07 02:31:14  jeroens
' Cleaning up resources
'
' Revision 1.3  2008/02/13 16:44:29  jeroens
' Renamed resources
'
' Revision 1.2  2007/10/10 02:59:14  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.1  2007/07/13 16:34:43  jeroens
' * Moved
'
' Revision 1.9  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.8  2007/04/29 03:45:12  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class FisheryInputFleetSizeDynamicsEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            ' Redim the grid dimension
            Me.Redim(1, 6)

            ' Define column header
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_EFFORTRESPPOWER)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_INITEFFORT)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_CAPITALDEPRECIATION)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_INITCAPTIALGROWTHRATE)

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing

            For rowIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(rowIndex)
                Me.Rows.Insert(rowIndex)
                ' Name is fleet name
                Me(rowIndex, 0) = New EwERowHeaderCell(rowIndex)
                Me(rowIndex, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                Me(rowIndex, 2) = New PropertyCell(source, eVarNameFlags.EPower)
                Me(rowIndex, 3) = New PropertyCell(source, eVarNameFlags.PcapBase)
                Me(rowIndex, 4) = New PropertyCell(source, eVarNameFlags.CapDepreciate)
                Me(rowIndex, 5) = New PropertyCell(source, eVarNameFlags.CapBaseGrowth)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

