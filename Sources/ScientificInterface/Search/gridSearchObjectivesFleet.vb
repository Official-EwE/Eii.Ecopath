' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.SearchObjectives
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Grid allowing setting of Fleet search objectives.
    ''' </summary>
    ''' =======================================================================

    Public Class gridSearchObjectivesFleet
        Inherits cEwEGrid

        Private m_Manager As ISearchObjective
        Private m_bIsMaxByFleetValue As Boolean = False

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
            Me.m_bIsMaxByFleetValue = False
        End Sub

        Public Property Manager() As ISearchObjective
            Get
                Return Me.m_Manager
            End Get
            Set(value As ISearchObjective)
                Me.m_Manager = value
                Me.RefreshContent()
            End Set
        End Property

        Public Property IsMaximizeByFleetValue() As Boolean
            Get
                Return Me.m_bIsMaxByFleetValue
            End Get
            Set(value As Boolean)
                Me.m_bIsMaxByFleetValue = value
                Me.RefreshContent()
            End Set
        End Property

#Region " Overrides "

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            If Not Me.m_bIsMaxByFleetValue Then
                Me.Redim(1, 3)
                Me(0, 0) = New cEwEColumnHeaderCell("")
                Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEET)
                Me(0, 2) = New cEwEColumnHeaderCell(My.Resources.FPS_FG_JOBS)
            Else
                Me.Redim(1, 4)
                Me(0, 0) = New cEwEColumnHeaderCell("")
                Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEET)
                Me(0, 2) = New cEwEColumnHeaderCell(My.Resources.FPS_FG_JOBS)
                Me(0, 3) = New cEwEColumnHeaderCell(My.Resources.FPS_FG_TP)
            End If

        End Sub

        Protected Overrides Sub FillData()

            If (Me.Manager Is Nothing) Then Return
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreGroupBase = Nothing

            For i As Integer = 1 To Me.UIContext.Core.nFleets
                source = Me.m_Manager.FleetObjectives(i)
                Me.Rows.Insert(i)

                If Not Me.m_bIsMaxByFleetValue Then
                    Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                    Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(i, 2) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSFleetJobCatchValue)
                Else
                    Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                    Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(i, 2) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSFleetJobCatchValue)
                    Me(i, 3) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.FPSFleetTargetProfit)
                End If
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
            Me.Columns(0).Width = 20
        End Sub

#End Region ' Overrides

    End Class

End Namespace


