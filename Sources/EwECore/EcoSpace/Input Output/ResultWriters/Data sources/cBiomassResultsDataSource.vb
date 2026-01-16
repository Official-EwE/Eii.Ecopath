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




Imports EwECore.Style
Imports EwEUtils.Utilities



''' <summary>
''' Implementation of <see cref="cEcospaceResultsWriterDataSourceBase">cResultsDataSourceBase</see> for biomass averaged over the total modeled area.
''' </summary>
Public Class cBiomassResultsDataSource
    Inherits cEcospaceResultsWriterDataSourceBase

    Sub New(Core As cCore, EcospaceData As cEcospaceDataStructures)
        MyBase.New(Core, EcospaceData)
    End Sub

    Public Overrides Function GetResult(OneBasedIndex As Integer, TimeIndex As Integer) As Single
        Return Me.m_spaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.Biomass, OneBasedIndex, TimeIndex)
    End Function

    Public Overrides Sub Init(Optional OptionalIndex As Integer = 0)

    End Sub

    Public Overrides ReadOnly Property nResults As Integer
        Get
            Return Me.m_core.nGroups
        End Get
    End Property

    Public Overrides Function FieldName(OneBasedIndex As Integer) As String
        Return Me.m_core.m_EcopathData.GroupName(OneBasedIndex)
    End Function

    Public Overrides ReadOnly Property FilenameIdentifier As String
        Get
            Return "Biomass"
        End Get
    End Property

    Public Overrides ReadOnly Property DataDescriptor As String
        Get
            Dim u As New cUnits(Me.m_core)
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_AVG_B_UNIT, u.ToString(cUnits.Currency))
        End Get
    End Property

    Public Overrides ReadOnly Property AreaDescriptor As String
        Get
            Dim u As New cUnits(Me.m_core)
            Return cStringUtils.Localize(My.Resources.CoreDefaults.ECOSPACE_AREA_UNIT, u.ToString(cUnits.Area))
        End Get
    End Property

    Public Overrides ReadOnly Property nWaterCells As Integer
        Get
            Return Me.m_spaceData.RegionCells(0)
        End Get
    End Property

    Public Overrides ReadOnly Property AreaIndex As Integer
        Get
            Return 0
        End Get
    End Property

End Class