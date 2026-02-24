' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
    Public Shadows Sub Attach(stb As ucShapeToolbox, sp As ucSketchPad)
        MyBase.Attach(stb, Nothing, sp, Nothing)
    End Sub

    ''' ---------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="shape"></param>
    ''' <returns></returns>
    ''' ---------------------------------------------------------------
    Protected Overrides Function IncludeShape(shape As EwECore.cShapeData) As Boolean

        If (Me.UIContext Is Nothing) Then Return False
        If Not (TypeOf shape Is cForcingFunction) Then Return False

        ' Fixed 
        Dim interactions As cMediatedInteractionManager = Me.Core.MediatedInteractionManager
        Dim shapes As New List(Of cShapeData)
        Dim shpTest As cForcingFunction = Nothing
        Dim interact As cPredPreyInteraction = Nothing
        Dim ft As eForcingFunctionApplication = eForcingFunctionApplication.NotSet
        Dim core As cCore = Me.UIContext.Core

        For iG1 As Integer = 1 To core.nLivingGroups
            For iG2 As Integer = 1 To core.nLivingGroups
                interact = interactions.PredPreyInteraction(iG1, iG2)
                If (interact IsNot Nothing) Then
                    For i As Integer = 1 To interact.nAppliedShapes
                        If (interact.getShape(i, shpTest, ft)) Then
                            If ReferenceEquals(shape, shpTest) Then
                                Return True
                            End If
                        End If
                    Next
                End If
            Next iG2
        Next iG1
        Return False

    End Function

    Public Overrides Function NumDataYears() As Integer
        Return Me.UIContext.Core.nTimeSeriesYears
    End Function

End Class
