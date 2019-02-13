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
Imports EwEPlugin
Imports EwECore

#End Region ' Imports

Public Class cLifespanPlugin
    Implements ILicensePlugin

    Private m_core As cCore = Nothing

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "DotSpatial.Maintenance"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return "Framework validation"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Validates the framework"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author As String _
            Implements EwEPlugin.IPlugin.Author
        Get
            Return "Ecopath International Initiative Research Association"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="EwEPlugin.IPlugin.Contact"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact As String _
            Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ecopathinternational@gmail.com"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public Sub Validate() Implements ILicensePlugin.Validate
        cDotSpatialUtils.IsLicensed(Me.m_core)
    End Sub

    Public Sub Expiry(ByRef dt As Date) Implements ILicensePlugin.Expiry
        dt = cDotSpatialUtils.ExpiryDate
    End Sub

End Class
