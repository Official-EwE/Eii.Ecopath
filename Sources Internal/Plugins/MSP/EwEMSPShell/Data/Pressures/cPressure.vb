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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Pressure data derived from MSP game play actions to impact the Ecospace model.
''' </summary>
''' <remarks>
''' In a <see cref="cGame">MSP game</see>, player actions translate to pressures.
''' This pressure data is received in cPressure classes, and are passed on to mapped 
''' <see cref="cDriver">Ecospace drivers</see> to impact the Ecospace model.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cPressure
    Implements IMELItem

#Region " Internal vars "

    ''' <summary>The scalar data wrapped by the pressure.</summary>
    Protected m_scalar As cScalar = Nothing

    ''' <summary>The grid data wrapped by the pressure.</summary>
    Protected m_grid As cGrid = Nothing

#End Region ' Internal vars

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create an instance of a pressure definition. Pressure definitions cannot 
    ''' accept actual pressure data; they just serve to define game dynamics.
    ''' </summary>
    ''' <param name="name">The name of the pressure to define.</param>
    ''' <param name="datatype">The <see cref="eDataTypes">type of data</see>
    ''' that the defined pressure will support.</param>
    ''' <seealso cref="DataType"/>
    ''' <seealso cref="eDataTypes"/>
    ''' -----------------------------------------------------------------------
    Friend Sub New(name As String, datatype As eDataTypes)
        MyBase.New()
        Me.DataType = datatype
        Me.Name = name
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a <see cref="eDataTypes.Scalar">scalar</see> pressure.
    ''' </summary>
    ''' <param name="name">The name of the pressure to define.</param>
    ''' <param name="scalar">The initial data for the pressure.</param>
    ''' <seealso cref="DataType"/>
    ''' <seealso cref="eDataTypes"/>
    ''' <seealso cref="Scalar"/>
    ''' <see cref="IsScalar"/>
    ''' -----------------------------------------------------------------------
    Public Sub New(name As String, scalar As Double)
        Me.New(name, eDataTypes.Scalar)
        Me.m_scalar = New cScalar(name, scalar)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a <see cref="eDataTypes.Grid">grid</see> pressure.
    ''' </summary>
    ''' <param name="name">The name of the pressure to define.</param>
    ''' <param name="iNumRows">The number of rows in the pressure grid.</param>
    ''' <param name="iNumColumns">The number of columns in the pressure grid.</param>
    ''' <param name="data">Optional initial data for the pressure.</param>
    ''' <seealso cref="DataType"/>
    ''' <seealso cref="eDataTypes"/>
    ''' <seealso cref="Grid"/>
    ''' <see cref="IsGrid"/>
    ''' -----------------------------------------------------------------------
    Public Sub New(name As String, iNumColumns As Integer, iNumRows As Integer, Optional data As Double(,) = Nothing)
        Me.New(name, eDataTypes.Grid)
        Me.m_grid = New cGrid(name, iNumColumns, iNumRows, data)
    End Sub

#End Region ' Constructors

#Region " Public bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type, defining the types of data that a pressure supports.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eDataTypes As Byte
        ''' <summary>Data type has not been set yet.</summary>
        NotSet = 0
        ''' <summary>Pressure contains a single value.</summary>
        Scalar
        ''' <summary>Pressure contains an map of values.</summary>
        Grid
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the pressure.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Name As String Implements IMELItem.Name

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="eDataTypes">type of data</see> that a pressure supports.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property DataType As eDataTypes

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a reference to the <see cref="cGrid"/> wrapped by the pressure, if 
    ''' this is a <see cref="eDataTypes.Grid"/> pressure.
    ''' </summary>
    ''' <returns>The grid, or null if the pressure does not accept grid data.</returns>
    ''' <seealso cref="IsGrid"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Grid As cGrid
        Get
            If Me.IsGrid Then Return Me.m_grid
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the scalar value wrapped by the pressure, if this is a
    ''' <see cref="eDataTypes.Scalar"/> pressure.
    ''' </summary>
    ''' <returns>The scalar value, or 0 if the pressure does not accept scalar 
    ''' data.</returns>
    ''' <seealso cref="IsScalar"/>
    ''' -----------------------------------------------------------------------
    Public Property Scalar As Double
        Get
            If Me.m_scalar IsNot Nothing Then Return Me.m_scalar.Value
            Return 0
        End Get
        Set(value As Double)
            If Me.IsScalar Then
                If (Me.m_scalar Is Nothing) Then
                    Me.m_scalar = New cScalar(Me.Name, value)
                Else
                    Me.m_scalar.Value = value
                End If
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns if this pressure accepts grid driver data. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsGrid As Boolean
        Get
            Return (Me.DataType = eDataTypes.Grid)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns if this pressure accepts scalar driver data. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property IsScalar As Boolean
        Get
            Return (Me.DataType = eDataTypes.Scalar)
        End Get
    End Property

#End Region ' Public bits

End Class
