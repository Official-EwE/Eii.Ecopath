' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    
    Public Class gridMCRunInput
        : Inherits cEwEGrid

        Private m_value As eMCRunDisplayInputValueTypes = 0
        Private m_mcmanager As cMonteCarloManager = Nothing

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property DisplayInputValue() As eMCRunDisplayInputValueTypes
            Get
                Return Me.m_value
            End Get
            Set(value As eMCRunDisplayInputValueTypes)
                Me.m_value = value
                Me.RefreshContent()
            End Set
        End Property

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
                If (value IsNot Nothing) Then
                    Me.m_mcmanager = value.Core.EcosimMonteCarlo
                Else
                    Me.m_mcmanager = Nothing
                End If
                MyBase.UIContext = value
            End Set
        End Property

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim headers As String() = Me.DataColumnsHeaders()

            Me.Redim(Me.NumDataRows + 1, headers.Length)
            For i As Integer = 0 To Me.ColumnsCount - 1
                Me(0, i) = New cEwEColumnHeaderCell(headers(i))
            Next

            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub FillData()

            Select Case Me.m_value
                Case eMCRunDisplayInputValueTypes.B
                    Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcBcv, eVarNameFlags.mcBLower, eVarNameFlags.mcB, eVarNameFlags.mcBUpper})
                Case eMCRunDisplayInputValueTypes.PB
                    Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcPBcv, eVarNameFlags.mcPBLower, eVarNameFlags.mcPB, eVarNameFlags.mcPBUpper})
                Case eMCRunDisplayInputValueTypes.EE
                    Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcEEcv, eVarNameFlags.mcEELower, eVarNameFlags.mcEE, eVarNameFlags.mcEEUpper})
                Case eMCRunDisplayInputValueTypes.BA
                    Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcBAcv, eVarNameFlags.mcBALower, eVarNameFlags.mcBA, eVarNameFlags.mcBAUpper})
                Case eMCRunDisplayInputValueTypes.BaBi
                    Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcBaBicv, eVarNameFlags.mcBaBiLower, eVarNameFlags.mcBaBi, eVarNameFlags.mcBaBiUpper})
                Case eMCRunDisplayInputValueTypes.QB
                    Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcQBcv, eVarNameFlags.mcQBLower, eVarNameFlags.mcQB, eVarNameFlags.mcQBUpper})
                Case eMCRunDisplayInputValueTypes.VU
                    Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcVUcv, eVarNameFlags.mcVULower, eVarNameFlags.mcVU, eVarNameFlags.mcVUUpper})
                Case eMCRunDisplayInputValueTypes.Landings, eMCRunDisplayInputValueTypes.Discards
                    Me.FillLandingsDiscardsValues()
                Case eMCRunDisplayInputValueTypes.Diets
                    Select Case Me.m_mcmanager.DietSamplingMethod
                        Case eMCDietSamplingMethod.Dirichlets
                            Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcDietMult})
                        Case eMCDietSamplingMethod.NormalDistribution
                            Me.FillValues(New eVarNameFlags() {eVarNameFlags.mcDCcv})
                        Case Else
                            Debug.Assert(False)
                    End Select
            End Select

        End Sub

        Private Sub FillValues(flags() As eVarNameFlags)

            Dim mcGrp As cMonteCarloGroup = Nothing

            For i As Integer = 1 To Me.RowsCount - 1
                mcGrp = Me.m_mcmanager.Groups(i)
                Me(i, 0) = New cEwERowHeaderCell(CStr(mcGrp.Index))
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mcGrp, eVarNameFlags.Name)

                For j As Integer = 0 To flags.Length - 1
                    Me(i, 2 + j) = New cPropertyCell(Me.PropertyManager, mcGrp, flags(j))
                Next
            Next

        End Sub

        Private Sub FillLandingsDiscardsValues()

            Dim mcGrp As cMonteCarloGroup = Nothing
            Dim i As Integer = 1

            For igrp As Integer = 1 To Me.Core.nGroups
                mcGrp = Me.m_mcmanager.Groups(igrp)
                For iflt As Integer = 1 To Me.Core.nFleets
                    Dim var As eVarNameFlags = If(Me.m_value = eMCRunDisplayInputValueTypes.Landings, eVarNameFlags.Landings, eVarNameFlags.Discards)
                    Dim vars() As eVarNameFlags = If(Me.m_value = eMCRunDisplayInputValueTypes.Landings,
                                                                   New eVarNameFlags() {eVarNameFlags.mcLandingscv, eVarNameFlags.mcLandingsLower, eVarNameFlags.mcLandings, eVarNameFlags.mcLandingsUpper},
                                                                   New eVarNameFlags() {eVarNameFlags.mcDiscardscv, eVarNameFlags.mcDiscardsLower, eVarNameFlags.mcDiscards, eVarNameFlags.mcDiscardsUpper})
                    Dim flt As cEcopathFleetInput = Me.Core.EcopathFleetInputs(iflt)
                    Dim val As Single = CSng(flt.GetVariable(var, igrp))

                    If (val > 0) Then
                        Me(i, 0) = New cEwERowHeaderCell(CStr(mcGrp.Index))
                        Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mcGrp, eVarNameFlags.Name)
                        Me(i, 2) = New cPropertyRowHeaderCell(Me.PropertyManager, flt, eVarNameFlags.Name)
                        For j As Integer = 0 To vars.Length - 1
                            Me(i, 3 + j) = New cPropertyCell(Me.PropertyManager, mcGrp, vars(j), flt)
                        Next
                        i += 1
                    End If
                Next
            Next

        End Sub

        Private Function NumDataRows() As Integer
            Select Case Me.m_value
                Case eMCRunDisplayInputValueTypes.Landings
                    Dim n As Integer = 0
                    For j As Integer = 1 To Me.Core.nFleets
                        Dim flt As cEcopathFleetInput = Me.Core.EcopathFleetInputs(j)
                        For i As Integer = 1 To Me.Core.nGroups
                            If (flt.Landings(i) > 0) Then n += 1
                        Next i
                    Next j
                    Return n
                Case eMCRunDisplayInputValueTypes.Discards
                    Dim n As Integer = 0
                    For j As Integer = 1 To Me.Core.nFleets
                        Dim flt As cEcopathFleetInput = Me.Core.EcopathFleetInputs(j)
                        For i As Integer = 1 To Me.Core.nGroups
                            If (flt.Discards(i) > 0) Then n += 1
                        Next i
                    Next j
                    Return n
            End Select
            Return Me.Core.nLivingGroups
        End Function

        Private Function DataColumnsHeaders() As String()

            Select Case Me.m_value
                Case eMCRunDisplayInputValueTypes.Diets
                    Select Case Me.m_mcmanager.DietSamplingMethod
                        Case eMCDietSamplingMethod.Dirichlets
                            Return New String() {"", SharedResources.HEADER_GROUPNAME, My.Resources.HEADER_DIET_MULTIPLIER}
                        Case eMCDietSamplingMethod.NormalDistribution
                            Return New String() {"", SharedResources.HEADER_GROUPNAME, SharedResources.HEADER_CV}
                        Case Else
                            Debug.Assert(False)
                    End Select
                Case eMCRunDisplayInputValueTypes.Landings, eMCRunDisplayInputValueTypes.Discards
                    Return New String() {"", SharedResources.HEADER_GROUPNAME, SharedResources.HEADER_FLEETNAME, SharedResources.HEADER_CV, SharedResources.HEADER_LOWERLIMIT, SharedResources.HEADER_MEAN, SharedResources.HEADER_UPPERLIMIT}
                Case Else
                    Return New String() {"", SharedResources.HEADER_GROUPNAME, SharedResources.HEADER_CV, SharedResources.HEADER_LOWERLIMIT, SharedResources.HEADER_MEAN, SharedResources.HEADER_UPPERLIMIT}
            End Select

            Debug.Assert(False)
            Return Nothing

        End Function

    End Class

End Namespace


