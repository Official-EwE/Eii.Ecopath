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
                source = core.EcosimFleetInputs(rowIndex)
                Me.Rows.Insert(rowIndex)
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
                Return eCoreComponentType.EcoSim
            End Get
        End Property

    End Class

End Namespace

