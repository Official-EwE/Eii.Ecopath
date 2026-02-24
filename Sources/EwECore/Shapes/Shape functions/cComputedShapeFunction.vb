' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' <summary>
''' A <see cref="cShapeFunction"/> which points descrbibe a distribution determined
''' by external logic. This function type is not editable.
''' </summary>
Public Class cComputedShapeFunction
    Inherits cShapeFunction

    Public Sub New()
        MyBase.New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub Defaults()
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.IsCompatible(eDataTypes)"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Function IsCompatible(datatype As eDataTypes) As Boolean
        Return Me.IsMediation(datatype)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.IsDistribution()"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property IsDistribution As Boolean
        Get
            Return True
        End Get
    End Property

    ' Not a good idea for serialization: this will bloat the parameter storage

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.nParameters"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property nParameters As Integer
        Get
            Return 2
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the name of a parameter. By default, one of the four standard
    ''' shape paramter names is returned (e.g., YZero, YEnd, YBase and Steepness)
    ''' </summary>
    ''' <param name="iParam">The one-based parameter index [1, <see cref="nParameters"/>]
    ''' to obtain the name for.</param>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ParamName(iParam As Integer) As String
        Get
            Debug.Assert((iParam >= 1) And (iParam <= Me.nParameters))
            Select Case iParam
                Case 1 : Return My.Resources.CoreDefaults.PARAM_MIN
                Case 2 : Return My.Resources.CoreDefaults.PARAM_MAX
            End Select
            Return "?"
        End Get
    End Property

    Public Overrides ReadOnly Property ParamStatus(iParam As Integer) As eStatusFlags
        Get
            Return eStatusFlags.NotEditable
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cShapeFunction.Defaults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property ShapeFunctionType As Long
        Get
            Return eShapeFunctionType.Computed
        End Get
    End Property

    Public Overrides Function Apply(obj As Object) As Boolean
        If MyBase.Apply(obj) Then
            Dim shape As cEnviroResponseFunction = TryCast(obj, cEnviroResponseFunction)
            If shape IsNot Nothing Then
                shape.ResponseLeftLimit = Me.ParamValue(1)
                shape.ResponseRightLimit = Me.ParamValue(2)
            End If
        End If
    End Function

    Public ReadOnly Property nPoints As Integer
        Get
            Return Me.m_points.Count - 1
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="iPoint">One-based index!</param>
    ''' <returns></returns>
    Public Property ShapeData(iPoint As Integer) As Single
        Get
            Return Me.m_points(iPoint)
        End Get
        Set(value As Single)
            Me.m_points(iPoint) = value
        End Set
    End Property

End Class

