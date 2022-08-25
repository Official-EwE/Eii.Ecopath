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
Public Class cGroupTimeSeries
    Inherits cTimeSeries

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Sub New(core As cCore, iDBID As Integer)
        MyBase.New(core, iDBID)
        Me.m_datatype = eDataTypes.GroupTimeSeries
    End Sub

    Public Overrides Function IsValid() As Boolean
        Return (Me.GroupIndexStatus And eStatusFlags.ErrorEncountered) = 0
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the index of the Group this time series applies to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property GroupIndex() As Integer
        Get
            Return Me.DatPool
        End Get

        Set(iGroup As Integer)
            Me.DatPool = iGroup
        End Set
    End Property

    Public ReadOnly Property GroupIndexStatus() As eStatusFlags
        Get
            If (Me.DatPool < 1 Or Me.DatPool > Me.m_core.nGroups) Then
                Return eStatusFlags.ErrorEncountered
            End If
            Return eStatusFlags.OK
        End Get
    End Property

End Class