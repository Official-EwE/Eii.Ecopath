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
' Copyright 2016- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'






Public Class cFishingEffortPressure
    Inherits cPressure

    Public Sub New(name As String)
        MyBase.New(name)
    End Sub

    Public Sub New(name As String, effortscalar As Single)
        Me.New(name)
        Me.EffortScalar = effortscalar
    End Sub

    Public Property EffortScalar As Single = 0

End Class
