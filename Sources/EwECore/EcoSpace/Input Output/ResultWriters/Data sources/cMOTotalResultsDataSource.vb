' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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