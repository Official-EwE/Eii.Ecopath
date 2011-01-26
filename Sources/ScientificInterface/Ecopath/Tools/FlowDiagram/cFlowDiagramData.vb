Option Strict On
Imports EwECore

Namespace Ecopath.Controls.FlowDiagram

    Public Class cFlowDiagramData
        Implements IUIElement

        Private m_uic As cUIContext = Nothing
        Private m_sDietMin As Single = 0
        Private m_sDietMax As Single = 0
        Private m_sBiomassMin As Single = 0
        Private m_sBiomassMax As Single = 0

        Private m_bInvalid As Boolean = True

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

        Public Sub Refresh()
            Me.m_bInvalid = True
        End Sub

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

        Public ReadOnly Property HighlightEatsColor() As Color
            Get
                Return Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
            End Get
        End Property

        Public ReadOnly Property HighlightIsEatenColor() As Color
            Get
                Return Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.CHECKED_BACKGROUND)
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
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sBiomassMax
            End Get
        End Property

        Public ReadOnly Property MinBiomass() As Single
            Get
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sBiomassMin
            End Get
        End Property

        Public ReadOnly Property MinDiet() As Single
            Get
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sDietMin
            End Get
        End Property

        Public ReadOnly Property DietMax() As Single
            Get
                If Me.m_bInvalid Then Me.Recalc()
                Return Me.m_sDietMax
            End Get
        End Property

#End Region ' Properties

#Region " Internals "

        Private Sub Recalc()

            If Not Me.m_bInvalid Then Return

            Me.m_sBiomassMax = 0
            Me.m_sBiomassMin = Single.MaxValue
            Me.m_sDietMax = 0
            Me.m_sDietMin = Single.MaxValue

            For i As Integer = 1 To Me.NumGroups
                For j As Integer = 1 To Me.NumGroups
                    Dim sDiet As Single = Me.Diet(i, j)
                    Me.m_sDietMax = Math.Max(Me.m_sDietMax, sDiet)
                    Me.m_sDietMin = Math.Min(Me.m_sDietMin, sDiet)

                    Dim sB As Single = Me.Biomass(i)
                    Me.m_sBiomassMax = Math.Max(Me.m_sBiomassMax, sDiet)
                    Me.m_sBiomassMin = Math.Min(Me.m_sBiomassMin, sDiet)
                Next j
            Next i

            Me.m_bInvalid = False

        End Sub

#End Region ' Interals

    End Class

End Namespace
