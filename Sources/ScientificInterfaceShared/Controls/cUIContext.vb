#Region " Imports "

Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Properties
Imports System.Threading

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Helper class, provides <see cref="IUIElement">User Interface</see>
    ''' elements with contextual information such as the core instance to
    ''' use, a style guide reference, and other centrally accessible elements
    ''' that some day may require multiple instances.
    ''' </summary>
    ''' =======================================================================
    Public Class cUIContext

#Region " Privates vars "

        ''' <summary>The core that a UI can interact with.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>The style guide that a UI can interact with.</summary>
        Private m_sg As cStyleGuide = Nothing
        ''' <summary>The property manager that a UI can interact with.</summary>
        Private m_propman As cPropertyManager = Nothing
        ''' <summary>The command handler that a UI can interact with.</summary>
        Private m_cmdhandler As cCommandHandler = Nothing
        ''' <summary>The form positions settings that a UI can interact with.</summary>
        Private m_formpos As cFormPositionSettings = Nothing
        ''' <summary>UI thread sync object.</summary>
        Private m_syncObj As SynchronizationContext = Nothing

#End Region ' Privates vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="core">The <see cref="cCore">core</see> that a UI can interact with.</param>
        ''' <param name="sg">The <see cref="cStyleGuide">style guide</see> that a UI can interact with.</param>
        ''' <param name="propman">The <see cref="PropertyManager">property manager</see>
        ''' that a UI can interact with.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
                       ByVal sg As cStyleGuide, _
                       ByVal propman As cPropertyManager, _
                       ByVal cmdhandler As cCommandHandler, _
                       ByVal formpos As cFormPositionSettings, _
                       ByVal syncObj As SynchronizationContext)

            Me.m_core = core
            Me.m_sg = sg
            Me.m_propman = propman
            Me.m_cmdhandler = cmdhandler
            Me.m_formpos = formpos
            Me.m_syncObj = syncObj

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cCore">core</see> that a UI can interact with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Core() As cCore
            Get
                Return Me.m_core
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cStyleGuide">style guide</see> that a UI can
        ''' interact with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property StyleGuide() As cStyleGuide
            Get
                Return Me.m_sg
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cPropertyManager">property manager</see> that a 
        ''' UI can interact with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property PropertyManager() As cPropertyManager
            Get
                Return Me.m_propman
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cCommandHandler">command handler</see> that a 
        ''' UI can interact with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property CommandHander() As cCommandHandler
            Get
                Return m_cmdhandler
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cFormPositionSettings">form position settings manager</see> that a 
        ''' UI can interact with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property FormPositionSettings() As cFormPositionSettings
            Get
                Return Me.m_formpos
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="SynchronizationContext">synchronization object</see> 
        ''' that the user interface was created on.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property SyncObject() As SynchronizationContext
            Get
                Return Me.m_syncObj
            End Get
        End Property

    End Class

End Namespace ' Controls
