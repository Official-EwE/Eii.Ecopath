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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cEcospaceFisheryWrapper
    Inherits cCoreIOWrapperBase

    Public Sub New(core As cCore)
        MyBase.New(core)
    End Sub

    Public Overrides Sub Init()

        MyBase.Init()

        For iFlt As Integer = 1 To Me.m_core.nFleets
            Me.m_objects.Add(Me.Core.EcospaceFleets(iFlt))
        Next

        Me.m_variables.Add(eVarNameFlags.EffectivePower)
        Me.m_variables.Add(eVarNameFlags.SEmult)
        
    End Sub

    Public Overrides Function HashValues() As System.Collections.Generic.List(Of cHashValues)
        Return MyBase.getVarResults()
    End Function

#Region " Internals "

    Protected Overrides ReadOnly Property ObjectDescriptor As String
        Get
            Return "EcospaceFishery"
        End Get
    End Property

#End Region ' Internals

End Class
