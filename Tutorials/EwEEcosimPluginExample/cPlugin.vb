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
Imports EwEPlugin
Imports EwECore

Public Class cPlugin
    Implements IEcosimEndTimestepPlugin
    Implements IEcospaceEndTimestepPlugin


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
            Return "Carl Christensen"
        End Get
    End Property

    Public ReadOnly Property Contact() As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "Nobody, please"
        End Get
    End Property

    Public ReadOnly Property Description() As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return Me.Name
        End Get
    End Property

    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "Ecosim plug-in example"
        End Get
    End Property

    Public Sub EcospaceEndTimeStep(ByVal EcospaceDatastructures As Object, ByVal iTime As Integer) _
        Implements EwEPlugin.IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

        If iTime = 13 Then

            Dim data As cEcospaceDataStructures = CType(EcospaceDatastructures, cEcospaceDataStructures)
            MsgBox("Ecospace run: Group 1 has a biomass of " & data.Bcell(1, 1, 1) & " in cell (1, 1) at time step " & iTime)

        End If

    End Sub
End Class
