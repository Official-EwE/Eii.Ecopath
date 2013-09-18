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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwECore
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Central indicator definitions.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cIndicatorSettings

#Region " Helper class cIndicatorInfo "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class providing name, description and access to computed values for a single indicator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cIndicatorInfo

#Region " Private fields "

        ''' <summary>The name of the indicator</summary>
        Private m_strName As String = ""
        ''' <summary>The description of the indicator</summary>
        Private m_strDescription As String = ""
        ''' <summary>The units of the indicator</summary>
        Private m_aunits() As cStyleGuide.eUnitType = Nothing
        ''' <summary>Mask to use for formatting units.</summary>
        Private m_strUnitMask As String = ""
        ''' <summary>The function name of the indicator in the <see cref="cIndicators">indicator</see></summary>
        Private m_strFunctionName As String = ""

#End Region ' Private fields

#Region " Construction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance.
        ''' </summary>
        ''' <param name="strName">Name to assign to the indicator.</param>
        ''' <param name="strFunctionName">The name of function for the indicator as exposed by the computed <see cref="cIndicators">indicator</see>.</param>
        ''' <param name="strDescription">Description to assign to the indicator.</param>
        ''' <param name="aunits">EwE <see cref="cStyleGuide.eUnitType">units</see> to show for the indicator.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strFunctionName As String, _
                       ByVal strName As String, _
                       ByVal strDescription As String, _
                       ByVal aunits() As cStyleGuide.eUnitType, _
                       ByVal strUnitMask As String)

            Me.m_strName = strName
            Me.m_strFunctionName = strFunctionName
            Me.m_aunits = aunits
            Me.m_strUnitMask = strUnitMask
            Me.m_strDescription = strDescription

        End Sub

#End Region ' Construction

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of the indicator.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Name As String
            Get
                Return Me.m_strName
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the description of the indicator.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Description As String
            Get
                Return Me.m_strDescription
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the units of the indicator.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Units As cStyleGuide.eUnitType()
            Get
                Return Me.m_aunits
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the mask to use for formatting the <see cref="Units"/>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property UnitMask As String
            Get
                Return Me.m_strUnitMask
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the value for the indicator from a computed <see cref="cIndicators">indicator</see>.
        ''' </summary>
        ''' <param name="indicators">The computed <see cref="cIndicators">indicator</see> to extract information from.</param>
        ''' <returns>A value, or <see cref="cCore.NULL_VALUE"/> if the property was not found.</returns>
        ''' -------------------------------------------------------------------
        Public Function GetValue(ByVal indicators As cIndicators) As Single

            If (indicators Is Nothing) Then Return 0

            ' Try to get property info from the indicator
            Dim mi As MethodInfo = GetType(cIndicators).GetMethod(Me.m_strFunctionName)
            ' Prepare default value
            Dim sValue As Single = cCore.NULL_VALUE
            ' Was property found?
            If (mi IsNot Nothing) Then
                ' #Yes: try to extract the value as a SINGLE precision number
                Try
                    sValue = CSng(mi.Invoke(indicators, New Object() {}))
                Catch ex As Exception
                    ' A failure is due to a programming error
                    Debug.Assert(False, "Property " & Me.m_strFunctionName & " cannot be converted to Single")
                End Try
            End If
            ' Return value
            Return sValue

        End Function

#End Region ' Public access

    End Class

#End Region ' Helper class cIndicatorInfo

