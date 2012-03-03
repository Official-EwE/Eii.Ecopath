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

Imports EwECore.ValueWrapper
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Data for one time series contained in an Ecosim scenario.
''' </summary>
''' <remarks>
''' This class is implemented as a <see cref="cShapeData">cShapeData</see>.
''' </remarks>
''' ---------------------------------------------------------------------------
Public MustInherit Class cTimeSeries
    Inherits cShapeData

#Region " Protected variables "

    ''' <summary>The <see cref="eTimeSeriesType">type</see> of a time series.</summary>
    Protected m_timeSeriesType As eTimeSeriesType = eTimeSeriesType.NotSet
    ''' <summary>The weight of time for a time series.</summary>
    Protected m_sWtType As Single = 1.0!
    ''' <summary>Covariance.</summary>
    Protected m_sCV As Single = 0
    ''' <summary>The index of the target that a time series applies to.</summary>
    Protected m_iDatPool As Integer = 0
    ''' <summary>Applied flag</summary>
    Protected m_bEnabled As Boolean = False
    ''' <summary>Sum of squares for a TS.</summary>
    Protected m_sDatSS As Single = 0.0!
    ''' <summary>Average zstat sumof(Log(observed/predicted))/nobs.</summary>
    Protected m_sDataQ As Single = 0.0!

    ''' <summary>exp(DataQ)</summary>
    Protected m_eDataQ As Single

    ''' <summary>The core a TS belongs to.</summary>
    Protected m_core As cCore = Nothing

    Protected m_status As eStatusFlags = eStatusFlags.Null
    Protected m_strStatus As String = ""

#End Region ' Protected variables

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByRef core As cCore, ByVal DBID As Integer)
        MyBase.New(0)

        Me.m_core = core
        Me.m_datatype = eDataTypes.NotSet
        Me.DBID = DBID

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eTimeSeriesType">type</see> of a time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property TimeSeriesType() As eTimeSeriesType
        Get
            Return Me.m_timeSeriesType
        End Get

        Set(ByVal tstype As eTimeSeriesType)
            ' It is not allowed to switch between group- and fleet based TS once a type has been assigned
            Dim tscatCurr As eTimeSeriesCategoryType = cTimeSeriesFactory.TimeSeriesCategory(Me.m_timeSeriesType)
            Select Case tscatCurr
                Case eTimeSeriesCategoryType.NotSet
                    Me.m_timeSeriesType = tstype
                Case Else
                    If cTimeSeriesFactory.TimeSeriesCategory(tstype) = tscatCurr Then
                        Me.m_timeSeriesType = tstype
                    Else
                        Debug.Assert(False, "Illegal assignment; a TS cannot switch categories")
                    End If
            End Select
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the CV for a time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CV As Single
        Get
            Return Me.m_sCV
        End Get
        Set(value As Single)
            Me.m_sCV = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the weight of time for a time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property WtType() As Single
        Get
            Return Me.m_sWtType
        End Get

        Set(ByVal sWtType As Single)
            Me.m_sWtType = sWtType
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the target that a time series applies to. The
    ''' type of the target is implied by the <see cref="TimeSeriesType">type</see>
    ''' of the time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DatPool() As Integer
        Get
            Return Me.m_iDatPool
        End Get

        Set(ByVal iDatPool As Integer)
            Me.m_iDatPool = iDatPool
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the annual values for a time series.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DatVal(ByVal iIndex As Integer) As Single
        Get
            Return CSng(Me.ShapeData(iIndex))
        End Get

        Set(ByVal sValue As Single)
            Me.ShapeData(iIndex) = sValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the apply flag on the Time Series. Call <see cref="cCore.UpdateTimeSeries">cCore.UpdateTimeSeries</see>
    ''' to enable all flagged time series to the Ecosim model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Enabled() As Boolean
        Get
            Return (Me.m_bEnabled) And (Me.m_status = eStatusFlags.OK)
        End Get

        Set(ByVal bEnable As Boolean)
            Me.m_bEnabled = bEnable
        End Set
    End Property


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sum of squares for the fit of a data set to the predicted value DatSS
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DataSS() As Single
        Get
            Return Me.m_sDatSS
        End Get

        Friend Set(ByVal sValue As Single)
            Me.m_sDatSS = sValue
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' average  zstat sumof(Log(observed/predicted))/nobs Datq
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property DataQ() As Single
        Get
            Return Me.m_sDataQ
        End Get

        Friend Set(ByVal sDataQ As Single)
            Me.m_sDataQ = sDataQ
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' exp(DataQ) average prediction error
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property eDataQ() As Single
        Get
            Return Me.m_eDataQ
        End Get

        Friend Set(ByVal eDataQ As Single)
            Me.m_eDataQ = eDataQ
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, call this to inform the EwE core that a Time Series has changed.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Update() As Boolean
        Try
            Me.m_core.onChanged(Me, eMessageType.DataModified)
            Return True
        Catch ex As Exception
            Debug.Assert(False, String.Format("Failed to update time series {0}", Me.Name))
            Return False
        End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states whether a time series is a reference series.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function IsReference() As Boolean
        Return (Me.m_timeSeriesType = eTimeSeriesType.BiomassRel) Or _
               (Me.m_timeSeriesType = eTimeSeriesType.BiomassAbs) Or _
               (Me.m_timeSeriesType = eTimeSeriesType.TotalMortality) Or _
               (Me.m_timeSeriesType = eTimeSeriesType.Catches) Or _
               (Me.m_timeSeriesType = eTimeSeriesType.CatchesForcing)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether a time series can be used.
    ''' </summary>
    ''' <returns>A <see cref="eStatusFlags"/> stating whether the time series
    ''' can be used.</returns>
    ''' -----------------------------------------------------------------------
    Public Property ValidationStatus() As eStatusFlags
        Get
            Return Me.m_status
        End Get
        Friend Set(ByVal value As eStatusFlags)
            Me.m_status = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set a textual message explaining the time series <see cref="ValidationStatus"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ValidationMessage() As String
        Get
            Return Me.m_strStatus
        End Get
        Friend Set(ByVal value As String)
            Me.m_strStatus = value
        End Set
    End Property

End Class
