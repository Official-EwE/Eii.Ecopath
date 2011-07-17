Imports EwECore

Public Class cAnomalySearchShapeGUIHandler
    Inherits cForcingShapeGUIHandler

    ''' ---------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uic"></param>
    ''' <param name="stb"></param>
    ''' <param name="sp"></param>
    ''' ---------------------------------------------------------------
    Public Shadows Sub Attach(ByVal uic As cUIContext, _
                              ByVal stb As ucShapeToolbox, _
                              ByVal sp As ucSketchPad)
        MyBase.Attach(uic, stb, Nothing, sp, Nothing)
    End Sub

    ' ''' ---------------------------------------------------------------
    ' ''' <summary>
    ' ''' 
    ' ''' </summary>
    ' ''' ---------------------------------------------------------------
    'Public Overrides Property SketchPad() As ucSketchPad
    '    Get
    '        Return MyBase.SketchPad
    '    End Get
    '    Set(ByVal value As ucSketchPad)
    '        MyBase.SketchPad = value
    '    End Set
    'End Property

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

    Protected Overrides Function NumDataYears() As Integer
        Return Me.UIContext.Core.nTimeSeriesYears
    End Function

End Class
