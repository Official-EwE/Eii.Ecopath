' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Reflection
Imports EwECore
Imports EwECore.Common
Imports EwECore.Style



''' -----------------------------------------------------------------------
''' <summary>
''' Class providing name, description and access to computed values for a single indicator.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cIndicatorInfo

#Region " Internal vars "

    Private m_varname As String = ""

#End Region ' Internal vars

#Region " Construction "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create a new instance.
    ''' </summary>
    ''' <param name="strName">Name to assign to the indicator.</param>
    ''' <param name="strFunctionName">The name of function for the indicator as exposed by the computed <see cref="cIndicators">indicator</see>.</param>
    ''' <param name="strDescription">Description to assign to the indicator.</param>
    ''' <param name="strValueDescription">Description of the value of indicator (biomass, catch, etc).</param>
    ''' <param name="strUnits">EwE <see cref="cUnits">units</see> to show for the indicator.</param>
    ''' <param name="md">Optional <see cref="cVariableMetaData">metadata</see> for plotting etc.</param>
    ''' -------------------------------------------------------------------
    Public Sub New(strFunctionName As String,
                   strName As String,
                   strDescription As String,
                   strValueDescription As String,
                   strUnits As String,
                   Optional md As cVariableMetaData = Nothing)

        Me.Name = strName
        Me.Abbreviation = strFunctionName
        Me.ValueDescription = strValueDescription
        Me.Units = strUnits
        Me.Description = strDescription
        Me.Metadata = md
    End Sub

#End Region ' Construction

#Region " Public access "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the indicator should be computed.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Property Enabled As Boolean = True

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the indicator.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Name As String = ""

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the description of the indicator.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Description As String = ""

    ''' -------------------------------------------------------------------
    ''' <summary>The indicator abbreviated name. This name MUST coincode with 
    ''' the function name  used to compute the indicator in 
    ''' <see cref="cIndicators"/></summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Abbreviation As String = ""

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the Description of the value of indicator (biomass, catch, etc)
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property ValueDescription As String = ""

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the units of the indicator.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Units As String = ""

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get any <see cref="cVariableMetaData"/> associated the indicator.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Metadata As cVariableMetaData = Nothing

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name for this indicator to use when writing information 
    ''' to file. By default, this returns the indicator <see cref="Name"/>.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Property OutputName As String
        Get
            If String.IsNullOrWhiteSpace(Me.m_varname) Then Return Me.Name
            Return Me.m_varname
        End Get
        Set(value As String)
            Me.m_varname = value
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the value for the indicator from a computed <see cref="cIndicators">indicator</see>.
    ''' </summary>
    ''' <param name="indicators">The computed <see cref="cIndicators">indicator</see> to extract information from.</param>
    ''' <returns>A value, or <see cref="cCore.NULL_VALUE"/> if the property was not found.</returns>
    ''' -------------------------------------------------------------------
    Public Function GetValue(indicators As cIndicators) As Single

        If (indicators Is Nothing) Then Return 0

        ' Try to get property info from the indicator
        Dim mi As MethodInfo = GetType(cIndicators).GetMethod(Me.Abbreviation)
        ' Prepare default value
        Dim sValue As Single = cCore.NULL_VALUE
        ' Was property found?
        If (mi IsNot Nothing) Then
            ' #Yes: try to extract the value as a SINGLE precision number
            Try
                sValue = CSng(mi.Invoke(indicators, New Object() {}))
            Catch ex As Exception
                ' A failure is due to a programming error
                Debug.Assert(False, "Property " & Me.Abbreviation & " cannot be converted to Single")
            End Try
        End If
        ' Return value
        Return sValue

    End Function

    Public Overrides Function ToString() As String
        Return Me.Name
    End Function

#End Region ' Public access

End Class
