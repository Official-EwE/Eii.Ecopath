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
#End Region ' Imports

''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for biomass averaged over the total modeled area.
''' </summary>
Public Class cMOTotalResultsDataSource
    Inherits cBiomassResultsDataSource
    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Return Me.m_spaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.OtherMortalityLoss, OneBasedIndex, TimeIndex)
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "OtherMortalityLoss"
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            Return My.Resources.CoreDefaults.ECOSPACE_REGAVG_DATA_M0
        End Get
    End Property

End Class