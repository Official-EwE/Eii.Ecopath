' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    Public Interface IShapeFunction

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize to a given shape.
        ''' </summary>
        ''' <param name="shape">The shape to init to.</param>
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
        ReadOnly Property ParamName(iParam As Integer) As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value of a parameter of the shape function.
        ''' </summary>
        ''' <param name="iParam">The index of the parameters [1,<see cref="nParameters"/>]
        ''' to access the value for.</param>
        ''' -----------------------------------------------------------------------
        Property ParamValue(iParam As Integer) As Single

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the human legible unit for a parameter of a shape function.
        ''' </summary>
        ''' <param name="iParam">The index of the parameters [1,<see cref="nParameters"/>]
        ''' to obtain the human legible unit for.</param>
        ''' -----------------------------------------------------------------------
        ReadOnly Property ParamUnit(iParam As Integer) As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the order a parameter needs to appear in the UI. Any UI should honour
        ''' this flag sorting parameters from low to high order.
        ''' </summary>
        ''' <param name="iParam">The index of the parameters [1,<see cref="nParameters"/>]
        ''' to obtain order for.</param>
        ''' -----------------------------------------------------------------------
        ReadOnly Property ParamStatus(iParam As Integer) As eStatusFlags

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the actual function data points, as computed from the <see cref="ParamValue">parameters values</see>.
        ''' </summary>
        ''' <param name="nPoints">The number of points to calculate the shape for.</param>
        ''' <returns>An array of points.</returns>
        ''' -----------------------------------------------------------------------
        Function Shape(nPoints As Integer) As Single()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return whether a shape function is relevant for a given <see cref="eDataTypes">data type</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Function IsCompatible(datatype As eDataTypes) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update a shape from the shape function.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Function Apply(shape As Object) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a (hopefully) unique identifier for a particular shape function,
        ''' regardless if this function is built-in to EwE or is provided by a plug-in.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property ShapeFunctionType() As Long

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether the shape function is a true distribution, with fixed
        ''' min and max values.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property IsDistribution() As Boolean

    End Interface

End Namespace

