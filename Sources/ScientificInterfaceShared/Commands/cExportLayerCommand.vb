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
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Definitions
Imports EwECore

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'Export Ecospace Layer Data' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cExportLayerCommand
        Inherits cCommand

        Private m_alayers() As cEcospaceLayer = Nothing

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~exportLayer"

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.new(cmdh, cExportLayerCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' ---------------------------------------------------------------------------
        Public Overrides Sub Invoke()
            Me.Invoke(New cEcospaceLayer() {})
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' <param name="alayers">The layers to export data from.</param>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal alayers() As cEcospaceLayer)
            Me.m_alayers = alayers
            MyBase.Invoke()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' <param name="arl">Array of raster layers to export data from.</param>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal arl() As cDisplayRasterLayer)

            Dim layers As New List(Of cEcospaceLayer)
            Dim layer As cEcospaceLayer = Nothing

            If (arl IsNot Nothing) Then
                For Each l As cDisplayLayer In arl
                    If TypeOf l Is cDisplayRasterLayerBundle Then
                        Dim rlb As cDisplayRasterLayerBundle = DirectCast(l, cDisplayRasterLayerBundle)
                        For i As Integer = 0 To rlb.nLayers
                            layer = rlb.Data(i)
                            If (layer IsNot Nothing) Then
                                layers.Add(layer)
                            End If
                        Next
                    ElseIf TypeOf l Is cDisplayRasterLayer Then
                        Dim rl As cDisplayRasterLayer = DirectCast(l, cDisplayRasterLayer)
                        layers.Add(rl.Data)
                    End If
                Next
            End If

            Me.Invoke(layers.ToArray)

        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get the raster layers the command was invoked for.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public ReadOnly Property Layers() As cEcospaceLayer()
            Get
                Return Me.m_alayers
            End Get
        End Property

    End Class

End Namespace ' Commands
