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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwECore
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Core

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Class providing name, description, and indicator info for a group of indicators.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cIndicatorInfoGroup

#Region " Private fields "

    ''' <summary>The name of the indicator group</summary>
    Private m_strName As String = ""
    ''' <summary>The description of the indicator group</summary>
    Private m_strDescription As String = ""
    ''' <summary>List of indicator info objects that belong to this group</summary>
    Private m_lIndicators As New List(Of cIndicatorInfo)

#End Region ' Private fields

#Region " Construction "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create a new instance.
    ''' </summary>
    ''' <param name="strName">Name to assign to the indicator group.</param>
    ''' <param name="strDescription">Description to assign to the indicator group.</param>
    ''' -------------------------------------------------------------------
    Public Sub New(ByVal strName As String, ByVal strDescription As String)

        Me.m_strName = strName
        Me.m_strDescription = strDescription

    End Sub

#End Region ' Construction

#Region " Public access "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the indicator group.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Name As String
        Get
            Return Me.m_strName
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the description of the indicator group.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Description As String
        Get
            Return Me.m_strDescription
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Add an indicator that is unitless or has a fixed unit.
    ''' </summary>
    ''' <param name="strName">The name to assign to the indicator.</param>
    ''' <param name="strPropertyName">The property name of the indicator as exposed by the computed <see cref="cIndicators">indicator</see>.</param>
    ''' <param name="strDescription">Description to assign to the indicator.</param>
    ''' <returns>The new indicator info object.</returns>
    ''' -------------------------------------------------------------------
    Public Function Add(ByVal strPropertyName As String, _
                        ByVal strName As String, _
                        ByVal strDescription As String, _
                        ByVal strValueDescription As String, _
                        Optional strFixedUnit As String = "") As cIndicatorInfo
        Return Me.Add(strPropertyName, strName, strDescription, strValueDescription, New eUnitType() {eUnitType.None}, strFixedUnit)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Add an indicator that has a sinlge-dimensioned, dynamic unit.
    ''' </summary>
    ''' <param name="strName">The name to assign to the indicator.</param>
    ''' <param name="strPropertyName">The property name of the indicator as exposed by the computed <see cref="cIndicators">indicator</see>.</param>
    ''' <param name="strDescription">Description to assign to the indicator.</param>
    ''' <returns>The new indicator info object.</returns>
    ''' -------------------------------------------------------------------
    Public Function Add(ByVal strPropertyName As String, _
                        ByVal strName As String, _
                        ByVal strDescription As String, _
                        ByVal strValueDescription As String, _
                        ByVal unit As eUnitType, _
                        Optional ByVal strUnitMask As String = "{0}") As cIndicatorInfo
        Return Me.Add(strPropertyName, strName, strDescription, strValueDescription, New eUnitType() {unit}, strUnitMask)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Add an indicator that has multiple dimensioned, dynamic units.
    ''' </summary>
    ''' <param name="strName">The name to assign to the indicator.</param>
    ''' <param name="strPropertyName">The property name of the indicator as exposed by the computed <see cref="cIndicators">indicator</see>.</param>
    ''' <param name="strDescription">Description to assign to the indicator.</param>
    ''' <param name="strValueDescription"> Description of the value of the indicator.</param>
    ''' <param name="aunits">Units to display.</param>
    ''' <param name="strUnitMask">Mask to use for formatting the <paramref name="aunits">units</paramref>.</param>
    ''' <returns>The new indicator info object.</returns>
    ''' -------------------------------------------------------------------
    Public Function Add(ByVal strPropertyName As String, _
                        ByVal strName As String, _
                        ByVal strDescription As String, _
                        ByVal strValueDescription As String, _
                        ByVal aunits() As eUnitType, _
                        Optional ByVal strUnitMask As String = "{0}/{1}") As cIndicatorInfo
        Dim ind As New cIndicatorInfo(strPropertyName, strName, strDescription, strValueDescription, aunits, strUnitMask)
        Me.m_lIndicators.Add(ind)
        Return ind
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of <see cref="cIndicatorInfo">indicators</see> in the group.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property NumIndicators As Integer
        Get
            Return Me.m_lIndicators.Count
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get <see cref="cIndicatorInfo">indicator info</see> for a given indicator.
    ''' </summary>
    ''' <param name="index">The index of the indicator [0, <see cref="NumIndicators"/>-1].</param>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Indicator(ByVal index As Integer) As cIndicatorInfo
        Get
            Return Me.m_lIndicators.Item(index)
        End Get
    End Property

#End Region ' Public access

End Class