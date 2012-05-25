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

    ''' <summary>
    ''' Class to assess compatibility of a dataset with the current spatial and temporal extent.
    ''' </summary>
    Public Class cDatasetCompatilibity

#Region " Private vars "

        ''' <summary>Number of time steps in assessment period.</summary>
        Private m_iNumTimeSteps As Integer = 0
        ''' <summary>Number of time steps with data.</summary>
        Private m_iNumTimeOverlap As Single = 0
        ''' <summary>Number of time steps with full spatial overlap.</summary>
        Private m_iNumFullSpatialOverlap As Single = 0
        ''' <summary>Number of time steps with partial spatial overlap.</summary>
        Private m_iNumPartialSpatialOverlap As Single = 0

#End Region ' Private vars

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Assess the compatibility of a <see cref="ISpatialDataSet"/> with a 
        ''' loaded Ecospace scenario.
        ''' </summary>
        ''' <param name="core">The core with a loaded Ecospace scenario.</param>
        ''' <param name="ds">The <see cref="ISpatialDataSet"/>to assess.</param>
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

        Public Enum eCompatibilityTypes As Integer
            Unknown
            TotalOverlap
            PartialOverlap
            NoOverlap
        End Enum

        Public ReadOnly Property TemporalCompatibility As eCompatibilityTypes
            Get
                If (Me.m_iNumTimeSteps = 0) Then Return eCompatibilityTypes.Unknown
                If (Me.m_iNumTimeOverlap = 0) Then Return eCompatibilityTypes.NoOverlap
                If (Me.m_iNumTimeOverlap < Me.m_iNumTimeSteps) Then Return eCompatibilityTypes.PartialOverlap
                Return eCompatibilityTypes.TotalOverlap
            End Get
        End Property

        Public ReadOnly Property SpatialCompatibility As eCompatibilityTypes
            Get
                If (Me.m_iNumTimeSteps = 0) Then Return eCompatibilityTypes.Unknown
                If (Me.m_iNumTimeOverlap = 0) Then Return eCompatibilityTypes.Unknown
                If (Me.m_iNumPartialSpatialOverlap = 0) Or (Me.m_iNumFullSpatialOverlap = 0) Then Return eCompatibilityTypes.NoOverlap
                If (Me.m_iNumFullSpatialOverlap < Me.m_iNumTimeOverlap) Then Return eCompatibilityTypes.PartialOverlap
                Return eCompatibilityTypes.TotalOverlap
            End Get
        End Property

        Public Overrides Function ToString() As String

            ' ToDo_JS: globalize this method

            Select Case Me.TemporalCompatibility
                Case eCompatibilityTypes.Unknown : Return "Unable to determine compatibility"
                Case eCompatibilityTypes.NoOverlap : Return "Dataset has no data for the given Ecospace run time"
            End Select
            Return String.Format("{0}% time steps covered; of which {1}% partial and {2}% total area overlap", _
                                 CInt(100 * Me.m_iNumTimeOverlap / Me.m_iNumTimeSteps), _
                                 CInt(Me.m_iNumPartialSpatialOverlap / Me.m_iNumTimeOverlap), _
                                 CInt(Me.m_iNumFullSpatialOverlap / Me.m_iNumTimeOverlap))
        End Function

#End Region ' Public access

#Region " Internals "

        Private Function Assess(core As cCore, ds As ISpatialDataSet, iTimeStart As Integer, iNumTimeSteps As Integer) As Boolean

            ' Special case for datasets without temporal range
            If (ds.TimeStart = Date.MinValue) And (ds.TimeEnd = Date.MaxValue) Then
                iNumTimeSteps = 1
            End If

            ' Initialize counters
            Me.m_iNumTimeSteps = iNumTimeSteps
            Me.m_iNumTimeOverlap = 0
            Me.m_iNumFullSpatialOverlap = 0
            Me.m_iNumPartialSpatialOverlap = 0

            ' Protect against improper use
            If (core.ActiveEcospaceScenarioIndex = -1) Then Return False
            If (Not ds.FractionIndexed = 0) Then Return False
            If (iNumTimeSteps = 0) Then Return False

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
                        If rcfEcospace.Contains(rcfMap) Then
                            Me.m_iNumFullSpatialOverlap += 1
                        ElseIf rcfEcospace.Contains(rcfMap) Then
                            Me.m_iNumPartialSpatialOverlap += 1
                        End If
                    End If
                End If
            Next

            Return True

        End Function

        ''' <summary>
        ''' Convert a lat/lon area into a vertically flipped rectangle for easy comparison.
        ''' </summary>
        ''' <param name="ptfTL"></param>
        ''' <param name="ptfBR"></param>
        ''' <returns></returns>
        Private Function ToRect(ptfTL As PointF, ptfBR As PointF) As RectangleF
            Return New RectangleF(ptfTL.X, ptfBR.Y, (ptfBR.X - ptfTL.X + 360) Mod 360, (ptfTL.Y - ptfBR.Y + 180) Mod 180)
        End Function

#End Region ' Internals

    End Class

End Namespace
