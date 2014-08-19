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

#End Region ' Imports

Namespace Core

    Public Interface IShapeFunction

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize to a given shape.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' -----------------------------------------------------------------------
        Sub Init(shape As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Set a shape function parameters to their default values.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub Defaults()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of parameters needed to configure a shape function.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property nParameters() As Integer

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the human legible name of a parameter of a shape function.
        ''' </summary>
        ''' <param name="iParam">The index of the parameters [1,<see cref="nParameters"/>]
        ''' to obtain the human legible name for.</param>
        ''' -----------------------------------------------------------------------
        ReadOnly Property ParamName(ByVal iParam As Integer) As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value of a parameter of the shape function.
        ''' </summary>
        ''' <param name="iParam">The index of the parameters [1,<see cref="nParameters"/>]
        ''' to access the value for.</param>
        ''' -----------------------------------------------------------------------
        Property ParamValue(ByVal iParam As Integer) As Single

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the actual function data points, as computed from the <see cref="ParamValue">parameters values</see>.
        ''' </summary>
        ''' <param name="nPoints">The length of the points array to return.</param>
        ''' <returns>An array of points.</returns>
        ''' -----------------------------------------------------------------------
        Function Shape(Optional ByVal nPoints As Integer = 1200) As Single()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a shape function is relevant for a given <see cref="EwEUtils.Core.eDataTypes">data type</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function IsRelevantDataType(ByVal DataType As EwEUtils.Core.eDataTypes) As Boolean

    End Interface

End Namespace

