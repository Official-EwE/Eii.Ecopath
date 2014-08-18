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
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Base class for implementing EwE core shape functions.
''' </summary>
Public MustInherit Class cShapeFunction
    Implements EwEUtils.Core.IShapeFunction

#Region " Private vars "

    ''' <summary>This original value is extracted from EwE5.</summary>
    Protected Const xBase As Single = 0.3

    Protected m_sValues As Single() = Nothing
    Protected m_sPoints As Single() = Nothing

#End Region ' Private vars

    Public Sub New()

        ' Not ready to be used yet
        Throw New NotImplementedException()

        ReDim Me.m_sValues(Me.nParameters)
        ReDim Me.m_sPoints(1200)
        Me.Defaults()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get one of the pre-defined shape function types for this shape.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' This will have to change once shape functions are delivered by 
    ''' plug-ins. Then, a class name will have to be used instead of an
    ''' enum to locate the function that was used.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ShapeFunctionType As EwECore.eShapeFunctionType

    Public MustOverride Sub Defaults() _
        Implements EwEUtils.Core.IShapeFunction.Defaults

    Public MustOverride Function IsRelevantDataType(datatype As EwEUtils.Core.eDataTypes) As Boolean _
        Implements EwEUtils.Core.IShapeFunction.IsRelevantDataType

    Public MustOverride ReadOnly Property nParameters As Integer _
        Implements EwEUtils.Core.IShapeFunction.nParameters

    Public Overridable ReadOnly Property ParamName(iParam As Integer) As String _
        Implements EwEUtils.Core.IShapeFunction.ParamName
        Get
            Debug.Assert((iParam > 1) And (iParam <= Me.nParameters))
            Select Case iParam
                Case 1 : Return My.Resources.CoreDefaults.PARAM_YZERO
                Case 2 : Return My.Resources.CoreDefaults.PARAM_YEND
                Case 3 : Return My.Resources.CoreDefaults.PARAM_YBASE
                Case 4 : Return My.Resources.CoreDefaults.PARAM_STEEPNESS
            End Select
            Return "?"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Flag, indicating that parameter values have changed and that the shape 
    ''' should be <see cref="CalculateShape">recalculated</see>.
    ''' </summary>
    ''' <returns>True if parameter values have recently changed.</returns>
    ''' -----------------------------------------------------------------------
    Protected Property ParamsChanged As Boolean = True

    Public Overridable Property ParamValue(iParam As Integer) As Single _
        Implements EwEUtils.Core.IShapeFunction.ParamValue
        Get
            Debug.Assert((iParam > 1) And (iParam <= Me.nParameters))
            Return Me.m_sValues(iParam)
        End Get
        Set(value As Single)
            Debug.Assert((iParam > 1) And (iParam <= Me.nParameters))
            If (Me.m_sValues(iParam) <> value) Then
                Me.m_sValues(iParam) = value
                Me.ParamsChanged = True
            End If
        End Set
    End Property

    Public MustOverride Function CalculateShape(Optional ByVal nPoints As Integer = 1200) As Single() _
        Implements EwEUtils.Core.IShapeFunction.CalculateShape

End Class
