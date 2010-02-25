#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class MCRunOutputGrid
        : Inherits EwEGrid

        Private m_core As cCore = Nothing
        Private m_mcmanager As cMonteCarloManager = Nothing

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.m_core = cCore.GetInstance()
            If Me.m_core Is Nothing Then Return
            Me.m_mcmanager = m_core.EcosimMonteCarlo

            Me.Redim(m_core.nLivingGroups + 1, 7)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASS)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_PB_ABBR)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_CB)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_EE)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.MCRUN_HEADER_OUTPUT_BA)

            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim mcGrp As cCoreGroupBase = Nothing

            For i As Integer = 1 To m_core.nLivingGroups
                mcGrp = m_mcmanager.Groups(i)
                Me(i, 0) = New EwERowHeaderCell(mcGrp.Index)
                Me(i, 1) = New EwERowHeaderCell(mcGrp.Name)
                Me(i, 2) = New PropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcBbf)
                Me(i, 3) = New PropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcPBbf)
                Me(i, 4) = New PropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcQBbf)
                Me(i, 5) = New PropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcEEbf)
                Me(i, 6) = New PropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcBAbf)
            Next

        End Sub

        Public Sub RefreshData()
            Me.FillData()
        End Sub

    End Class

End Namespace


