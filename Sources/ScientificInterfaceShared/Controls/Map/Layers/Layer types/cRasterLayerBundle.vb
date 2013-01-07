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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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

    ''' <summary>
    ''' Layer that wraps a collection of <see cref="cEcospaceLayer"/>s for bundled display in the UI.
    ''' </summary>
    Public Class cRasterLayerBundle
        Inherits cRasterLayer

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

        Public Property iLayer As Integer
            Get
                Dim i As Integer = Me.m_iLayer
                '' Fleets include the 0 'All' fleet
                'If (Me.m_cc <> eCoreCounterTypes.nFleets) Then i += 1
                Return i
            End Get
            Set(value As Integer)
                '' Fleets include the 0 'All' fleet
                'If (Me.m_cc <> eCoreCounterTypes.nFleets) Then value -= 1
                Me.m_iLayer = Math.Max(0, value)
            End Set
        End Property

        Public Overrides ReadOnly Property Data As EwECore.cEcospaceLayer
            Get
                Return Me.m_layers(Me.m_iLayer)
            End Get
        End Property

    End Class ' Layer

End Namespace
