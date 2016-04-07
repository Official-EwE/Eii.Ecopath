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
Imports EwEPlugin.Data
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Helper class to distribute search results
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cFishBaseTaxonData
    Inherits cTaxonSearchData
    Implements IPluginData

#Region " Privates "

    ' -- ID --
    Private m_lCodeSAUP As Long = 0
    Private m_lCodeFB As Long = 0
    Private m_lCodeSLB As Long = 0
    Private m_strCodeLSID As String = ""
    Private m_strCodeFAO As String = ""

    ' -- Plugin connection --
    Private m_strPluginName As String = ""
    Private m_searchfields As eTaxonClassificationType

    ' -- Data --
    Private m_strCommon As String = ""
    Private m_strPhylum As String = ""
    Private m_strClass As String = ""
    Private m_strOrder As String = ""
    Private m_strFamily As String = ""
    Private m_strGenus As String = ""
    Private m_strSpecies As String = ""
    Private m_sNorth As Single = cCore.NULL_VALUE
    Private m_sSouth As Single = cCore.NULL_VALUE
    Private m_sEast As Single = cCore.NULL_VALUE
    Private m_sWest As Single = cCore.NULL_VALUE
    Private m_bExploited As Boolean = False
    Private m_ecology As eEcologyTypes = eEcologyTypes.NotSet
    Private m_conservation As eIUCNConservationStatusTypes = eIUCNConservationStatusTypes.NotSet
    Private m_occurrence As eOccurrenceStatusTypes = eOccurrenceStatusTypes.NotSet
    Private m_organism As eOrganismTypes = eOrganismTypes.Fishes
    Private m_sLastUpdated As Double = cDateUtils.DateToJulian(Date.Now())
    Private m_sMaxLength As Single = cCore.NULL_VALUE
    Private m_sMeanLength As Single = cCore.NULL_VALUE
    Private m_sMeanLifespan As Single = cCore.NULL_VALUE
    Private m_sMeanWeight As Single = cCore.NULL_VALUE
    Private m_sVulnerability As Single = cCore.NULL_VALUE
    Private m_dModified As Double

#End Region ' Privates

#Region " Constructor "

    Public Sub New(ByVal strPluginName As String)
        MyBase.New(strPluginName)
        Me.m_strPluginName = strPluginName
    End Sub

#End Region ' Constructor

#Region " Properties "

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
