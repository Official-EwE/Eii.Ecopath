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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore.Common
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' -----------------------------------------------------------------------
''' <summary>
''' Provides access to Forcing and EggProduction shapes, and a base class 
''' for Mediation functions.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cForcingFunction
    Inherits cShapeData

#Region " Protected data "

    Private m_bAllowValidation As Boolean = True

    Protected m_iIndex As Integer = 0
    Protected m_data As cEcosimDatastructures
    Protected m_manager As cBaseShapeManager

    '   Protected m_Type As eDataTypes
    Protected m_nYears As Integer

    ' Parameters use to build a Curve
    Protected m_ShapeFunctionType As Long
    Protected m_params As Single() = New Single() {}

    'this flag is used to stop updating during initialization
    'it is more of a safe guard 
    Protected m_bInInit As Boolean

    Protected m_bLockUpdates As Boolean
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cForcingFunction)()

#End Region ' Protected data

#Region " Public fields/properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the shape from a shape function. This updates the shape data,
    ''' the <see cref="ShapeFunctionType"/>, and <see cref="ShapeFunctionParameter(Integer)"/>
    ''' values.
    ''' </summary>
    ''' <param name="fn">The shapefunction to update from.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Reshape(fn As IShapeFunction)
        ' Safety check
        If (fn Is Nothing) Then Return
        ' Update
        Me.ShapeData = fn.Shape(Me.nPoints)
        Me.m_ShapeFunctionType = fn.ShapeFunctionType
        ReDim Me.m_params(fn.nParameters)
        For i As Integer = 1 To fn.nParameters
            Me.ShapeFunctionParameter(i) = fn.ParamValue(i)
        Next
        Me.Update()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eShapeFunctionType"/> that defines the forcing function.
    ''' </summary>
    ''' <seealso cref="Reshape(IShapeFunction)"/>
    ''' -----------------------------------------------------------------------
    Public Property ShapeFunctionType() As Long
        Get
            Return Me.m_ShapeFunctionType
        End Get
        Friend Set(value As Long)
            If (value <> Me.m_ShapeFunctionType) Then
                Me.m_ShapeFunctionType = value
                Me.Update()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of shape function parameters.
    ''' </summary>
    ''' <seealso cref="ShapeFunctionType()"/>
    ''' <seealso cref="ShapeFunctionParameter(Integer)"/>
    ''' <seealso cref="ShapeFunctionParameters()"/>
    ''' <seealso cref="Reshape(IShapeFunction)"/>
    ''' <seealso cref="IShapeFunction"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property nParams As Integer
        Get
            Return Me.m_params.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the value of a shape function parameter.
    ''' </summary>
    ''' <param name="iParam">One-based parameter index.</param>
    ''' <seealso cref="ShapeFunctionType()"/>
    ''' <seealso cref="nParams()"/>
    ''' <seealso cref="ShapeFunctionParameters()"/>
    ''' <seealso cref="Reshape(IShapeFunction)"/>
    ''' <seealso cref="IShapeFunction"/>
    ''' -----------------------------------------------------------------------
    Public Property ShapeFunctionParameter(iParam As Integer) As Single
        Get
            If 1 <= iParam And iParam <= Me.nParams Then
                Return Me.m_params(iParam - 1)
            End If
            Return cCore.NULL_VALUE
        End Get
        Set(value As Single)
            If 1 <= iParam And iParam <= Me.nParams Then
                Me.m_params(iParam - 1) = value
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get an array with the values for all shape function parameters.
    ''' </summary>
    ''' <seealso cref="ShapeFunctionType()"/>
    ''' <seealso cref="nParams()"/>
    ''' <seealso cref="Reshape(IShapeFunction)"/>
    ''' <seealso cref="IShapeFunction"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ShapeFunctionParameters() As Single()
        Get
            Return Me.m_params
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Index of the shape in the list managers list of shape
    ''' </summary>
    ''' <remarks>This is a zero based index set when the shape is added to the manager (Construction of the shape) </remarks>
    ''' -----------------------------------------------------------------------
    Public Property ID() As Integer

    Public Property NYears() As Integer
        Get
            Return Me.m_nYears
        End Get
        Friend Set(value As Integer)
            Me.m_nYears = value
            Me.Update()
        End Set
    End Property

#End Region ' Public fields/properties

