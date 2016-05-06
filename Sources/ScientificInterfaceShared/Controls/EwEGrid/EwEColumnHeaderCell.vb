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
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style
Imports SourceGrid2.VisualModels

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' EwEColumnHeader implements a column header with EwE style
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class EwEColumnHeaderCell
        : Inherits EwEHeaderCell

        Private m_vizDefault As IVisualModel = Nothing

#Region " Construction / destruction "

        Public Sub New(Optional ByVal strValue As String = "")
            MyBase.New(strValue)
            Me.m_vizDefault = Me.VisualModel
            Me.VisualModel = New cEwEGridColumnHeaderVisualizer()
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal unitType As eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, New eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal strUnitMask As String, ByVal aUnitTypes() As eUnitType)
            Me.New("")
            Me.SetUnitHeader(strUnitMask, aUnitTypes)
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags)
            Me.New(varname, eDescriptorTypes.Name)
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, detail As eDescriptorTypes)
            Me.New(New cVarnameTypeFormatter().GetDescriptor(varname, detail))
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, ByVal strUnitMask As String, ByVal unitType As eUnitType)
            Me.New(varname, eDescriptorTypes.Name, strUnitMask, unitType)
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, detail As eDescriptorTypes, ByVal strUnitMask As String, ByVal unitType As eUnitType)
            Me.New(String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                 New cVarnameTypeFormatter().GetDescriptor(varname, detail), _
                                 strUnitMask), _
                   New eUnitType() {unitType})
        End Sub

        Public Sub New(ByVal varname As eVarNameFlags, ByVal strUnitMask As String, ByVal aUnitTypes() As eUnitType)
            Me.New(String.Format(My.Resources.GENERIC_LABEL_DOUBLE, _
                                 New cVarnameTypeFormatter().GetDescriptor(varname, eDescriptorTypes.Name), _
                                 strUnitMask), _
                   aUnitTypes)
        End Sub

        Public Overrides Sub Dispose()
            Me.VisualModel = Me.m_vizDefault
            MyBase.Dispose()
        End Sub

#End Region ' Construction / destruction

    End Class

End Namespace
