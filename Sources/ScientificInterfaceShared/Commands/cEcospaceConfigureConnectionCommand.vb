' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'







Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' A <see cref="cCommand">Command</see> to invoke the ecospace data connections
    ''' interface.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEcospaceConfigureConnectionCommand
        Inherits cCommand

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared cCOMMAND_NAME As String = "~ecospaceconfigureconnection"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, cCOMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the command to make the EwE6 GUI navigate to user interface
        ''' element defined by this call.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(layer As cEcospaceLayer, _
                                    Optional conn As SpatialData.cSpatialDataConnection = Nothing)
            Me.Layer = layer
            Me.Connection = conn
            MyBase.Invoke()
            Me.Layer = Nothing
            Me.Connection = Nothing
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cEcospaceLayer"/> this command was invoked for,
        ''' if any.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Layer() As cEcospaceLayer
            Get
                Return DirectCast(Me.Parameter("Layer"), cEcospaceLayer)
            End Get
            Set(value As cEcospaceLayer)
                Me.Parameter("Layer") = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="SpatialData.cSpatialDataConnection"/> to edit, if any.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Connection As SpatialData.cSpatialDataConnection
            Get
                Return DirectCast(Me.Parameter("Connection"), SpatialData.cSpatialDataConnection)
            End Get
            Private Set(value As SpatialData.cSpatialDataConnection)
                Me.Parameter("Connection") = value
            End Set
        End Property

    End Class

End Namespace
