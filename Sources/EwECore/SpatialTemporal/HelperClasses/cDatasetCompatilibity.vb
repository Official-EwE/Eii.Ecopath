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
Imports EwEUtils.SpatialData
Imports System.Text
Imports System.Drawing

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>Helper class for assessing the compatibility of a <see cref="ISpatialDataSet"/> 
    ''' with the spatial and temporal extent of the currently loaded Ecospace scenario.</para>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cDatasetCompatilibity

#Region " Private vars "

        Private m_core As cCore = Nothing
        Private m_ds As ISpatialDataSet = Nothing
        ''' <summary>Spatial bounds of the current base map.</summary>
        Private m_rcfBasemap As RectangleF = Nothing

        ' -- Time step assessment period --
        Private m_iFirstTimeStep As Integer = 0
        Private m_iLastTimeStep As Integer = 0

        ''' <summary>Number of time steps in assessment period.</summary>
        Private m_iNumTimeSteps As Integer = 0
        ''' <summary>Number of time steps with data.</summary>
        Private m_iNumTimeOverlap As Integer = 0
        ''' <summary>Number of data time steps with full spatial overlap.</summary>
        Private m_iNumFullSpatialOverlap As Integer = 0
        ''' <summary>Number of data time steps with partial spatial overlap.</summary>
        Private m_iNumPartialSpatialOverlap As Integer = 0
        ''' <summary>Number of files that could not be loaded.</summary>
        Private m_iNumError As Integer = 0
        ''' <summary>Number of indexed files</summary>
        Private m_iNumIndexed As Integer = 0

