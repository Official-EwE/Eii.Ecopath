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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to pick the current Icon for EwE6.
''' </summary>
''' <remarks>
''' Thee hee hee...
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cEwEIcon

    Private Shared m_ico As Icon = Nothing
    Private Shared m_bTried As Boolean = False
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the current icon for EwE.
    ''' </summary>
    ''' <returns>The current icon for EwE, catered to important events.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function Current() As Icon

        If Not cEwEIcon.m_bTried Then

            Dim tf As String = cFileUtils.MakeTempFile(".ico")
            Try
                My.Computer.Network.DownloadFile(New Uri("http://ecopath.org/EwE/current.ico"), tf)
                cEwEIcon.m_ico = New Icon(tf)
            Catch ex As Exception
                ' OK
            End Try
            cEwEIcon.m_bTried = True
        End If

        If (cEwEIcon.m_ico IsNot Nothing) Then
            Return cEwEIcon.m_ico
        End If

        ' Prepare icon
        Select Case cDateUtils.GetNextEvent(15)
            Case cDateUtils.eNextEvent.Easter
                Return My.Resources.Ecopath3_easter
            Case cDateUtils.eNextEvent.Xmas
                Return My.Resources.Ecopath4_hohoho
            Case cDateUtils.eNextEvent.DagVanDeLiefde
                Return My.Resources.Ecopath6_joepie
        End Select
#If BETA = 1 Then
                Return My.Resources.Ecopath2_beta
#Else
        Return My.Resources.Ecopath0
#End If

    End Function

End Class
