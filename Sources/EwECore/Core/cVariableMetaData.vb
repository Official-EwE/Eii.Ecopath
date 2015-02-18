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

Option Strict On
Imports EwECore.ValueWrapper

''' ---------------------------------------------------------------------------
''' <summary>
''' Meta data for a variable, describing its value range and default value.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cVariableMetaData

    ' -- Variables for numeric values --

    ''' <summary>Minimum value for a variable.</summary>
    Private m_min As Single = 0
    ''' <summary>Minimum value operator.</summary>
    Private m_operatorMin As cOperatorBase = Nothing
    ''' <summary>Maximum value for a variable.</summary>
    Private m_max As Single = 0
    ''' <summary>Maximum value operator.</summary>
    Private m_operatorMax As cOperatorBase = Nothing
    ''' <summary>Default value for variable when a value is missing or in error.</summary>
    Private m_nullvalue As Object = Nothing
    ''' <summary>Length of array items</summary>
    Private m_iArrayLength As Integer = cCore.NULL_VALUE

    ' -- Variables for string values --
    ''' <summary>Allowed length of string values.</summary>
    Private m_iStringLength As Integer = 0

    Private m_vartype As eValueTypes

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor use boolean values.
    ''' </summary>
    ''' <param name="bValueDefault">Default value to assign to variable when in error.</param>
    ''' <remarks>Booleans do not have min or max values.</remarks>
    ''' -----------------------------------------------------------------------
    Sub New(Optional ByVal bValueDefault As Boolean = False)
        Me.m_nullvalue = bValueDefault
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constuctor for string values.
    ''' </summary>
    ''' <param name="iLength">The max allowed string length.</param>
    ''' <param name="strValueDefault">
    ''' Default value to assign to variable when in error.</param>
    ''' <remarks>Strings do not have min or max values.</remarks>
    ''' -----------------------------------------------------------------------
    Sub New(ByVal iLength As Integer, Optional ByVal strValueDefault As String = "")
        Me.m_iStringLength = iLength
        Me.m_nullvalue = strValueDefault
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for numeric values.
    ''' </summary>
    ''' <param name="sMin">Lowest value a variable can contain.</param>
    ''' <param name="sMax">Highest value a variable can contain.</param>
    ''' <param name="operatorMin"><see cref="cOperatorBase">Operator</see>
    ''' stating how the <paramref name="sMin">minimum value</paramref> is included
    ''' in the variable value range.</param>
    ''' <param name="operatorMax"><see cref="cOperatorBase">Operator</see>
    ''' stating how the <paramref name="sMax">maximum value</paramref> is included
    ''' in the variable value range.</param>
    ''' <param name="sValueDefault">Default value to assign to variable when in error.</param>
    ''' -----------------------------------------------------------------------
    Sub New(ByVal sMin As Single, ByVal sMax As Single, _
            ByVal operatorMin As cOperatorBase, ByVal operatorMax As cOperatorBase, _
            Optional ByVal sValueDefault As Single = 0.0!)
        Me.m_min = sMin
        Me.m_max = sMax
        Me.m_operatorMin = operatorMin
        Me.m_operatorMax = operatorMax
        Me.m_nullvalue = sValueDefault
    End Sub

#End Region ' Constructors

#Region " Operators "

    Friend Sub Attach(ByVal value As cValue)

        Me.m_vartype = value.varType

        Select Case Me.m_vartype
            Case eValueTypes.Bool, eValueTypes.Int, eValueTypes.Sng
                Debug.Assert(value.Length = 0, "Variable malformed")
            Case eValueTypes.BoolArray, eValueTypes.IntArray, eValueTypes.SingleArray
                Me.m_iArrayLength = value.Length
            Case eValueTypes.Str
                ' NOP
            Case Else
                Throw New NotImplementedException()
        End Select

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the minimum value operator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MinOperator() As cOperatorBase
        Get
            Return Me.m_operatorMin
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the maximum value operator.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MaxOperator() As cOperatorBase
        Get
            Return Me.m_operatorMax
        End Get
    End Property

#End Region ' Operators

#Region " Properties "

    ' Properties are Public read and Friend write at this time this is by design.
    ' If the are exposed by the core they should not be editable.

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the minimum value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Min() As Single
        Get
            Return Me.m_min
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the maximum value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Max() As Single
        Get
            Return Me.m_max
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the default value for a variable.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NullValue() As Object
        Get
            Return Me.m_nullvalue
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the maximum allowed string length for variables.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Length() As Integer
        Get
            Return Me.m_iStringLength
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eValueTypes">value type</see> of the variable 
    ''' that this metadata represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property VarType As eValueTypes
        Get
            Return Me.m_vartype
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the maximum allowed index for the variable, or 0 if the variable
    ''' is not an array.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ArrayLength As Integer
        Get
            Return Me.m_iArrayLength
        End Get
    End Property

#End Region ' Properties

End Class