#Region " Helper class cIndicatorInfoGroup "

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
                            Optional strFixedUnit As String = "") As cIndicatorInfo
            Return Me.Add(strPropertyName, strName, strDescription, New cStyleGuide.eUnitType() {cStyleGuide.eUnitType.None}, strFixedUnit)
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
                            ByVal unit As cStyleGuide.eUnitType, _
                            Optional ByVal strUnitMask As String = "{0}") As cIndicatorInfo
            Return Me.Add(strPropertyName, strName, strDescription, New cStyleGuide.eUnitType() {unit}, strUnitMask)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add an indicator that has multiple dimensioned, dynamic units.
        ''' </summary>
        ''' <param name="strName">The name to assign to the indicator.</param>
        ''' <param name="strPropertyName">The property name of the indicator as exposed by the computed <see cref="cIndicators">indicator</see>.</param>
        ''' <param name="strDescription">Description to assign to the indicator.</param>
        ''' <param name="aunits">Units to display.</param>
        ''' <param name="strUnitMask">Mask to use for formatting the <paramref name="aunits">units</paramref>.</param>
        ''' <returns>The new indicator info object.</returns>
        ''' -------------------------------------------------------------------
        Public Function Add(ByVal strPropertyName As String, _
                            ByVal strName As String, _
                            ByVal strDescription As String, _
                            ByVal aunits() As cStyleGuide.eUnitType, _
                            Optional ByVal strUnitMask As String = "{0}/{1}") As cIndicatorInfo
            Dim ind As New cIndicatorInfo(strPropertyName, strName, strDescription, aunits, strUnitMask)
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

#End Region ' Helper class cIndicatorInfoGroup

#Region " Private variables "

    Private m_lIndicatorGroups As New List(Of cIndicatorInfoGroup)

#End Region ' Private variables

#Region " Constructor "

    Public Sub New()
        Me.Populate()
    End Sub

#End Region ' Constructor

