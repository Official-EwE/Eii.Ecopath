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

Imports EwEPlugin
Imports EwECore

''' <summary>
''' Base code of a plug-in that monitors Ecosim time steps.
''' </summary>
''' <remarks>
''' <para>In order to run and test this plugin it must be integrated within the EwE6 scientific interface. 
''' To achieve this, add this project to the EwE6 solution, and reference this project from within the 
''' ScientificInterface. This ensures that your plug-in will be built with EwE6, and will be loaded by the 
''' EwE6 plug-in manager when you run EwE6.</para>
''' </remarks>
Public Class cEcosimPlugin
    Implements IEcosimEndTimestepPlugin

    Public Sub Initialize(ByVal core As Object) _
        Implements EwEPlugin.IPlugin.Initialize

        MsgBox(Me.Name & " loaded")

    End Sub

    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        If iTime = 42 Then
            MsgBox("Ecosim run: Group 1 has biomass " & BiomassAtTimestep(1) & " at time step " & iTime)
        End If

    End Sub

    Public ReadOnly Property Author() As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "your name"
        End Get
    End Property

    Public ReadOnly Property Contact() As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "your email"
        End Get
    End Property

    Public ReadOnly Property Description() As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "a description"
        End Get
    End Property

    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "Ecosim plug-in example"
        End Get
    End Property

End Class
