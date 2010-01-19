'==============================================================================
'
' $Log: cFlowDiagramData.vb,v $
' Revision 1.1  2009/06/22 02:25:37  jeroens
' Revamped, rewarped and lobotomized
'
'==============================================================================

Option Strict On
Imports EwECore

Namespace Ecopath.Controls.FlowDiagram

    Public Class cFlowDiagramData

        Private m_sg As cStyleGuide = cStyleGuide.GetInstance()
        Private m_core As cCore = Nothing

#Region " Constructor "

        Public Sub New()
            Me.m_core = cCore.GetInstance()
        End Sub

#End Region ' Constructor

#Region " Properties "

        Public ReadOnly Property NumGroups() As Integer
            Get
                Return Me.m_core.nGroups
            End Get
        End Property

        Public ReadOnly Property Biomass(ByVal iIndex As Integer) As Single
            Get
                Return Me.m_core.EcoPathGroupOutputs(iIndex).Biomass
            End Get
        End Property

        Public ReadOnly Property GroupName(ByVal iIndex As Integer) As String
            Get
                Return Me.m_core.EcoPathGroupInputs(iIndex).Name
            End Get
        End Property

        Public ReadOnly Property GroupColor(ByVal iGroup As Integer) As Color
            Get
                Return Me.m_sg.GroupColor(Me.m_core, iGroup)
            End Get
        End Property

        Public ReadOnly Property GroupVisible(ByVal iGroup As Integer) As Boolean
            Get
                Return Me.m_sg.GroupVisible(iGroup)
            End Get
        End Property

        Public ReadOnly Property Diet(ByVal iPred As Integer, ByVal iPrey As Integer) As Single
            Get
                Dim group As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iPred)
                Return group.DietComp(iPrey)
            End Get
        End Property

        Public ReadOnly Property TrophicLevel(ByVal iIndex As Integer) As Single
            Get
                Return Me.m_core.EcoPathGroupOutputs(iIndex).TTLX
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