#Region " Public fields "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a new group to the settings.
    ''' </summary>
    ''' <param name="strName">Name to assign to the group.</param>
    ''' <param name="strDescription">Optional description to assign to the group.</param>
    ''' <returns>The new group.</returns>
    ''' -----------------------------------------------------------------------
    Public Function AddGroup(ByVal strName As String, _
                             Optional ByVal strDescription As String = "") As cIndicatorInfoGroup
        Dim grp As New cIndicatorInfoGroup(strName, strDescription)
        Me.m_lIndicatorGroups.Add(grp)
        Return grp
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of <see cref="cIndicatorInfoGroup"/>s in the settings.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumIndicatorGroups As Integer
        Get
            Return Me.m_lIndicatorGroups.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cIndicatorInfoGroup"/>s at a given index in the settings.
    ''' </summary>
    ''' <param name="index">The index to obtain the <see cref="cIndicatorInfoGroup"/> for.</param>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IndicatorGroup(ByVal index As Integer) As cIndicatorInfoGroup
        Get
            Return Me.m_lIndicatorGroups(index)
        End Get
    End Property

#End Region ' Public fields

#Region " Internals "

    Private Sub Populate()

        Dim grp As cIndicatorInfoGroup = Nothing
        Dim ind As cIndicatorInfo = Nothing

        Dim aunitCatch() As cStyleGuide.eUnitType = New cStyleGuide.eUnitType() {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time}

        ' 6 trophic-based indicators
        grp = Me.AddGroup(My.Resources.GROUP_TROPHIC, My.Resources.GROUP_TROPHIC_DESC)
        grp.Add("TLC", My.Resources.IND_TLC, My.Resources.IND_TLC_DESC)
        grp.Add("MTI", My.Resources.IND_MTI, String.Format(My.Resources.IND_MTIX_DESC, 3.25))
        grp.Add("TLco", My.Resources.IND_TLCo, My.Resources.IND_TLCo_DESC)
        grp.Add("TLco2", String.Format(My.Resources.IND_TLCoX, 2), String.Format(My.Resources.IND_TLCoX_DESC, 2))
        grp.Add("TLco325", String.Format(My.Resources.IND_TLCoX, 3.25), String.Format(My.Resources.IND_TLCoX_DESC, 3.25))
        grp.Add("TLco4", String.Format(My.Resources.IND_TLCoX, 4), String.Format(My.Resources.IND_TLCoX_DESC, 4))

        '10 biomass-based indicators
        grp = Me.AddGroup(My.Resources.GROUP_BIOMASS, My.Resources.GROUP_BIOMASS_DESC)
        grp.Add("TotalB", My.Resources.IND_TOTALB, My.Resources.IND_TOTALB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("CommercialB", My.Resources.IND_COMMB, My.Resources.IND_COMMB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("FishB", My.Resources.IND_FISHB, My.Resources.IND_FISHB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("InveB", My.Resources.IND_INVEB, My.Resources.IND_INVEB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("InveFishB", My.Resources.IND_INVFISHB, My.Resources.IND_INVFISHB_DESC)
        grp.Add("DemB", My.Resources.IND_DEMB, My.Resources.IND_DEMB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("PelB", My.Resources.IND_PELB, My.Resources.IND_PELB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("DemPelB", My.Resources.IND_DEMPELB, My.Resources.IND_DEMPELB_DESC)
        grp.Add("PredB", My.Resources.IND_PREDB, My.Resources.IND_PREDB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("KemptonsQ", My.Resources.IND_KEMPTONSQ, My.Resources.IND_KEMPTONQ_DESC)

        '9 catch-based indicators
        grp = Me.AddGroup(My.Resources.GROUP_CATCH, My.Resources.GROUP_CATCH_DESC)
        grp.Add("Ctotal", My.Resources.IND_TOTALC, My.Resources.IND_TOTALC_DESC, aunitCatch)
        grp.Add("FishC", My.Resources.IND_FISHC, My.Resources.IND_FISHC_DESC, aunitCatch)
        grp.Add("InveC", My.Resources.IND_INVC, My.Resources.IND_INVC_DESC, aunitCatch)
        grp.Add("InveFishC", My.Resources.IND_INVFISHC, My.Resources.IND_INVFISHC_DESC)
        grp.Add("DemC", My.Resources.IND_DEMC, My.Resources.IND_DEMC_DESC, aunitCatch)
        grp.Add("PelC", My.Resources.IND_PELC, My.Resources.IND_PELC_DESC, aunitCatch)
        grp.Add("DemPelC", My.Resources.IND_DEMPELC, My.Resources.IND_DEMPELC_DESC)
        grp.Add("sC4", My.Resources.IND_PREDC, My.Resources.IND_PREDC_DESC, aunitCatch)
        grp.Add("DT", My.Resources.IND_DIS, My.Resources.IND_DIS_DESC, aunitCatch)

        '7 species-based indicators
        grp = Me.AddGroup(My.Resources.GROUP_SPECIES, My.Resources.GROUP_SPECIES_DESC)
        grp.Add("IVIC", My.Resources.IND_IVIC, My.Resources.IND_IVIC_DESC)
        grp.Add("EndemicB", My.Resources.IND_ENDB, My.Resources.IND_ENDB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("EndemicC", My.Resources.IND_ENDC, My.Resources.IND_ENDC_DESC, aunitCatch)
        grp.Add("IUCNB", My.Resources.IND_IUCNB, My.Resources.IND_IUCNB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("IUCNC", My.Resources.IND_IUCNC, My.Resources.IND_IUCNC_DESC, aunitCatch)
        grp.Add("MSRB", My.Resources.IND_MSRB, My.Resources.IND_MSRB_DESC, cStyleGuide.eUnitType.Currency)
        grp.Add("MSRC", My.Resources.IND_MSRC, My.Resources.IND_MSRC_DESC, aunitCatch)

        ' 6 size-based indicators
        grp = Me.AddGroup(My.Resources.GROUP_SIZE, My.Resources.GROUP_SIZE_DESC)
        grp.Add("MLengthB", My.Resources.IND_MLB, My.Resources.IND_MLB_DESC, My.Resources.UNIT_LENGTH_CM)
        grp.Add("MLengthC", My.Resources.IND_MLC, My.Resources.IND_MLC_DESC, My.Resources.UNIT_LENGTH_CM)
        grp.Add("MWeightB", My.Resources.IND_MWB, My.Resources.IND_MWB_DESC, My.Resources.UNIT_WEIGHT_KG)
        grp.Add("MWeightC", My.Resources.IND_MWC, My.Resources.IND_MWC_DESC, My.Resources.UNIT_WEIGHT_KG)
        grp.Add("MLifeSpanB", My.Resources.IND_MLSC, My.Resources.IND_MLSC_DESC, My.Resources.UNIT_TIME_YEAR)
        grp.Add("MLifeSpanC", My.Resources.IND_MLSB, My.Resources.IND_MLSB_DESC, My.Resources.UNIT_TIME_YEAR)

    End Sub

#End Region ' Internals

End Class
