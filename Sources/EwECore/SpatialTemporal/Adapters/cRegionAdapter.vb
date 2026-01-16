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


Imports EwECore.SpatialData

Public Class cRegionAdapter
    Inherits cSpatialDataAdapter

    Public Sub New(core As cCore, var As eVarNameFlags, cc As eCoreCounterTypes)
        MyBase.New(core, var, cc)
    End Sub

    Protected Overrides Function SetCell(layer As cEcospaceLayer, conn As cSpatialDataConnection, iRow As Integer, iCol As Integer, sCellValueAtT As Double) As Boolean
        Return MyBase.SetCell(layer, conn, iRow, iCol, CInt(sCellValueAtT))
    End Function

End Class
