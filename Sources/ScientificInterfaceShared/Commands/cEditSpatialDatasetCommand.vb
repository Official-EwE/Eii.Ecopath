' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to edit a spatial temporal dataset.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEditSpatialDatasetCommand
        Inherits cCommand

        Private m_ds As ISpatialDataSet = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~editspatialdataset"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the <see cref="cBrowserCommand"/> class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Invokes the command to edit a spatial dataset in the EwE UI.
        ''' </summary>
        ''' <param name="ds"><see cref="ISpatialDataSet"/> to edit.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ds As ISpatialDataSet)
            Me.m_ds = ds
            MyBase.Invoke()
            Me.m_ds = Nothing
        End Sub

        ''' <summary>
        ''' Get the dataset to configure.
        ''' </summary>
        Public ReadOnly Property Dataset() As ISpatialDataSet
            Get
                Return Me.m_ds
            End Get
        End Property

    End Class

End Namespace
