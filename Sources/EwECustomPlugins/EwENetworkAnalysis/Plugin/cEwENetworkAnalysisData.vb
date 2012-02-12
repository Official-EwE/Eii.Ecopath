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
#Region " Imports "

Option Strict On
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Core

#End Region

Friend Class cEwENetworkAnalysisData
    Implements EwEPlugin.Data.IPluginData
    Implements INetworkAnalysisData

    Private m_strAssemblyName As String = ""
    Private m_strPluginName As String = ""
    Private m_assAscendancy(6, 5) As Single

    Public Sub New(ByVal strAssemblyName As String, ByVal strPluginName As String)
        Me.m_strAssemblyName = strAssemblyName
        Me.m_strPluginName = strPluginName
    End Sub

    Public ReadOnly Property AssemblyName() As String _
        Implements IPluginData.AssemblyName
        Get
            Return Me.m_strAssemblyName
        End Get
    End Property

    Public ReadOnly Property PluginName() As String _
        Implements IPluginData.PluginName
        Get
            Return Me.m_strPluginName
        End Get
    End Property

    Public ReadOnly Property Ascendancy() As Single(,) _
        Implements INetworkAnalysisData.Ascendancy
        Get
            Return Me.m_assAscendancy
        End Get
    End Property

    Public ReadOnly Property RunType() As IRunType _
        Implements IPluginData.RunType
        Get
            Return Nothing
        End Get
    End Property

End Class
