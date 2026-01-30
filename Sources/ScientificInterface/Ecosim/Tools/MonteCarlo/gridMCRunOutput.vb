' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim


    Public Class gridMCRunOutput
        Inherits cEwEGrid

        Private m_mcmanager As cMonteCarloManager = Nothing

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Property UIContext() As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
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
            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_BIOMASS)
            Me(0, 3) = New cEwEColumnHeaderCell(eVarNameFlags.PBOutput, eDescriptorTypes.Abbreviation)
            Me(0, 4) = New cEwEColumnHeaderCell(SharedResources.HEADER_CB)
            Me(0, 5) = New cEwEColumnHeaderCell(eVarNameFlags.EEInput, eDescriptorTypes.Abbreviation)
            Me(0, 6) = New cEwEColumnHeaderCell(SharedResources.HEADER_BIOMACCUM_ABBR)
            'Me(0, 7) = New EwEColumnHeaderCell("Landings")
            'Me(0, 8) = New EwEColumnHeaderCell("Discards")

            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim mcGrp As cCoreGroupBase = Nothing

            For i As Integer = 1 To Me.Core.nLivingGroups
                mcGrp = Me.m_mcmanager.Groups(i)
                Me(i, 0) = New cEwERowHeaderCell(CStr(mcGrp.Index))
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mcGrp, eVarNameFlags.Name)
                Me(i, 2) = New cPropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcBbf)
                Me(i, 3) = New cPropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcPBbf)
                Me(i, 4) = New cPropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcQBbf)
                Me(i, 5) = New cPropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcEEbf)
                Me(i, 6) = New cPropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcBAbf)
                'Me(i, 7) = New PropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcLandingsbf)
                'Me(i, 8) = New PropertyCell(Me.PropertyManager, mcGrp, eVarNameFlags.mcDiscardsbf)
            Next

        End Sub

    End Class

End Namespace


