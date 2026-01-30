' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports EwECore
Imports EwECore.Common



''' <summary>
''' Core data layer wrapper for transect raster data.
''' </summary>
Public Class cTransectLayer
    Inherits cEcospaceLayerSingle

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor for an NxN layer of Single values, that derives its data and 
    ''' identity from a manager.
    ''' </summary>
    ''' <param name="core"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(core As cCore, ds As cTransectDatastructures)
        ' Provide a bogus varname (but not NotSet!) as the manager does not care
        MyBase.New(core, ds, My.Resources.RASTER_TRANSECT_NAME, eVarNameFlags.Author)
    End Sub

    Public Overrides Property Cell(iRow As Integer, iCol As Integer, Optional iIndexSec As Integer = -9999) As Object
        Get
            Dim cells As Point() = CType(Me.Manager.LayerData(eVarNameFlags.NotSet, 0), Point())
            If (cells Is Nothing) Then Return cCore.NULL_VALUE
            If Not cells.Contains(New Point(iCol, iRow)) Then Return cCore.NULL_VALUE
            Return 1
        End Get
        Set(value As Object)
            ' NOP
        End Set
    End Property

End Class