#End Region ' Private vars

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Assess the compatibility of a <see cref="ISpatialDataSet"/> with a 
        ''' loaded Ecospace scenario.
        ''' </summary>
        ''' <param name="core">The core with a loaded Ecospace scenario.</param>
        ''' <param name="ds">The <see cref="ISpatialDataSet"/>to assess.</param>
        ''' <remarks>
        ''' This method will make an assessment of the full Ecospace run time.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore, ds As ISpatialDataSet)

            ' Sanity checks
            Debug.Assert(core IsNot Nothing)
            Debug.Assert(ds IsNot Nothing)

            Me.m_core = core
            Me.m_ds = ds

            If Not core.StateMonitor.HasEcospaceLoaded Then Return

            Me.m_rcfBasemap = Me.ToRect(Me.m_core.EcospaceBasemap.PosTopLeft, Me.m_core.EcospaceBasemap.PosBottomRight)

            Dim iNumTimeSteps As Integer = core.nEcospaceTimeSteps
            ' Special case for datasets without temporal range
            If (ds.TimeStart = Date.MinValue) Or (ds.TimeEnd = Date.MaxValue) Then
                iNumTimeSteps = 0
            End If

            ' Assess the entire Ecospace run time
            Me.Assess(1, iNumTimeSteps)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Assess the compatibility of a <see cref="ISpatialDataSet"/> with a 
        ''' loaded Ecospace scenario.
        ''' </summary>
        ''' <param name="core">The core with a loaded Ecospace scenario.</param>
        ''' <param name="ds">The <see cref="ISpatialDataSet"/>to assess.</param>
        ''' <param name="iNumTimeSteps">One-based Ecospace time step for the
        ''' assessement.</param>
        ''' <param name="iTimeStart">Number of time steps to assess.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore, ds As ISpatialDataSet, iTimeStart As Integer, iNumTimeSteps As Integer)
            ' Sanity checks
            Debug.Assert(core IsNot Nothing)
            Debug.Assert(ds IsNot Nothing)

            Me.m_core = core
            Me.m_ds = ds

            ' Assess the entire Ecospace run time
            Me.Assess(iTimeStart, iNumTimeSteps)
        End Sub

#End Region ' Construction

#Region " Public access "

        ''' <summary>Compatibility levels.</summary>
        Public Enum eCompatibilityTypes As Integer
            ''' <summary>Assessment has not been performed yet.</summary>
            NotSet = 0
            ''' <summary>Errors occurred while assessing the compatibility.</summary>
            ''' <remarks>The number of erroneous data sets can be checked via <see cref="NumError"/>.</remarks>
            Errors
            ''' <summary>No temporal overlap.</summary>
            NoTemporal
            ''' <summary>Temporal overlap, spatial unknown because indexing has not been performed.</summary>
            TemporalNotIndexed
            ''' <summary>Temporal overlap, but no spatial overlap.</summary>
            NoSpatial
            ''' <summary>Temporal overlap, but patial overlap.</summary>
            PartialSpatial
            ''' <summary>Temporal and total spatial overlap.</summary>
            TotalOverlap
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' <para>
        ''' Get a measure of temporal compatibility between a dataset and a
        ''' loaded Ecospace scenario. Values are to be interpreted as follows:
        ''' </para>
        ''' <list type="table">
        ''' <listheader>
        ''' <term>Value</term><description>Meaning</description>
        ''' </listheader>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.Errors"/></term>
        ''' <description>Assessment failed; this happens when there is no Ecospace scenario loaded, or any external data could not be accessed.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.NoTemporal"/></term>
        ''' <description>Thew dataset does not contain any data for the Ecospace run time period.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.NoSpatial"/></term>
        ''' <description>The dataset contains data for Ecospace time steps, but the data does not spatially overlap.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.PartialSpatial"/></term>
        ''' <description>The dataset contains data for Ecospace time steps, but the data partially spatially overlaps.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.TotalOverlap"/></term>
        ''' <description>The dataset contains data for Ecospace time steps, and all data spatially overlaps.</description></item>
        ''' </list>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Compatibility As eCompatibilityTypes
            Get
                If (Me.m_iNumError > 0) Then Return eCompatibilityTypes.Errors
                If (Me.m_iNumTimeOverlap = 0) Then Return eCompatibilityTypes.NoTemporal
                If (Me.m_iNumIndexed = 0) Then Return eCompatibilityTypes.TemporalNotIndexed
                If (Me.m_iNumFullSpatialOverlap = 0 And Me.m_iNumPartialSpatialOverlap = 0) Then Return eCompatibilityTypes.NoSpatial
                If (Me.m_iNumFullSpatialOverlap < Me.m_iNumTimeOverlap) Then Return eCompatibilityTypes.PartialSpatial
                Return eCompatibilityTypes.TotalOverlap
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="iTimeStep">The Ecospace time step to check compatibility for.</param>
        ''' <returns>
        ''' <para>Return values should be interpreted as follows:</para>
        ''' </returns>
        Public Function CompatibilityAt(iTimeStep As Integer) As eCompatibilityTypes

            Dim tm As DateTime = Me.m_core.EcospaceTimestepToAbsoluteTime(iTimeStep)
            Dim ptfMapTL As PointF = Nothing
            Dim ptfMapBR As PointF = Nothing
            Dim rcfMap As RectangleF = Nothing

            If Not Me.m_ds.HasDataAtT(tm) Then
                Return eCompatibilityTypes.NoTemporal
            End If

            Select Case Me.m_ds.IndexStatusAtT(tm)

                Case ISpatialDataSet.eIndexStatus.NotIndexed
                    Return eCompatibilityTypes.TemporalNotIndexed

                Case ISpatialDataSet.eIndexStatus.Failed
                    Return eCompatibilityTypes.Errors

                Case ISpatialDataSet.eIndexStatus.Indexed
                    If Me.m_ds.GetExtentAtT(tm, ptfMapTL, ptfMapBR) Then
                        rcfMap = Me.ToRect(ptfMapTL, ptfMapBR)
                        If rcfMap.Contains(Me.m_rcfBasemap) Then
                            Return eCompatibilityTypes.TotalOverlap
                        ElseIf rcfMap.IntersectsWith(Me.m_rcfBasemap) Then
                            Return eCompatibilityTypes.PartialSpatial
                        End If
                        Return eCompatibilityTypes.NoSpatial
                    End If
            End Select

            Return eCompatibilityTypes.Errors

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of assessed Ecospace time steps. If this method
        ''' returns 0 an error occurred and the assessment is invalid.
        ''' <seealso cref="NumOverlappingTimeSteps"/>
        ''' <seealso cref="NumFullSpatialOverlap"/>
        ''' <seealso cref="NumPartialSpatialOverlap"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumAssessedTimeSteps As Integer
            Get
                Return Me.m_iNumTimeSteps
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of assessed time steps for which the data set
        ''' contains external data.
        ''' <seealso cref="NumAssessedTimeSteps"/>
        ''' <seealso cref="NumFullSpatialOverlap"/>
        ''' <seealso cref="NumPartialSpatialOverlap"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumOverlappingTimeSteps As Integer
            Get
                Return Me.m_iNumTimeOverlap
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of assessed time steps for which the data set
        ''' contains external data that partially - not fully - overlaps
        ''' the area of the Ecospace scenario.
        ''' <seealso cref="NumOverlappingTimeSteps"/>
        ''' <seealso cref="NumFullSpatialOverlap"/>
        ''' <seealso cref="NumAssessedTimeSteps"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumPartialSpatialOverlap As Integer
            Get
                Return Me.m_iNumPartialSpatialOverlap
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of assessed time steps for which the data set
        ''' contains external data that fully - not partially - overlaps
        ''' the area of the Ecospace scenario.
        ''' <seealso cref="NumOverlappingTimeSteps"/>
        ''' <seealso cref="NumAssessedTimeSteps"/>
        ''' <seealso cref="NumPartialSpatialOverlap"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumFullSpatialOverlap As Integer
            Get
                Return Me.m_iNumFullSpatialOverlap
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of assessed time steps for which no data could be
        ''' loaded.
        ''' <seealso cref="NumOverlappingTimeSteps"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property NumError As Integer
            Get
                Return Me.m_iNumError
            End Get
        End Property

        Public ReadOnly Property NumIndexed As Integer
            Get
                Return Me.m_iNumIndexed
            End Get
        End Property

        Public ReadOnly Property FirstTimeStep As Integer
            Get
                Return Me.m_iFirstTimeStep
            End Get
        End Property

        Public ReadOnly Property LastTimeStep As Integer
            Get
                Return Me.m_iLastTimeStep
            End Get
        End Property

        Public ReadOnly Property PercentIndexed As Integer
            Get
                If (Me.m_iNumTimeOverlap > 0) Then
                    Return CInt(Math.Ceiling(100 * Me.m_iNumIndexed / Me.m_iNumTimeOverlap))
                End If
                Return 0
            End Get
        End Property

        Public ReadOnly Property Status As eStatusFlags
            Get
                Return Me.ToStatus(Me.Compatibility)
            End Get
        End Property

        Public ReadOnly Property StatusAt(iTimeStep As Integer) As eStatusFlags
            Get
                Return Me.ToStatus(Me.CompatibilityAt(iTimeStep))
            End Get
        End Property

