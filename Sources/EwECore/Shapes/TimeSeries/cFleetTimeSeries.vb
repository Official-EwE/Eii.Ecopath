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
Imports EwEUtils.Core

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cFleetTimeSeries
    Inherits cTimeSeries

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(core As cCore, iDBID As Integer)
        MyBase.New(core, iDBID)
        Me.m_datatype = eDataTypes.FleetTimeSeries
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the fleet this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FleetIndex() As Integer
        Get
            Return Me.DatPool
        End Get
        Set(iFleet As Integer)
            Me.DatPool = iFleet
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the group index that a time series applies to. Group targets apply 
    ''' to fleet x group time series such as <see cref="eTimeSeriesType.DiscardMortality"/>
    ''' and <see cref="eTimeSeriesType.DiscardProportion"/>. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GroupIndex As Integer
        Get
            Return Me.DatPoolSec
        End Get
        Set(value As Integer)
            Me.DatPoolSec = value
        End Set
    End Property

    Public Overrides Function IsValid() As Boolean
        Return ((Me.FleetIndexStatus Or Me.GroupIndexStatus) And eStatusFlags.ErrorEncountered) = 0
    End Function

    Public ReadOnly Property FleetIndexStatus() As eStatusFlags
        Get
            If (Me.DatPool < 1 Or Me.DatPool > Me.m_core.nFleets) Then
                Return eStatusFlags.ErrorEncountered
            End If
            Return eStatusFlags.OK
        End Get
    End Property

    Public ReadOnly Property GroupIndexStatus() As eStatusFlags
        Get
            If (Me.m_timeSeriesType = eTimeSeriesType.DiscardMortality) Or (Me.m_timeSeriesType = eTimeSeriesType.DiscardProportion) Or
               (Me.m_timeSeriesType = eTimeSeriesType.Catchabilities) Or (Me.m_timeSeriesType = eTimeSeriesType.OffVesselPrice) Then
                If (Me.DatPoolSec < 1 Or Me.DatPoolSec > Me.m_core.nGroups) Then
                    Return eStatusFlags.ErrorEncountered
                End If
                Return eStatusFlags.OK
            End If
            Return eStatusFlags.Null
        End Get
    End Property

End Class
