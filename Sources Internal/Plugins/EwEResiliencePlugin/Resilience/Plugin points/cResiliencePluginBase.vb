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

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public MustInherit Class cResiliencePluginBase
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IDisposedPlugin

    Protected m_uic As cUIContext = Nothing
    Protected m_core As cCore = Nothing

#Region " UIC integration "

    Public Sub UIContext(uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext
        Try
            Me.m_uic = CType(uic, cUIContext)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' UIC integration

#Region " Generic plug-in bits "

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "F. Arreguín-Sánchez, M. Zetina-Rejon, J. Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:farregui@ipn.mx"
        End Get
    End Property

    Public Overridable ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for the EwE6 software to estimate, save and display resilience, as demonstrated in 'Measuring resilience in aquatic trophic networks from supply–demand-of-energy relationships'"
        End Get
    End Property

    Public Overridable Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception

        End Try
    End Sub

    Public MustOverride ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name

#End Region ' Generic plug-in bits

    Protected Overridable Sub Dispose() Implements EwEPlugin.IDisposedPlugin.Dispose

    End Sub

End Class
