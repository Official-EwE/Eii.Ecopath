Option Strict On
Imports EwECore

Namespace Ecopath.Controls.FlowDiagram

    Public Class cFlowDiagramData
        Implements IUIElement

        Private m_uic As cUIContext = Nothing

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.UIContext = uic
        End Sub

#End Region ' Constructor

#Region " Properties "

        Friend Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Private Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Public ReadOnly Property RenderFont() As Font
            Get
                Return Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)
            End Get
        End Property

        Public ReadOnly Property TextColor() As Color
            Get
                Return Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            End Get
        End Property

        Public ReadOnly Property HighlightColor() As Color
            Get
                Return Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
            End Get
        End Property

        Public ReadOnly Property NumGroups() As Integer
            Get
                Return Me.m_uic.Core.nGroups
            End Get
        End Property

        Public ReadOnly Property Biomass(ByVal iIndex As Integer) As Single
            Get
                Return Me.m_uic.Core.EcoPathGroupOutputs(iIndex).Biomass
            End Get
        End Property

        Public ReadOnly Property GroupName(ByVal iIndex As Integer) As String
            Get
                Return Me.m_uic.Core.EcoPathGroupInputs(iIndex).Name
            End Get
        End Property

        Public ReadOnly Property GroupColor(ByVal iGroup As Integer) As Color
            Get
                Return Me.m_uic.StyleGuide.GroupColor(Me.m_uic.Core, iGroup)
            End Get
        End Property

        Public ReadOnly Property GroupVisible(ByVal iGroup As Integer) As Boolean
            Get
                Return Me.m_uic.StyleGuide.GroupVisible(iGroup)
            End Get
        End Property

        Public ReadOnly Property Diet(ByVal iPred As Integer, ByVal iPrey As Integer) As Single
            Get
                Dim group As cEcoPathGroupInput = Me.m_uic.Core.EcoPathGroupInputs(iPred)
                Return group.DietComp(iPrey)
            End Get
        End Property

        Public ReadOnly Property TrophicLevel(ByVal iIndex As Integer) As Single
            Get
                Return Me.m_uic.Core.EcoPathGroupOutputs(iIndex).TTLX
            End Get
        End Property

        Public ReadOnly Property BiomassMax() As Single
            Get
                Dim sMax As Single = 0
                For i As Integer = 1 To Me.NumGroups
                    sMax = Math.Max(sMax, Me.Biomass(i))
                Next i
                Return sMax
            End Get
        End Property

        Public ReadOnly Property MinBiomass() As Single
            Get
                Dim sMin As Single = Me.BiomassMax()
                For i As Integer = 1 To Me.NumGroups
                    sMin = Math.Min(sMin, Me.Biomass(i))
                Next i
                Return Math.Max(0, sMin)
            End Get
        End Property

        Public ReadOnly Property MinDiet() As Single
            Get
                Dim sMin As Single = Me.DietMax
                For i As Integer = 1 To Me.NumGroups
                    For j As Integer = 1 To Me.NumGroups
                        sMin = Math.Min(sMin, Me.Diet(i, j))
                    Next j
                Next i
                Return Math.Max(sMin, 0)
            End Get
        End Property

        Public ReadOnly Property DietMax() As Single
            Get
                Dim sMax As Single = 0
                For i As Integer = 1 To Me.NumGroups
                    For j As Integer = 1 To Me.NumGroups
                        sMax = Math.Max(sMax, Me.Diet(i, j))
                    Next j
                Next i
                Return sMax
            End Get
        End Property

#End Region ' Properties

    End Class

End Namespace
