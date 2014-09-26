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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEPlugin.Data
Imports EwEUtils.Utilities
Imports EwECore

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to distribute search results
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cOBISTaxonData
    Inherits cTaxonSearchData
    Implements IPluginData

#Region " Privates "

    ' -- Plugin connection --
    Private m_strAssemblyName As String = ""
    Private m_strPluginName As String = ""

#End Region ' Privates

#Region " Constructor "

    Public Sub New(ByVal strAssemblyName As String, _
                   ByVal strPluginName As String)
        MyBase.New(strPluginName)
        Me.m_strAssemblyName = strAssemblyName
        Me.m_strPluginName = strPluginName
    End Sub

#End Region ' Constructor

#Region " Properties "

    ''' <inheritdocs cref="IPluginData.AssemblyName"/>
    Public ReadOnly Property AssemblyName() As String _
        Implements EwEPlugin.Data.IPluginData.AssemblyName
        Get
            Return Me.m_strAssemblyName
        End Get
    End Property

    ''' <inheritdocs cref="IPluginData.PluginName"/>
    Public ReadOnly Property PluginName() As String _
        Implements EwEPlugin.Data.IPluginData.PluginName
        Get
            Return Me.m_strPluginName
        End Get
    End Property

    ''' <inheritdocs cref="IPluginData.RunType"/>
    Public ReadOnly Property RunType() As IRunType _
        Implements EwEPlugin.Data.IPluginData.RunType
        Get
            Return Nothing
        End Get
    End Property

#End Region ' Properties

End Class
