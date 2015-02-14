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
Imports System.IO
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region

Public Class cCompareManager

#Region "Private variables"
    Private m_core As cCore
    Private m_EcopathData As cEcopathDataStructures
    Private m_EcosimData As cEcosimDatastructures
    Private m_EcospaceData As cEcospaceDataStructures

#End Region

#Region "Public Properties"

    Public ReadOnly Property Core As cCore
        Get
            Return Me.m_core
        End Get
    End Property

    Public ReadOnly Property EcopathData As cEcopathDataStructures
        Get
            Return Me.m_EcopathData
        End Get
    End Property

    Public ReadOnly Property EcosimData As cEcosimDatastructures
        Get
            Return Me.m_EcosimData
        End Get
    End Property


    Public ReadOnly Property EcoSpaceData As cEcospaceDataStructures
        Get
            Return Me.m_EcospaceData
        End Get
    End Property

#End Region

#Region "Construction Initialization"

    Public Sub New(ByVal Core As cCore, PathData As cEcopathDataStructures, _
                   SimData As cEcosimDatastructures, SpaceData As cEcospaceDataStructures)
        Me.m_core = Core
        Me.m_EcopathData = PathData
        Me.m_EcosimData = SimData
        Me.m_EcospaceData = SpaceData
    End Sub

#End Region

End Class
