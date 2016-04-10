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

#End Region ' Imports

Public Class cResilienceData

    Public Structure sBounds

        ''' <summary>Minimum supply</summary>
        Public Property smin As Single
        ''' <summary>Maximum supply</summary>
        Public Property smax As Single
        ''' <summary>Minimum demand</summary>
        Public Property dmin As Single
        ''' <summary>Maximum demand</summary>
        Public Property dmax As Single

        Public Sub Init()
            Me.dmax = Single.MinValue
            Me.dmin = Single.MaxValue
            Me.smax = Single.MinValue
            Me.smin = Single.MaxValue
        End Sub

    End Structure

    Private m_nGroups As Integer = 0
    Private m_nTimes As Integer = 0
    Private m_nYears As Integer = 0
    Private m_boundsT As New sBounds
    Private m_boundsY As New sBounds
    Private m_bCalculated As Boolean = False

    Public Sub Resize(nGroups As Integer, nTimes As Integer, nYears As Integer)

        Me.m_nGroups = nGroups
        Me.m_nTimes = nTimes
        Me.m_nYears = nYears

        ReDim IsConsumer(nGroups)

        ReDim GroupDemandAtT(nGroups, nTimes)
        ReDim GroupDemandAtY(nGroups, nYears)
        ReDim GroupSupplyAtT(nGroups, nTimes)
        ReDim GroupSupplyAtY(nGroups, nYears)
        ReDim SlopeAtT(nTimes)
        ReDim SlopeAtY(nYears)
        ReDim InterceptAtT(nTimes)
        ReDim InterceptAtY(nYears)

        Me.m_bCalculated = False

    End Sub

    Public ReadOnly Property NumTimeSteps As Integer
        Get
            Return Me.m_nTimes
        End Get
    End Property

    Public ReadOnly Property NumYears As Integer
        Get
            Return Me.m_nYears
        End Get
    End Property

    Public Property GroupDemandAtT As Single(,)
    Public Property GroupDemandAtY As Single(,)
    Public Property GroupSupplyAtT As Single(,)
    Public Property GroupSupplyAtY As Single(,)
    Public Property SlopeAtT As Single()
    Public Property SlopeAtY As Single()
    Public Property InterceptAtT As Single()
    Public Property InterceptAtY As Single()
    Public Property IsConsumer As Boolean()

    Public ReadOnly Property ResilienceAtT(iTime As Integer) As Single
        Get
            Return -Me.SlopeAtT(iTime)
        End Get
    End Property

    Public ReadOnly Property ResilienceAtY(iYear As Integer) As Single
        Get
            Return -Me.SlopeAtY(iYear)
        End Get
    End Property

    Public Function DataboundsT() As sBounds
        Return Me.m_boundsT
    End Function

    Public Function DataboundsY() As sBounds
        Return Me.m_boundsT
    End Function

    Public ReadOnly Property Calculated As Boolean
        Get
            Return Me.m_bCalculated
        End Get
    End Property

    Public Sub CalculateBounds()

        Me.m_boundsT.Init()
        Me.m_boundsY.Init()
        For i As Integer = 1 To Me.m_nGroups
            For t As Integer = 0 To Me.m_nTimes - 1
                Me.m_boundsT.dmin = Math.Min(Me.m_boundsT.dmin, Me.GroupSupplyAtT(i, t))
                Me.m_boundsT.dmax = Math.Max(Me.m_boundsT.dmax, Me.GroupSupplyAtT(i, t))
                Me.m_boundsT.smin = Math.Min(Me.m_boundsT.smin, Me.GroupDemandAtT(i, t))
                Me.m_boundsT.smax = Math.Max(Me.m_boundsT.smax, Me.GroupDemandAtT(i, t))
            Next
            For t As Integer = 0 To Me.m_nYears - 1
                Me.m_boundsY.dmin = Math.Min(Me.m_boundsY.dmin, Me.GroupSupplyAtY(i, t))
                Me.m_boundsY.dmax = Math.Max(Me.m_boundsY.dmax, Me.GroupSupplyAtY(i, t))
                Me.m_boundsY.smin = Math.Min(Me.m_boundsY.smin, Me.GroupDemandAtY(i, t))
                Me.m_boundsY.smax = Math.Max(Me.m_boundsY.smax, Me.GroupDemandAtY(i, t))
            Next t
        Next i

        Me.m_bCalculated = True

    End Sub

End Class
