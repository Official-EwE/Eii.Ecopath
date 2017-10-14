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
Imports EwECore
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cMPAState

    Private m_core As cCore = Nothing
    Private m_bIsClosed(cCore.N_MONTHS) As Boolean

    Public Sub New(core As cCore, mpa As cEcospaceMPA, timestamp As Date)
        Me.m_core = core
        Me.MPA = mpa
        Me.TimeStamp = timestamp
    End Sub

    Public Property IsClosed(iMonth As Integer) As Boolean
        Get
            iMonth = Math.Min(cCore.N_MONTHS, Math.Max(1, iMonth))
            Return Me.m_bIsClosed(iMonth)
        End Get
        Set(value As Boolean)
            iMonth = Math.Min(cCore.N_MONTHS, Math.Max(1, iMonth))
            Me.m_bIsClosed(iMonth) = value
        End Set
    End Property

    Public ReadOnly Property MPA As cEcospaceMPA = Nothing
    Public ReadOnly Property TimeStamp As Date

    Public Sub Load()
        For iMonth As Integer = 1 To cCore.N_MONTHS
            Me.IsClosed(iMonth) = Me.MPA.IsClosed(iMonth)
        Next
    End Sub

    Public Sub Apply()
        Dim val As cValue = MPA.ValueDescriptor(eVarNameFlags.MPAMonth)
        Dim bValidation As Boolean = val.AllowValidation
        val.AllowValidation = False
        For iMonth As Integer = 1 To cCore.N_MONTHS
            Me.MPA.IsClosed(iMonth) = Me.IsClosed(iMonth)
        Next
        val.AllowValidation = bValidation
        Me.m_core.onChanged(Me.MPA)
    End Sub

    Public Overrides Function ToString() As String

        Return cStringUtils.Localize("MPA '{0}' {1}", Me.MPA.Name, Me.ClosureText())

    End Function

    Public Function ClosureText() As String

        Dim sb As New StringBuilder()
        Dim bIsClosed As Boolean = False
        Dim nLength As Integer = 0

        For i As Integer = 1 To cCore.N_MONTHS
            If Me.IsClosed(i) Then
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
                        bTerminate = (Me.IsClosed(i + 1) = False)
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

        Return If(sb.Length = 0, "open all year", "closed " & sb.ToString())

    End Function

End Class
