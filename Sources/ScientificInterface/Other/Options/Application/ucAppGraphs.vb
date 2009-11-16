#Region " Imports "

Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Style

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' GUI via which users configure display of graphs.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucAppGraphs

#Region " Variables "

        ''' <summary>Only ref to core.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>Only ref to styleguide.</summary>
        Private m_sg As cStyleGuide = Nothing
        ''' <summary>Prevent loops.</summary>
        Private m_bInUpdate As Boolean = False

#End Region ' Variables

#Region " Constructors "

        Public Sub New()

            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.m_core = cCore.GetInstance()
            Me.m_sg = cStyleGuide.GetInstance()

        End Sub

#End Region ' Constructors

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Control's load event which gets called every time the control gets loaded. 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            Me.m_nudThumbnailSize.Value = CDec(Math.Max(Me.m_nudThumbnailSize.Minimum, Math.Min(Me.m_nudThumbnailSize.Maximum, Me.m_sg.ThumbnailSize)))

            Select Case Me.m_sg.ShowLegends
                Case TriState.UseDefault
                    Me.m_rbLegendSelective.Checked = True
                Case TriState.True
                    Me.m_rbLegendAlways.Checked = True
                Case TriState.False
                    Me.m_rbLegendNever.Checked = True
            End Select

        End Sub

#End Region ' Event handlers

#Region " Public methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save thumbnail size back to the style guide.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Save()

            Dim tsShowLegends As TriState = TriState.UseDefault

            If Me.m_rbLegendAlways.Checked Then
                tsShowLegends = TriState.True
            ElseIf Me.m_rbLegendNever.Checked Then
                tsShowLegends = TriState.False
            End If

            Me.m_sg.SuspendEvents()
            Me.m_sg.ThumbnailSize = CInt(Me.m_nudThumbnailSize.Value)
            Me.m_sg.ShowLegends = tsShowLegends
            Me.m_sg.ResumeEvents()

        End Sub

#End Region ' Public methods

    End Class

End Namespace