#End Region ' Public access

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform the assessment of a dataset for a given number of time steps.
        ''' </summary>
        ''' <param name="iTimeStart"></param>
        ''' <param name="iNumTimeSteps"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function Assess(iTimeStart As Integer, iNumTimeSteps As Integer) As Boolean

            ' Store assessment period
            Me.m_iFirstTimeStep = iTimeStart
            Me.m_iLastTimeStep = iTimeStart + iNumTimeSteps - 1

            ' Initialize counters
            Me.m_iNumTimeSteps = iNumTimeSteps
            Me.m_iNumTimeOverlap = 0
            Me.m_iNumIndexed = 0
            Me.m_iNumFullSpatialOverlap = 0
            Me.m_iNumPartialSpatialOverlap = 0

            ' Protect against improper use
            If (Me.m_core.ActiveEcospaceScenarioIndex = -1) Then Return False
            If (iNumTimeSteps < 0) Then Return False

            Dim iTimeEnd As Integer = iTimeStart + iNumTimeSteps - 1

            For iStep As Integer = iTimeStart To iTimeEnd

                Select Case CompatibilityAt(iStep)

                    Case eCompatibilityTypes.NoTemporal
                        ' No temporal overlap.

                    Case eCompatibilityTypes.NotSet, _
                         eCompatibilityTypes.TemporalNotIndexed
                        ' Compatibility for this time step has not been assessed.
                        ' Just count the time step, make no other assumptions.
                        Me.m_iNumTimeOverlap += 1

                    Case eCompatibilityTypes.NoSpatial
                        Me.m_iNumTimeOverlap += 1
                        Me.m_iNumIndexed += 1

                    Case eCompatibilityTypes.PartialSpatial
                        Me.m_iNumTimeOverlap += 1
                        Me.m_iNumIndexed += 1
                        Me.m_iNumPartialSpatialOverlap += 1

                    Case eCompatibilityTypes.TotalOverlap
                        Me.m_iNumTimeOverlap += 1
                        Me.m_iNumIndexed += 1
                        Me.m_iNumFullSpatialOverlap += 1

                    Case eCompatibilityTypes.Errors
                        Me.m_iNumTimeOverlap += 1
                        Me.m_iNumIndexed += 1
                        Me.m_iNumError += 1

                End Select

            Next

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a lat/lon area into an extent rectangle.
        ''' </summary>
        ''' <param name="ptfTL"></param>
        ''' <param name="ptfBR"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function ToRect(ptfTL As PointF, ptfBR As PointF) As RectangleF
            Return New RectangleF(ptfTL.X, ptfTL.Y, ptfBR.X - ptfTL.X, ptfBR.Y - ptfTL.Y)
        End Function

        Private Function ToStatus(flag As eCompatibilityTypes) As eStatusFlags
            Select Case flag
                Case eCompatibilityTypes.NotSet
                    Return eStatusFlags.Null
                Case eCompatibilityTypes.TemporalNotIndexed
                    Return eStatusFlags.Null
                Case eCompatibilityTypes.NoSpatial, eCompatibilityTypes.NoTemporal
                    Return eStatusFlags.MissingParameter
                Case eCompatibilityTypes.Errors
                    Return eStatusFlags.ErrorEncountered
                Case eCompatibilityTypes.PartialSpatial
                    Return eStatusFlags.MissingParameter
                Case eCompatibilityTypes.TotalOverlap
                    Return eStatusFlags.OK
            End Select
        End Function

#End Region ' Internals

    End Class

End Namespace
