#Region " Imports "

Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Helper class, provides <see cref="IUIElement">User Interface</see>
    ''' elements with contextual information such as the core instance to
    ''' use, a style guide reference, and possibly other elements.
    ''' </summary>
    ''' =======================================================================
    Public Class cUIContext

#Region " Privates vars "

        ''' <summary>The core that a UI interfaces with.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>The style guide that a UI interfaces with.</summary>
        Private m_sg As cStyleGuide = Nothing

#End Region ' Privates vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="core">The <see cref="cCore">core</see> that a UI interfaces with.</param>
        ''' <param name="sg">The <see cref="cStyleGuide">style guide</see> that a UI interfaces with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, ByVal sg As cStyleGuide)
            Me.m_core = core
            Me.m_sg = sg
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cCore">core</see> that a UI interfaces with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Core() As cCore
            Get
                Return Me.m_core
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cStyleGuide">style guide</see> that a UI interfaces with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property StyleGuide() As cStyleGuide
            Get
                Return Me.m_sg
            End Get
        End Property

    End Class

End Namespace ' Controls
