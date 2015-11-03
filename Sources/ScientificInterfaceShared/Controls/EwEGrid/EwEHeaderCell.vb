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
Imports EwECore
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwERowHeaderCell implements a Common cell rendered as an EwE name field.
    ''' EwERowHeaderCells are used in EwE to replace Row headers which values are statically
    ''' set.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public MustInherit Class EwEHeaderCell
        : Inherits EwECell

#Region " Construction "

        Public Sub New(Optional ByVal objValue As Object = Nothing)
            MyBase.New(objValue, GetType(String))
            ' Disable edit
            Me.DataModel.EnableEdit = False
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As cStyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, New cStyleGuide.eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As cStyleGuide.eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

#End Region ' Construction 

#Region " Data (style) "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to enusre that header cells use names colour feedback
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overrides Property Style() As cStyleGuide.eStyleFlags
            Get
                Return (cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable Or MyBase.Style)
            End Get
            Set(ByVal styleNew As cStyleGuide.eStyleFlags)
                MyBase.Style = (styleNew Or cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable)
            End Set
        End Property

#End Region ' Data (style) 

#Region " Unit header text "

        Protected m_aUnitTypes() As cStyleGuide.eUnitType
        Protected m_strUnitMask As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure the cell to automatically incorporate unit strings into
        ''' its content. These unit strings will be synchronized with 
        ''' <see cref="cStyleGuide.UnitsChanged">cStyleGuide unit changes</see>.
        ''' </summary>
        ''' <param name="strUnitMask">Mask to format units with. This mask must
        ''' contain a {#} placeholder for every dynamic unit; {0} for the first
        ''' unit, {1} for the second unit, etc. Only two units are currently 
        ''' supported.</param>
        ''' <param name="aUnitTypes">An array of unit types to format into the
        ''' header cell.</param>
        ''' -------------------------------------------------------------------
        Public Sub SetUnitHeader(ByVal strUnitMask As String, _
                                 ByVal aUnitTypes() As cStyleGuide.eUnitType)
            ' Sanity checks
            Debug.Assert(aUnitTypes.Length = 1 Or aUnitTypes.Length = 2)
            ' Store
            Me.m_strUnitMask = strUnitMask
            Me.m_aUnitTypes = aUnitTypes
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure the cell to no longer incorporate unit strings into its 
        ''' text.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub ClearUnitHeader()
            Me.m_strUnitMask = ""
            Me.m_aUnitTypes = Nothing
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value for a header cell.
        ''' </summary>
        ''' <bugfix number="892">
        ''' Moved this functionality from DisplayText to make sure header values
        ''' are correctly picked up by Copy and Cut operations.
        ''' </bugfix>
        ''' <remarks>If a header cell value contains a '|' character, the value 
        ''' is split by this character. The first part (left side of '|') is used
        ''' as value part, and the last part (right side of '|') is used as tooltip
        ''' text.</remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Property Value() As Object
            Get
                If (Me.m_aUnitTypes Is Nothing) Then
                    Return MyBase.Value
                End If

                If (Me.StyleGuide Is Nothing) Then Return Me.m_strUnitMask

                Return Me.StyleGuide.FormatUnitString(Me.m_strUnitMask, Me.m_aUnitTypes)
            End Get
            Set(ByVal value As Object)
                If TypeOf value Is String Then
                    Dim strValue As String = CStr(value)
                    If strValue.IndexOf("|"c) > -1 Then
                        Dim astrBits As String() = strValue.Split("|"c)
                        Me.ToolTipText = astrBits(1)
                        value = astrBits(0)
                    End If
                End If
                MyBase.Value = value
            End Set
        End Property

#End Region ' Unit header text

    End Class

End Namespace
