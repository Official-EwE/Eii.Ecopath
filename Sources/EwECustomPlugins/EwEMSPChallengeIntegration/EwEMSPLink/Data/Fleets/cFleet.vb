Option Strict On
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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Imports EwECore

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Wrapper class for an EwE fleet to configure MSP Challenge-specific settings:
''' <list type="bullet">
''' <item>Nationality (for EwE > MSP)</item>
''' <item>Bycatch species (for EwE internal)</item>
''' </list>
''' </summary>
''' <remarks>Need to work out how to transfer "Ecological mode" state flag 
''' (per fleet? for all?) from MEL to EwE.</remarks>
''' ---------------------------------------------------------------------------
Public Class cFleet
    Implements IMELItem

    Private m_core As cCore = Nothing
    Private m_iDBID As Integer = Nothing
    Private m_bycatch As New List(Of Integer)

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create an instance of a pressure definition. Pressure definitions cannot 
    ''' accept actual pressure data; they just serve to define game dynamics.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <param name="iDBID">Ecopath fleet ID</param>
    ''' -----------------------------------------------------------------------
    Friend Sub New(core As cCore, iDBID As Integer)
        MyBase.New()
        Me.m_core = core
        Me.m_iDBID = iDBID
    End Sub

#End Region ' Constructors

#Region " Public bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the pressure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name
        Get
            Dim i As Integer = Me.iFleet
            If (i < 1) Then Return "(Undefined fleet)"
            Return Me.m_core.EcopathFleetInputs(i).Name
        End Get
        Private Set(value As String)
            ' NOP
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get / set the fleet nationality. ToDo: decide with HW how to define this
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Nationality As String = ""

    ''' <summary>
    ''' Get/set whether a given group is considered bycatch for this fleet
    ''' </summary>
    ''' <param name="iDBID"></param>
    ''' <returns></returns>
    Public Property Bycatch(iDBID As Integer) As Boolean
        Get
            Return Me.m_bycatch.Contains(iDBID)
        End Get
        Set(value As Boolean)
            If value Then
                If Not Me.Bycatch(iDBID) Then Me.m_bycatch.Add(iDBID)
            Else
                If Me.Bycatch(iDBID) Then Me.m_bycatch.Remove(iDBID)
            End If
        End Set
    End Property

    Public ReadOnly Property NoBycatch As Integer
        Get
            Return Me.m_bycatch.Count
        End Get
    End Property

    Public ReadOnly Property NoDiscards As Integer
        Get
            Dim i As Integer = Me.iFleet
            If (i < 1) Then Return cCore.NULL_VALUE
            Dim fleet As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(i)
            Dim n As Integer = 0

            For igrp As Integer = 0 To Me.m_core.nGroups
                If fleet.Discards(igrp) > 0 Then n += 1
            Next
            Return n
        End Get
    End Property

#End Region ' Public bits

    Private Function iFleet() As Integer
        Return Array.IndexOf(m_core.EcopathDataStructures.FleetDBID, Me.m_iDBID)
    End Function

    Private Function iGroup(iDBID As Integer) As Integer
        Return Array.IndexOf(m_core.EcopathDataStructures.GroupDBID, iDBID)
    End Function

End Class
