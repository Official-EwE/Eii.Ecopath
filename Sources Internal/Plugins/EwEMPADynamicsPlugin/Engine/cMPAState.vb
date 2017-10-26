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

Option Strict On
Imports System.Text
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cMPAState

    Private m_ds As cEcospaceDataStructures = Nothing
    Private m_bIsClosed() As CheckState
    Private m_bIsEnforced() As CheckState

    Public Sub New(ds As cEcospaceDataStructures, iMPA As Integer, timestamp As Date)

        Me.m_ds = ds
        Me.MPA = iMPA
        Me.TimeStamp = timestamp

        ReDim Me.m_bIsClosed(cCore.N_MONTHS)
        ReDim Me.m_bIsEnforced(ds.nFleets)

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
            iFleet = Math.Min(Me.m_ds.nFleets, Math.Max(1, iFleet))
            Return Me.m_bIsEnforced(iFleet)
        End Get
        Set(value As CheckState)
            iFleet = Math.Min(Me.m_ds.nFleets, Math.Max(1, iFleet))
            Me.m_bIsEnforced(iFleet) = value
        End Set
    End Property

    Public ReadOnly Property MPA As Integer = Nothing
    Public ReadOnly Property TimeStamp As Date

    Public Sub Load()
        For iMonth As Integer = 1 To cCore.N_MONTHS
            ' Reverse thinking!
            Me.IsClosed(iMonth) = If(Me.m_ds.MPAmonth(iMonth, Me.MPA), CheckState.Unchecked, CheckState.Checked)
        Next
        For iFleet As Integer = 1 To Me.m_ds.nFleets
            ' Reverse thinking!
            Me.IsEnforced(iFleet) = If(Me.m_ds.MPAfishery(iFleet, Me.MPA), CheckState.Unchecked, CheckState.Checked)
        Next
    End Sub

    Public Sub Apply()
        For iMonth As Integer = 1 To cCore.N_MONTHS
            If (Me.IsClosed(iMonth) <> CheckState.Indeterminate) Then
                ' Reverse thinking!
                Me.m_ds.MPAmonth(iMonth, Me.MPA) = (Me.IsClosed(iMonth) = CheckState.Unchecked)
            End If
        Next

        For iFleet As Integer = 1 To Me.m_ds.nFleets
            If (Me.IsEnforced(iFleet) <> CheckState.Indeterminate) Then
                ' Reverse thinking!
                Me.m_ds.MPAfishery(iFleet, Me.MPA) = (Me.IsEnforced(iFleet) = CheckState.Unchecked)
            End If
        Next

    End Sub

    Public Overrides Function ToString() As String
        Return Me.m_ds.MPAname(Me.MPA)
    End Function

    Public Function ClosureState() As String

        Dim sb As New StringBuilder()
        Dim bIsClosed As Boolean = False
        Dim nLength As Integer = 0
        Dim nClosed As Integer = 0

        For iMonth As Integer = 1 To cCore.N_MONTHS
            If Me.m_ds.MPAmonth(iMonth, Me.MPA) Then
                nClosed += 1

                If (bIsClosed = False) Then
                    bIsClosed = True
                    nLength = 0
                    If (sb.Length > 0) Then sb.Append(", ")
                    sb.Append(cDateUtils.GetMonthName(iMonth, False))
                Else
                    nLength += 1
                    ' Peek ahead
                    Dim bTerminate As Boolean = False
                    If (iMonth < cCore.N_MONTHS) Then
                        bTerminate = (Me.m_ds.MPAmonth(iMonth + 1, Me.MPA) = False)
                    Else
                        bTerminate = True
                    End If

                    If (bTerminate) Then
                        If (nLength >= 1) Then
                            sb.Append("-")
                            sb.Append(cDateUtils.GetMonthName(iMonth, False))
                        End If
                    End If
                End If
            Else
                bIsClosed = False
            End If
        Next

        Select Case nClosed
            Case 0
                Return My.Resources.STATUS_YEAR_OPEN
            Case cCore.N_MONTHS
                Return My.Resources.STATUS_YEAR_CLOSED
        End Select
        Return sb.ToString()

    End Function

    Public Function RegulationState() As String

        Dim sb As New StringBuilder()
        Dim n As Integer = 0

        For iFleet As Integer = 1 To Me.m_ds.nFleets
            If Not Me.m_ds.MPAfishery(iFleet, Me.MPA) Then
                n += 1
                If (sb.Length > 0) Then sb.Append(", ")
                sb.Append(iFleet)
            End If
        Next
        Select Case n
            Case 0
                Return My.Resources.STATUS_FLEETS_OPEN
            Case Me.m_ds.nFleets
                Return My.Resources.STATUS_FLEETS_CLOSED
        End Select
        Return sb.ToString()

    End Function

End Class