#Region " Constructors and Initialization "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new Forcing Function Shape from the underlying EcoSim Data.
    ''' </summary>
    ''' <param name="esData"><see cref="cEcosimDatastructures">Ecosim data structure</see>
    ''' to create the forcing function from.</param>
    ''' <param name="Manager"></param>
    ''' <param name="iDBID"></param>
    ''' <param name="DataType"></param>
    ''' <remarks>
    ''' This is used by the Manager to create forcing function from the 
    ''' underlying EcoSim data.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Friend Sub New(ByRef esData As cEcosimDatastructures, _
                   ByRef Manager As cBaseShapeManager, _
                   iDBID As Integer, _
                   DataType As eDataTypes)

        MyBase.New(esData.ForcePoints)

        Me.m_bInInit = True
        Me.m_data = esData

        Me.m_datatype = DataType
        Me.m_coreComponent = Me.CoreComponent
        Me.m_iDBID = iDBID

        Me.m_manager = Manager 'keep a reference to the manager for this shape

        'Load()

        Me.m_bInInit = False

    End Sub


    ''' <summary>
    ''' Initialize the propeties from the underlying EcoSim data structures for this shapes Database ID 
    ''' </summary>
    ''' <returns>True if successful</returns>
    ''' <remarks>This seperates creation from initialization so that an existing object can be repopluated from its underlying data</remarks>
    Protected Friend Overridable Function Load() As Boolean

        Me.m_iEcoSimIndex = Array.IndexOf(Me.m_data.ForcingDBIDs, Me.m_iDBID)
        Debug.Assert(Me.m_iEcoSimIndex <> -1, "Failed to find index for Shape.")

        If Me.m_iEcoSimIndex = -1 Then Return False
        Me.m_bInInit = True
        Me.LockUpdates()

        'copy the data from zscale into an array that will be used to create a forcing data object
        Me.Init(Me.m_data.ForcePoints)
        For ipt As Integer = 1 To Me.m_data.ForcePoints
            Me.ShapeData(ipt) = Me.m_data.zscale(ipt, Me.m_iEcoSimIndex)
        Next ipt

        Me.Name = Me.m_data.ForcingTitles(Me.m_iEcoSimIndex)

        Me.m_nYears = Me.m_data.NumYears

        'shape parameters
        Me.m_ShapeFunctionType = Me.m_data.ForcingShapeParams(Me.m_iEcoSimIndex).ShapeFunctionType
        Me.m_params = CType(Me.m_data.ForcingShapeParams(Me.m_iEcoSimIndex).ShapeFunctionParams.Clone(), Single())

        Me.IsSeasonal = Me.m_data.isSeasonal(Me.m_iEcoSimIndex)

        Me.UnlockUpdates()
        Me.m_bInInit = False

        Return True

    End Function

    ''' <inheritdocs cref="cShapeData.Dispose"/>
    Friend Overrides Sub Dispose()
        MyBase.Dispose()
        Me.m_data = Nothing
    End Sub

    ''' <inheritdocs cref="cShapeData.Clear"/>
    Public Overrides Sub Clear()
        MyBase.Clear()
    End Sub

#End Region ' Constructors and Initialization

#Region " Updating "

    ''' <summary>
    ''' Update the already existing underlying EcoSim data structures (m_data)
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' <remarks>This gets called by the cForcingData when it has been edited to update the existing EcoSim data</remarks>
    Public Overrides Function Update() As Boolean

        Try

            Debug.Assert(Me.m_data IsNot Nothing, Me.ToString & ".Update() underlying ecosim data has not been set.")

            'do not update during initialization
            If Me.m_bInInit Then
                'update will be call be the Forcing Data object (m_xData) when it is populated it has no way of knowing who is changing its value
                'so it has to call update on its parent
                Return False
            End If

            If Me.IsSeasonal Then
                ' At the end of an edit, extend seasonal pattern until the end of the shape
                Me.LockUpdates()
                For ipt As Integer = cCore.N_MONTHS + 1 To Me.nPoints
                    Me.ShapeData(ipt) = Me.ShapeData(1 + ((ipt - 1) Mod cCore.N_MONTHS))
                Next ipt
                Me.UnlockUpdates(False)
            End If

            'turn the Database ID into an Array index using the Ecosim Data structures database ID this value should be good
            Me.m_iEcoSimIndex = Array.IndexOf(Me.m_data.ForcingDBIDs, Me.m_iDBID)
            Debug.Assert(Me.m_iEcoSimIndex >= 0, Me.ToString & ".Update() Failed to find index for Database ID " & Me.m_iDBID)
            If (Me.m_iEcoSimIndex = cCore.NULL_VALUE) Or (Me.m_iEcoSimIndex > Me.m_data.NumForcingShapes) Then
                m_logger.LogError(Me.ToString & ".Update() index out of bounds. Data not updated.")
                Return False
            End If

            'make sure the shape data is the same size as the EcoSim Shape data
            'this is a double check as the data size was checked when the forcing function was created by the Shape Manager
            'however it could have been changed by an interface at a later date
            Me.ResizeData(Me.m_data.ForcePoints)

            'populate the raw shape data
            For ipt As Integer = 1 To Me.nPoints
                Me.m_data.zscale(ipt, Me.m_iEcoSimIndex) = Me.ShapeData(ipt)
            Next ipt
            Me.m_data.ForcingTitles(Me.m_iEcoSimIndex) = Me.Name

            Me.m_data.ForcingShapeType(Me.m_iEcoSimIndex) = Me.m_datatype

            'shape parameters
            Me.m_data.ForcingShapeParams(Me.m_iEcoSimIndex).ShapeFunctionType = Me.m_ShapeFunctionType
            Me.m_data.ForcingShapeParams(Me.m_iEcoSimIndex).ShapeFunctionParams = CType(Me.m_params.Clone(), Single())

            Me.m_data.isSeasonal(Me.m_iEcoSimIndex) = Me.IsSeasonal()

            Me.ShapeChanged()
            Return True

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".update() Error: " & ex.Message)
            m_logger.LogError(ex, ".update() Error: " & ex.Message)
            Return False

        End Try

    End Function

    ''' <summary>
    ''' Tell the manager that a shape has changed
    ''' </summary>
    Friend Sub ShapeChanged()

        'tell the manager that a shape has changed its data
        If Not Me.IsLockedUpdates Then Me.m_manager.ShapeChanged(Me)

    End Sub

#End Region ' Updating

    Public Overridable Function ToCSVString() As String
        Return Me.Name '+ ", mean " + Me.m_p3.ToString + ", YZero " + Me.m_p0.ToString + ", YEnd " + Me.m_p2.ToString
    End Function

End Class ' cForcingFunction


