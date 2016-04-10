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
Imports System.Windows.Forms

''' <summary>
''' A very, very basic plug-in form.
''' </summary>
Public Class frmEwEPlugin

    Private m_plugin As cBaseWithInterfacePluginPoint

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

        If (Me.Core IsNot Nothing) Then

            Dim model As cEwEModel = Me.Core.EwEModel

        End If

    End Sub


    Public Sub Init(ByVal PluginPoint As cBaseWithInterfacePluginPoint)
        m_plugin = PluginPoint
    End Sub

    Private Sub m_btButton_Click(sender As System.Object, e As System.EventArgs) Handles m_btButton.Click
        Dim ValueFromTextBox As Single
        'Get the value form the textbox
        'textbox store values as a String 
        'We need to convert it to a Single
        Single.TryParse(Me.m_txtTextbox.Text, ValueFromTextBox)

        'Call the Plugin with the Value from the text box
        Me.m_plugin.DoSomething(ValueFromTextBox)

    End Sub

    Private Sub btnClickMe_Click(sender As System.Object, e As System.EventArgs) Handles btnClickMe.Click
        Me.m_plugin.OpenModel("")
    End Sub

End Class