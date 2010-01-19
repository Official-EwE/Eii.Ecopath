#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Collections
Imports System.Windows.Forms

#End Region ' Imports

Namespace Controls.Wizard

    ''' =======================================================================
    ''' <summary>
    ''' Base class for implementing a GUI-driven wizard.
    ''' </summary>
    ''' <remarks>
    ''' Note that this class can be severely improved. For one, it does
    ''' not support branches in the logic. Pages be connected in a parent/child
    ''' tree structure, etc. For now I have not bothered.
    ''' </remarks>
    ''' =======================================================================
    <CLSCompliant(True)> _
    Public Class cWizard

#Region " Private vars "

        ''' <summary>Core that this wizard operates on.</summary>
        Private m_core As cCore = Nothing

        ''' <summary>List of wizard pages.</summary>
        Private m_lPages As New List(Of Type)
        ''' <summary>Index of active page.</summary>
        Private m_iPageActive As Integer = -1
        ''' <summary>The current active page.</summary>
        Private m_page As IWizardPage = Nothing

        ''' <summary>Navigator attached to this wizard.</summary>
        Private m_nav As IWizardNavigation = Nothing
        ''' <summary>Form hosting this wizard.</summary>
        Private m_parent As Form = Nothing
        ''' <summary>Panel where wizard can display its content.</summary>
        Private m_content As Panel = Nothing

#End Region ' Private vars 

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, create a new wizard and embeds itself into your form, muahahaha!
        ''' </summary>
        ''' <param name="parent">Form hosting this wizard.</param>
        ''' <param name="content">Panel where wizard can display its content.</param>
        ''' <param name="nav">Navigator attached to this wizard.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore, _
                       ByVal parent As Form, _
                       ByVal content As Panel, _
                       ByVal nav As IWizardNavigation)

            ' Sanity checks
            Debug.Assert(nav IsNot Nothing)

            Me.m_core = core

            Me.m_parent = parent
            Me.m_content = content

            Me.m_nav = nav
            Me.m_nav.Attach(Me)

        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a page to the wizard.
        ''' </summary>
        ''' <param name="tpage">Type of page class to add to the wizard. Note
        ''' that a page must inherit from <see cref="Control">Windows.Forms.Control</see>,
        ''' and must implement the <see cref="IWizardPage">IWizardPage</see>
        ''' interface.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddPage(ByVal tpage As Type)

            ' Sanity checks
            Debug.Assert(GetType(IWizardPage).IsAssignableFrom(tpage), "Page must implement IWizardPage")
            Debug.Assert(GetType(IWizardPage).IsAssignableFrom(tpage), "Page must be a valid Windows Forms Control")

            ' Add page type to the list of candidate pages
            Me.m_lPages.Add(tpage)
            ' Is this the first page added?
            If (Me.m_iPageActive = -1) Then
                ' #Yes: show this page
                Me.SwitchPage(0)
            Else
                ' #No: just update the navigation
                Me.m_nav.UpdateNavigation()
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for wizard pages to inform the wizard that a pages content
        ''' has changed.
        ''' </summary>
        ''' <param name="page">The page whose content changed.</param>
        ''' -------------------------------------------------------------------
        Public Sub PageChanged(ByVal page As IWizardPage)
            ' Is this the current active page?
            If (Object.ReferenceEquals(page, Me.m_page)) Then
                ' #Yes: refresh navigation
                Me.m_nav.UpdateNavigation()
            End If
        End Sub
#End Region ' Public access

#Region " Context "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the core that this wizard operates on.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Core() As cCore
            Get
                Return Me.m_core
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the active page in the wizard.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property ActivePage() As IWizardPage
            Get
                Return Me.m_page
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the parent form hosting the wizard.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property Parent() As Form
            Get
                Return Me.m_parent
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the navigator controlling this wizard.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property Navigation() As IWizardNavigation
            Get
                Return Me.m_nav
            End Get
        End Property

#End Region ' Context

#Region " Navigation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Switch to a new wizard page.
        ''' </summary>
        ''' <param name="iPage">Index of the page to switch to.</param>
        ''' -------------------------------------------------------------------
        Protected Sub SwitchPage(ByVal iPage As Integer)
            Dim ctrl As Control = Nothing

            ' Optimization
            If (iPage = Me.m_iPageActive) Then Return

            ' Hold layout while switching
            Me.m_content.SuspendLayout()

            ' Clear existing page
            If Me.m_page IsNot Nothing Then
                Me.m_page.Close()
                Me.m_content.Controls.Clear()
            End If

            ' Truncate page number
            Me.m_iPageActive = Math.Max(0, Math.Min(Me.m_lPages.Count - 1, iPage))

            ' Create new page
            Me.m_page = DirectCast(Activator.CreateInstance(Me.m_lPages(Me.m_iPageActive)), IWizardPage)

            Me.m_page.Init(Me)

            ctrl = DirectCast(Me.m_page, Control)
            ctrl.Dock = DockStyle.Fill
            Me.m_content.Controls.Add(ctrl)
            ctrl.Show()

            Me.m_content.ResumeLayout()

            Me.m_nav.UpdateNavigation()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for the navigation system to poll whether the wizard is
        ''' allowed to navigate backward.
        ''' </summary>
        ''' <returns>
        ''' True if allowed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Friend Function CanNavBack() As Boolean

            Dim page As IWizardPage = Me.ActivePage()

            If (page Is Nothing) Then Return False
            If (page.IsBusy) Then Return False
            If (Me.m_iPageActive = 0) Then Return False

            Return page.AllowNavBack

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for the navigation system to navigate backward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Overridable Sub NavigateBack()
            ' Sanity check
            If (Me.CanNavBack = False) Then Return
            ' Navigate back
            Me.SwitchPage(Me.m_iPageActive - 1)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for the navigation system to poll whether the wizard is
        ''' allowed to navigate forward.
        ''' </summary>
        ''' <returns>
        ''' True if allowed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Friend Function CanNavForward() As Boolean

            Dim page As IWizardPage = Me.ActivePage()

            If (page Is Nothing) Then Return False
            If (page.IsBusy) Then Return False
            If (Me.m_iPageActive >= Me.m_lPages.Count - 1) Then Return False

            Return page.AllowNavForward

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for the navigation system to navigate forward.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Overridable Sub NavigateNext()
            ' Sanity check
            If (Me.CanNavForward = False) Then Return
            ' Navigate back
            Me.SwitchPage(Me.m_iPageActive + 1)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for the navigation system to poll whether the wizard is
        ''' allowed to close.
        ''' </summary>
        ''' <returns>
        ''' True if allowed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Friend Function CanClose() As Boolean

            Dim page As IWizardPage = Me.ActivePage()

            If (page Is Nothing) Then Return False
            Return (page.IsBusy = False)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for the navigation system to poll whether the wizard is
        ''' allowed to finish after all steps completed succesfully.
        ''' </summary>
        ''' <returns>
        ''' True if allowed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Friend Function CanFinish() As Boolean

            Dim page As IWizardPage = Me.ActivePage()

            If (page Is Nothing) Then Return False
            If (page.IsBusy) Then Return False

            Return (Me.m_iPageActive >= Me.m_lPages.Count - 1)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback for the navigation system to close the wizard.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Friend Overridable Sub Close(ByVal result As DialogResult)
            Me.m_parent.DialogResult = result
            Me.m_parent.Close()
        End Sub

#End Region ' Navigation

    End Class

End Namespace ' Controls.Wizard
