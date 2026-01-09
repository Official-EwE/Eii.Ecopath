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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports EwECore



Public Class cShapeDataComparer
    Implements IComparer(Of cShapeData)

    Public Function Compare(x As cShapeData, y As cShapeData) As Integer Implements IComparer(Of cShapeData).Compare
        If (x Is Nothing) Then Return -1
        If (y Is Nothing) Then Return 1
        Return x.Index.CompareTo(y.Index)
    End Function

End Class
