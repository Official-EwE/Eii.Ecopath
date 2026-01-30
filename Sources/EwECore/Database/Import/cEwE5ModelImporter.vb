' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Database

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing an importer to convert an EwE5 document
    ''' into an EwE6 database.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class cEwE5ModelImporter
        Inherits cModelImporter

#Region " Private vars "

        ''' <summary>EWE5 NULL value.</summary>
        Protected Const cEWE5_NULL As Integer = -90

#End Region ' Private vars

#Region " Construction "

        Public Sub New(core As cCore)
            MyBase.New(core)
        End Sub

#End Region ' Construction

    End Class

End Namespace
