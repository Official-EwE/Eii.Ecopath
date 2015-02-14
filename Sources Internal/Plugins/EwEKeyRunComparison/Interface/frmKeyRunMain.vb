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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

Imports EwECore
Imports System.Windows.Forms

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmKeyRunMain

    Private m_CompManager As cCompareManager

    Public Sub New()

        ' This call is required by the designer.
        Me.InitializeComponent()


    End Sub

    ''' <summary>
    ''' OnLoad is called when a form is about to go 'live'. It is the perfect place to
    ''' perform last moment configurations before the form is made visible to the user.
    ''' </summary>
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)


    End Sub

    Public Sub Init(ByVal ComparisonManager As cCompareManager)
        m_CompManager = ComparisonManager
    End Sub

    Private Sub m_btTest_Click(sender As Object, e As System.EventArgs) Handles m_btTest.Click

    End Sub

End Class