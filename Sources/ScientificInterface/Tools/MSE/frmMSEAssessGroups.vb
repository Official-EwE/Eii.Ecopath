#Region " Imports "

Option Strict On
Imports ScientificInterface.Ecosim
Imports EwECore.MSE
Imports EwEUtils.Core

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Form implementing the MSE Group CV / Assessment interface.
''' </summary>
''' ===========================================================================
Public Class frmMSEAssessGroups

    Private m_propStartYear As cProperty = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Overrides Property UIContext() As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As cUIContext)
            MyBase.UIContext = value
            Me.m_blocks.UIContext = value
        End Set
    End Property

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        Try

            ' Create and attach datasource
            Dim ds As New cMSEGroupColorBlockDataSource(Me.UIContext)
            Me.m_blocks.Attach(ds, New ucCVBlockSelector)

            ' Track styleguide changes
            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            ' Track MSE start year changes
            Me.m_propStartYear = Me.PropertyManager.GetProperty(Me.UIContext.Core.MSEManager.ModelParameters, eVarNameFlags.MSEStartYear)
            AddHandler Me.m_propStartYear.PropertyChanged, AddressOf OnLastYearChanged

        Catch ex As Exception

        End Try

        ' Show form
        MyBase.OnLoad(e)

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)

        Try

            ' No longer track styleguide changes
            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            ' No longer track MSE start year changes
            RemoveHandler Me.m_propStartYear.PropertyChanged, AddressOf OnLastYearChanged
            ' Release blocks
            Me.m_blocks.Dispose()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & " Exception: " & ex.Message)
        End Try

        MyBase.OnFormClosed(e)

    End Sub

    ''' <summary>
    ''' Style guide changed event handler.
    ''' </summary>
    ''' <param name="ct"></param>
    ''' <remarks></remarks>
    Protected Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)

        ' If colours have changed?
        If (ct And cStyleGuide.eChangeType.Colours) > 0 Then
            ' #Yes: totally refresh the blocks (Invalidate won't cut it; the block needs to update its colour dictionary)
            Me.m_blocks.Refresh()
        End If

    End Sub

    Private Sub OnLastYearChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

        Try
            If (changeFlags And cProperty.eChangeFlags.Value) > 0 Then
                Me.m_blocks.Refresh()
            End If
        Catch ex As Exception
            EwECore.cLog.Write(ex)
        End Try

    End Sub

End Class

