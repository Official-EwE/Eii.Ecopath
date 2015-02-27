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
Imports EwECore
Imports EwECore.Auxiliary
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Properties
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map.Layers

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Layer that wraps a collection of <see cref="cEcospaceLayer"/>s for 
    ''' bundled display and processing in the UI. The indexing of the bundled
    ''' data is based on <see cref="eCoreCounterTypes"/>.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Class cDisplayRasterLayerBundle
        Inherits cDisplayRasterLayer

        Private m_layers As cEcospaceLayer()
        Private m_iLayer As Integer = 0
        Private m_cc As eCoreCounterTypes = eCoreCounterTypes.NotSet

        Public Sub New(ByVal uic As cUIContext, _
                       ByVal data As cEcospaceLayer(), _
                       ByVal renderer As cLayerRenderer, _
                       ByVal editor As cLayerEditor, _
                       ByVal cc As eCoreCounterTypes, _
                       ByVal source As cCoreInputOutputBase, _
                       Optional ByVal varName As eVarNameFlags = eVarNameFlags.Name, _
                       Optional ByVal sValueSet As Single = cCore.NULL_VALUE, _
                       Optional ByVal sValueClear As Single = cCore.NULL_VALUE)

            MyBase.New(uic, data(0), renderer, editor, source, varName, sValueSet, sValueClear)

            ' Sanity check
            Debug.Assert(cc <> eCoreCounterTypes.NotSet, "Cannot declare a layer bundle without providing a core counter that this bundle uses")

            Me.m_cc = cc

            ReDim Me.m_layers(uic.Core.GetCoreCounter(cc))
            For Each l As cEcospaceLayer In data
                Try
                    Me.m_layers(l.Index) = l
                Catch ex As Exception

                End Try
            Next

            For i As Integer = 0 To Me.m_layers.Length - 1
                If Me.m_layers(i) IsNot Nothing Then Me.m_iLayer = i : Exit For
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the index of the current active layer in the bundle.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property iLayer As Integer
            Get
                Return Me.m_iLayer
            End Get
            Set(value As Integer)
                Me.m_iLayer = Math.Max(0, Math.Min(value, Me.m_uic.Core.GetCoreCounter(Me.m_cc)))
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of layers in the bundle.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property nLayers As Integer
            Get
                Return Me.m_uic.Core.GetCoreCounter(Me.CoreCounter)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eCoreCounterTypes"/> that defines the indexing
        ''' of the layers bundled in this class.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property CoreCounter As eCoreCounterTypes
            Get
                Return Me.m_cc
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the secundary data index for a raster bundle. The type of 
        ''' object is derived from the bundle <see cref="CoreCounter"/>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property SourceSec As cCoreInputOutputBase
            Get
                If (Me.m_iLayer = 0) Then Return Nothing

                Select Case Me.m_cc
                    Case eCoreCounterTypes.nGroups
                        Return Me.m_uic.Core.EcoPathGroupInputs(Me.m_iLayer)
                    Case eCoreCounterTypes.nFleets
                        Return Me.m_uic.Core.FleetInputs(Me.m_iLayer)
                    Case Else
                        Debug.Assert(False)
                End Select
                Return Nothing
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the Ecospace layer <see cref="iLayer">currently active</see> in
        ''' the bundle.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides ReadOnly Property Data As EwECore.cEcospaceLayer
            Get
                Return Me.Data(Me.m_iLayer)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get an Ecospace layer from the bundle.
        ''' </summary>
        ''' <param name="iLayer">The index of the layer to obtain. Note that this
        ''' value cannot exceed the range stipulted by the underlying <see cref="CoreCounter"/>.</param>
        ''' -------------------------------------------------------------------
        Public Overloads ReadOnly Property Data(ByVal iLayer As Integer) As EwECore.cEcospaceLayer
            Get
                Debug.Assert(iLayer <= Me.m_uic.Core.GetCoreCounter(Me.m_cc))
                Return Me.m_layers(iLayer)
            End Get
        End Property

        Public Overrides Property Name As String
            Get
                Return Me.m_layers(Me.m_iLayer).Name
            End Get
            Set(value As String)
                Me.m_layers(Me.m_iLayer).Name = value
            End Set
        End Property

    End Class ' Layer

End Namespace
