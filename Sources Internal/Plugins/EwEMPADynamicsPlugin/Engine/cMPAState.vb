Option Strict On
Imports System.Text
' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Imports System.Windows.Forms
Imports EwECore
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cMPAState

    Private m_core As cCore = Nothing
    Private m_bIsClosed() As CheckState
    Private m_bIsEnforced() As CheckState

    Public Sub New(core As cCore, mpa As cEcospaceMPA, timestamp As Date)

        ReDim Me.m_bIsClosed(cCore.N_MONTHS)
        ReDim Me.m_bIsEnforced(core.nFleets)

        Me.m_core = core
        Me.MPA = mpa
        Me.TimeStamp = timestamp

    End Sub

    Public Property IsClosed(iMonth As Integer) As CheckState
        Get
            iMonth = Math.Min(cCore.N_MONTHS, Math.Max(1, iMonth))
            Return Me.m_bIsClosed(iMonth)
        End Get
        Set(value As CheckState)
            iMonth = Math.Min(cCore.N_MONTHS, Math.Max(1, iMonth))
            Me.m_bIsClosed(iMonth) = value
        End Set
    End Property

    Public Property IsEnforced(iFleet As Integer) As CheckState
        Get
            iFleet = Math.Min(Me.m_core.nFleets, Math.Max(1, iFleet))
            Return Me.m_bIsEnforced(iFleet)
        End Get
        Set(value As CheckState)
            iFleet = Math.Min(Me.m_core.nFleets, Math.Max(1, iFleet))
            Me.m_bIsEnforced(iFleet) = value
        End Set
    End Property

    Public ReadOnly Property MPA As cEcospaceMPA = Nothing
    Public ReadOnly Property TimeStamp As Date

    Public Sub Load()
        For iMonth As Integer = 1 To cCore.N_MONTHS
            Me.IsClosed(iMonth) = If(Me.MPA.IsClosed(iMonth), CheckState.Checked, CheckState.Unchecked)
        Next
        For iFleet As Integer = 1 To Me.m_core.nFleets
            Dim fleet As cEcospaceFleetInput = Me.m_core.EcospaceFleetInputs(iFleet)
            ' Reverse thinking!
            Me.IsEnforced(iFleet) = If(fleet.MPAFishery(Me.MPA.Index), CheckState.Unchecked, CheckState.Checked)
        Next
    End Sub

    Public Sub Apply()
        Dim val As cValue = MPA.ValueDescriptor(eVarNameFlags.MPAMonth)
        Dim bValidation As Boolean = val.AllowValidation
        val.AllowValidation = False
        For iMonth As Integer = 1 To cCore.N_MONTHS
            If Me.IsClosed(iMonth) <> CheckState.Indeterminate Then
                Me.MPA.IsClosed(iMonth) = (Me.IsClosed(iMonth) = CheckState.Checked)
            End If
        Next
        val.AllowValidation = bValidation

        For iFleet As Integer = 1 To Me.m_core.nFleets
            If (Me.IsEnforced(iFleet) <> CheckState.Indeterminate) Then
                Dim fleet As cEcospaceFleetInput = Me.m_core.EcospaceFleetInputs(iFleet)
                bValidation = val.AllowValidation
                val.AllowValidation = False
                val = fleet.ValueDescriptor(eVarNameFlags.MPAFishery)
                ' Reverse thinking!
                fleet.MPAFishery(Me.MPA.Index) = (Me.IsEnforced(iFleet) = CheckState.Unchecked)
                val.AllowValidation = bValidation
            End If
        Next

        Me.m_core.onChanged(Me.MPA)

    End Sub

    Public Overrides Function ToString() As String

        Return Me.MPA.Name
        '' ToDo: globalize this method
        'Return cStringUtils.Localize("MPA '{0}' {1}, {2} ", Me.MPA.Name, Me.ClosureText(), Me.RegulationText())

    End Function

    Public Function ClosureState() As String

        ' ToDo: globalize this method

        Dim sb As New StringBuilder()
        Dim bIsClosed As Boolean = False
        Dim nLength As Integer = 0
        Dim nClosed As Integer = 0

        For i As Integer = 1 To cCore.N_MONTHS
            If Me.MPA.IsClosed(i) Then
                nClosed += 1

                If (bIsClosed = False) Then
                    bIsClosed = True
                    nLength = 0
                    If (sb.Length > 0) Then sb.Append(", ")
                    sb.Append(cDateUtils.GetMonthName(i, False))
                Else
                    nLength += 1
                    ' Peek ahead
                    Dim bTerminate As Boolean = False
                    If (i < cCore.N_MONTHS) Then
                        bTerminate = (Me.MPA.IsClosed(i + 1) = False)
                    Else
                        bTerminate = True
                    End If

                    If (bTerminate) Then
                        If (nLength >= 1) Then
                            sb.Append("-")
                            sb.Append(cDateUtils.GetMonthName(i, False))
                        End If
                    End If
                End If
            Else
                bIsClosed = False
            End If
        Next

        Select Case nClosed
            Case 0
                Return "open all year"
            Case cCore.N_MONTHS
                Return "closed all year"
        End Select
        Return sb.ToString()

    End Function

    Public Function RegulationState() As String

        ' ToDo: globalize this method

        Dim sb As New StringBuilder()
        Dim n As Integer = 0

        For i As Integer = 1 To Me.m_core.nFleets
            Dim fleet As cEcospaceFleetInput = Me.m_core.EcospaceFleetInputs(i)
            If Not fleet.MPAFishery(Me.MPA.Index) Then
                n += 1
                If (sb.Length > 0) Then sb.Append(", ")
                sb.Append(fleet.Name)
            End If
        Next
        Select Case n
            Case 0
                Return "open to all fleets"
            Case Me.m_core.nFleets
                Return "closed to all fleets"
        End Select
        Return sb.ToString()

    End Function

End Class
