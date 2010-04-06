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

        Private m_mcmanager As cMonteCarloManager = Nothing

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As ScientificInterfaceShared.Controls.cUIContext)
                If (value IsNot Nothing) Then
                    Me.m_mcmanager = value.Core.EcosimMonteCarlo
                Else
                    Me.m_mcmanager = Nothing
                End If
                MyBase.UIContext = value
            End Set
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(Me.Core.nLivingGroups + 1, 7)
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

            For i As Integer = 1 To Me.Core.nLivingGroups
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

    End Class

End Namespace


