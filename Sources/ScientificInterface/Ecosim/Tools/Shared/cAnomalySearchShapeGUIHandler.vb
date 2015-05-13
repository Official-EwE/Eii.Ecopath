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

Imports EwECore

Public Class cAnomalySearchShapeGUIHandler
    Inherits cForcingShapeGUIHandler

    Public Sub New(uic As cUIContext)
        MyBase.New(uic)
    End Sub

    ''' ---------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="stb"></param>
    ''' <param name="sp"></param>
    ''' ---------------------------------------------------------------
    Public Shadows Sub Attach(ByVal stb As ucShapeToolbox, _
                              ByVal sp As ucSketchPad)
        MyBase.Attach(stb, Nothing, sp, Nothing)
    End Sub

    ''' ---------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="shape"></param>
    ''' <returns></returns>
    ''' ---------------------------------------------------------------
    Protected Overrides Function IncludeShape(ByVal shape As EwECore.cShapeData) As Boolean
        Dim manager As cMediatedInteractionManager = Me.Core.MediatedInteractionManager
        If Not (TypeOf shape Is cForcingFunction) Then Return False
        If (manager Is Nothing) Then Return False
        Return manager.IsApplied(DirectCast(shape, cForcingFunction))
    End Function

    Public Overrides Function NumDataYears() As Integer
        Return Me.UIContext.Core.nTimeSeriesYears
    End Function

End Class
