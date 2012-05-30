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
Imports EwECore
Imports EwEUtils.SpatialData
Imports System.Text
Imports System.Drawing

#End Region ' Imports

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>Helper class for assessing the compatibility of a <see cref="ISpatialDataSet"/> 
    ''' with the spatial and temporal extent of the currently loaded Ecospace scenario.</para>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cDatasetCompatilibity

#Region " Private vars "

        ''' <summary>Number of time steps in assessment period.</summary>
        Private m_iNumTimeSteps As Integer = 0
        ''' <summary>Percentage if dataset indexed.</summary>
        Private m_iPercIndexed As Integer = 0
        ''' <summary>Number of time steps with data.</summary>
        Private m_iNumTimeOverlap As Integer = 0
        ''' <summary>Number of data time steps with full spatial overlap.</summary>
        Private m_iNumFullSpatialOverlap As Integer = 0
        ''' <summary>Number of data time steps with partial spatial overlap.</summary>
        Private m_iNumPartialSpatialOverlap As Integer = 0
        ''' <summary>Number of files that could not be loaded.</summary>
        Private m_iNumError As Integer = 0

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
            ' Assess the entire Ecospace run time
            Me.Assess(core, ds, 1, core.nEcospaceTimeSteps)
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
            ' Assess the entire Ecospace run time
            Me.Assess(core, ds, iTimeStart, iNumTimeSteps)
        End Sub

#End Region ' Construction

#Region " Public access "

        ''' <summary>Compatibility levels.</summary>
        Public Enum eCompatibilityTypes As Integer
            ''' <summary>Unknown compatiblity.</summary>
            Unknown
            ''' <summary>No overlap.</summary>
            NoOverlap
            ''' <summary>Patial overlap.</summary>
            PartialOverlap
            ''' <summary>Total overlap.</summary>
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
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.Unknown"/></term>
        ''' <description>Assessment failed; this happens when there is no Ecospace scenario loaded, or an assessment was made for 0 time steps.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.NoOverlap"/></term>
        ''' <description>The dataset does not contain any data for the time steps in the assessment period.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.PartialOverlap"/></term>
        ''' <description>The dataset contains data for one or more but not all time steps in the assessment period.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.TotalOverlap"/></term>
        ''' <description>The dataset contains data for all time steps in the assessment period.</description></item>
        ''' </list>
        ''' <seealso cref="SpatialCompatibility"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TemporalCompatibility As eCompatibilityTypes
            Get
                If (Me.m_iNumTimeOverlap = 0) Then Return eCompatibilityTypes.Unknown
                If (Me.m_iNumError = Me.m_iNumTimeOverlap) Then Return eCompatibilityTypes.Unknown
                If (Me.m_iNumTimeOverlap < Me.m_iNumTimeSteps) Then Return eCompatibilityTypes.PartialOverlap
                Return eCompatibilityTypes.TotalOverlap
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a measure of spatial compatibility between a dataset and a
        ''' loaded Ecospace scenario. Values are to be interpreted as follows:
        ''' <list type="table">
        ''' <listheader>
        ''' <term>Value</term><description>Meaning</description>
        ''' </listheader>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.Unknown"/></term>
        ''' <description>Assessment failed; this happens when there is no Ecospace scenario loaded, or an assessment was made for 0 time steps.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.NoOverlap"/></term>
        ''' <description>No data in the dataset spatially overlaps with the Ecospace scenario for the assessment period.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.PartialOverlap"/></term>
        ''' <description>Not all of the data in the dataset entirely covers the Ecospace scenario area for the assessment period.</description></item>
        ''' <item><term><see cref="cDatasetCompatilibity.eCompatibilityTypes.TotalOverlap"/></term>
        ''' <description>All of the data in the dataset entirely covers the Ecospace scenario area for the assessment period.</description></item>
        ''' </list>
        ''' <seealso cref="TemporalCompatibility"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property SpatialCompatibility As eCompatibilityTypes
            Get
                If (Me.m_iNumTimeOverlap = 0) Then Return eCompatibilityTypes.Unknown
                If (Me.m_iNumError = Me.m_iNumTimeOverlap) Then Return eCompatibilityTypes.Unknown
                If (Me.m_iNumPartialSpatialOverlap = 0) And (Me.m_iNumFullSpatialOverlap = 0) Then Return eCompatibilityTypes.NoOverlap
                If (Me.m_iNumFullSpatialOverlap < Me.m_iNumTimeOverlap) Then Return eCompatibilityTypes.PartialOverlap
                Return eCompatibilityTypes.TotalOverlap
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Percentage of the dataset that is indexed.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property PercentIndexed As Integer
            Get
                Return Me.m_iPercIndexed
            End Get
        End Property

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
                Return Me.m_iNumFullSpatialOverlap
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert the dataset compatibility assessment to a string.
        ''' </summary>
        ''' <returns>The dataset compatibility assessment, converted to a string.</returns>
        ''' -------------------------------------------------------------------
        Public Overrides Function ToString() As String

            ' Avoid divisions by zero
            Dim iNumTS As Integer = Math.Max(Me.m_iNumTimeSteps, 1)
            Dim iNumOverlap As Integer = Math.Max(Me.m_iNumTimeOverlap, 1)

            ' Errors first!
            If (Me.m_iNumError > 0) Then
                Return String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_NODATA, CInt(100 * Me.m_iNumError / iNumOverlap))
            End If

            Select Case Me.TemporalCompatibility

                Case eCompatibilityTypes.Unknown
                    Return My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_NOASSESSMENT

                Case eCompatibilityTypes.NoOverlap
                    Return String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_NOOVERLAP, Me.m_iPercIndexed)

            End Select

            Return String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_COMPATIBILITY, _
                                 Me.m_iPercIndexed, _
                                 CInt(100 * Me.m_iNumTimeOverlap / iNumTS), _
                                 CInt(100 * Me.m_iNumPartialSpatialOverlap / iNumOverlap), _
                                 CInt(100 * Me.m_iNumFullSpatialOverlap / iNumOverlap))
        End Function

#End Region ' Public access

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform assessment.
        ''' </summary>
        ''' <param name="core"></param>
        ''' <param name="ds"></param>
        ''' <param name="iTimeStart"></param>
        ''' <param name="iNumTimeSteps"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function Assess(core As cCore, ds As ISpatialDataSet, _
                                iTimeStart As Integer, iNumTimeSteps As Integer) As Boolean

            ' Special case for datasets without temporal range
            If (ds.TimeStart = Date.MinValue) Or (ds.TimeEnd = Date.MaxValue) Then
                iNumTimeSteps = 0
            End If

            ' Initialize counters
            Me.m_iPercIndexed = CInt(ds.FractionIndexed * 100)
            Me.m_iNumTimeSteps = iNumTimeSteps
            Me.m_iNumTimeOverlap = 0
            Me.m_iNumFullSpatialOverlap = 0
            Me.m_iNumPartialSpatialOverlap = 0

            ' Protect against improper use
            If (core.ActiveEcospaceScenarioIndex = -1) Then Return False
            If (iNumTimeSteps < 0) Then Return False

            Dim iTimeEnd As Integer = iTimeStart + iNumTimeSteps
            Dim rcfEcospace As RectangleF = Me.ToRect(core.EcospaceBasemap.PosTopLeft, core.EcospaceBasemap.PosBottomRight)
            Dim ptfMapTL As PointF = Nothing
            Dim ptfMapBR As PointF = Nothing
            Dim rcfMap As RectangleF = Nothing

            For iStep As Integer = iTimeStart To iTimeEnd
                Dim tm As DateTime = core.EcospaceTimestepToAbsoluteTime(iStep)
                If ds.HasDataAtT(tm) Then
                    Me.m_iNumTimeOverlap += 1
                    If ds.GetExtentAtT(tm, ptfMapTL, ptfMapBR) Then
                        rcfMap = Me.ToRect(ptfMapTL, ptfMapBR)
                        If rcfMap.Contains(rcfEcospace) Then
                            Me.m_iNumFullSpatialOverlap += 1
                        ElseIf rcfMap.IntersectsWith(rcfEcospace) Then
                            Me.m_iNumPartialSpatialOverlap += 1
                        End If
                    Else
                        Me.m_iNumError += 1
                    End If
                End If
            Next

            Return True

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a lat/lon area into a vertically flipped rectangle for easy comparison.
        ''' </summary>
        ''' <param name="ptfTL"></param>
        ''' <param name="ptfBR"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Private Function ToRect(ptfTL As PointF, ptfBR As PointF) As RectangleF
            Return New RectangleF(ptfTL.X, ptfBR.Y, (ptfBR.X - ptfTL.X + 360) Mod 360, (ptfTL.Y - ptfBR.Y + 180) Mod 180)
        End Function

#End Region ' Internals

    End Class

End Namespace
