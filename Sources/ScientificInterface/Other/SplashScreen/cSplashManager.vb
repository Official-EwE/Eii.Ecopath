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
Imports System.Threading

#End Region ' Imports

Public Class cSplashManager

    Private ReadOnly m_context As SynchronizationContext
    Private ReadOnly m_parent As Form
    Private m_splash As frmSplash = Nothing

    Public Sub New(ByVal synchronizationContext As SynchronizationContext, parent As Form)
        Me.m_context = synchronizationContext
        Me.m_parent = parent
    End Sub

    Public Sub ShowSplash()
        Me.m_splash = New frmSplash()
        'Console.WriteLine("Opening")
        Me.m_context.Send(Sub(callback) Me.m_splash.ShowDialog(Me.m_parent), Nothing)
    End Sub

    Public Sub CloseSplash()
        'Console.WriteLine("Closing")
        If (Me.m_splash IsNot Nothing) Then Me.m_splash.Close()
    End Sub

End Class
