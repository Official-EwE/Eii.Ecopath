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
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwERowHeaderCell implements a EwERowHeaderCell to implement row headers. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwERowHeaderCell
        : Inherits EwEHeaderCell

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell with an optional static value.
        ''' </summary>
        ''' <param name="strValue">The value to set.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(Optional ByVal strValue As String = "")
            MyBase.New(strValue)
            ' Set visualizer
            Me.VisualModel = New cEwEGridRowHeaderVisualizer()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell displaying a single unit.
        ''' </summary>
        ''' <param name="strUnitMask">The mask should contain ONE {0} placeholder where
        ''' the <paramref name="unitType">unit</paramref> will be displayed.</param>
        ''' <param name="unitType">The <see cref="eUnitType">unit</see>
        ''' to dynamically substitute in the cell display text.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal strUnitMask As String, ByVal unitType As eUnitType)
            Me.New(strUnitMask, New eUnitType() {unitType})
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Construct a header cell displaying a series of units.
        ''' </summary>
        ''' <param name="strUnitMask">The mask should contain a string format 
        ''' placeholder for each <paramref name="aunitTypes">unit</paramref>.</param>
        ''' <param name="aUnitTypes">The <see cref="eUnitType">units</see>
        ''' to dynamically substitute in the cell display text.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As eUnitType)
            Me.New()
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags)
            Me.New(New cVarnameTypeFormatter().GetDescriptor(varname, eDescriptorTypes.Name))
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, ByVal strUnitMask As String, ByVal unitType As eUnitType)
            Me.New(String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                 New cVarnameTypeFormatter().GetDescriptor(varname, eDescriptorTypes.Name), _
                                 strUnitMask), _
                   New eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, ByVal strUnitMask As String, ByVal aUnitTypes() As eUnitType)
            Me.New(String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                 New cVarnameTypeFormatter().GetDescriptor(varname, eDescriptorTypes.Name), _
                                 strUnitMask), _
                   aUnitTypes)
        End Sub

#End Region ' Construction 

    End Class

End Namespace
