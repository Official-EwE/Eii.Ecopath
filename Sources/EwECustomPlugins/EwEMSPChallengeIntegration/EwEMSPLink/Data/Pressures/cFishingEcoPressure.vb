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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'






Public Class cFishingEcoPressure
    Inherits cPressure

    Public Sub New(name As String)
        MyBase.New(name)
    End Sub

    Public Sub New(name As String, bIsEcological As Boolean)
        Me.New(name)
        Me.bIsEcological = bIsEcological
    End Sub

    Public Property bIsEcological As Boolean = False

End Class
