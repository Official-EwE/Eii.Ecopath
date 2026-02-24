' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
        Public Overloads Sub Invoke(layer As cEcospaceLayer,
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
