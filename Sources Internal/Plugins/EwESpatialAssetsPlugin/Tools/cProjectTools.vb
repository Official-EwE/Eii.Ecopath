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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Drawing
Imports DotSpatial.Projections

#End Region ' Imports

Public Class cProjectTools

    Private m_piSource As ProjectionInfo = Nothing
    Private m_piDest As ProjectionInfo = Nothing

    Public Sub New(strWktSource As String, strWktDest As String)

        Me.m_piSource = ProjectionInfo.FromEsriString(strWktSource)
        Me.m_piDest = ProjectionInfo.FromEsriString(strWktDest)

    End Sub

    Public Function Transform(ptf As PointF) As PointF

        Dim pts() As Double = New Double() {ptf.X, ptf.Y}
        Dim z() As Double = New Double() {1.0!}

        Reproject.ReprojectPoints(pts, z, Me.m_piSource, Me.m_piDest, 0, 1)

        Return New PointF(CSng(pts(0)), CSng(pts(1)))

    End Function

End Class
