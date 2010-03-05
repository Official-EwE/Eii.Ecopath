#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class gridFishingQuotas
        Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim src As cCoreInputOutputBase = Nothing

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(1, 2 + core.nFleets)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            For iFleet As Integer = 1 To core.nFleets
                src = core.FleetInputs(iFleet)
                Me(0, 1 + iFleet) = New PropertyColumnHeaderCell(Me.PropertyManager, _
                                                                 src, eVarNameFlags.Name, Nothing, _
                                                                 My.Resources.GENERIC_LABEL_DETAILEDLABEL, cStyleGuide.eUnitType.Currency)
            Next

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub FillData()

            Dim reg As cEcosimFisheriesRegulation = Nothing
            Dim group As cCoreInputOutputBase = Nothing

            ' For each group
            For iGroup As Integer = 1 To core.nGroups

                Me.AddRow()

                'Get the group info
                group = core.EcoPathGroupInputs(iGroup)

                ' Fleet name As row header
                Me(iGroup, 0) = New EwERowHeaderCell(iGroup)
                Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                ' Fleet cells
                For iFleet As Integer = 1 To core.nFleets
                    reg = core.EcosimFisheriesRegulations(iFleet)
                    Me(iGroup, 1 + iFleet) = New PropertyCell(Me.PropertyManager, reg, eVarNameFlags.Quota, group)
                Next
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoSim
            End Get
        End Property

    End Class

End Namespace ' Ecosim
