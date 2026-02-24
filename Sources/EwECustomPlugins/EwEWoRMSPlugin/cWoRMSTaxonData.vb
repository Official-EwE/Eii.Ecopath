' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Common
Imports EwECore.Plugins.Data

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to distribute search results
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cWoRMSTaxonData
    Inherits cTaxonSearchData
    Implements IPluginData

#Region " Privates "

    Private m_strPluginName As String = ""

#End Region ' Privates

#Region " Constructor "

    Public Sub New(strPluginName As String)
        MyBase.New(strPluginName)
        Me.m_strPluginName = strPluginName
    End Sub

#End Region ' Constructor

#Region " Properties "

    ''' <inheritdocs cref="IPluginData.PluginName"/>
    Public ReadOnly Property PluginName() As String _
        Implements IPluginData.PluginName
        Get
            Return Me.m_strPluginName
        End Get
    End Property

    ''' <inheritdocs cref="IPluginData.RunType"/>
    Public ReadOnly Property RunType() As IRunType _
        Implements IPluginData.RunType
        Get
            Return Nothing
        End Get
    End Property

#End Region ' Properties

End Class
