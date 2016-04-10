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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmEwEPlugin

    Public Sub New()

        ' This call is required by the designer.
        Me.InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

    End Sub

    ''' <summary>
    ''' OnLoad is called when a form is about to go 'live'. It is the perfect place to
    ''' perform last moment configurations before the form is made visible to the user.
    ''' </summary>
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.Core IsNot Nothing) Then

            ' Show name of model in the text box

            Dim model As cEwEModel = Me.Core.EwEModel
            Me.m_lblMessage.Text = "Hello " & model.Name & "!"

        End If

    End Sub

End Class