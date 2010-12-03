
#Region " Imports "

Option Strict On
Imports ScientificInterface.Ecosim
Imports EwECore.MSE

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Form class for assessing MSE Fleet CV values.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmMSEAssessFleets

    Private m_fpStartYear As cIntegerProperty

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
        Me.Grid = Me.m_grid
    End Sub

    Public Overrides Property UIContext() As cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(ByVal value As cUIContext)
            MyBase.UIContext = value
            Me.m_grid.UIContext = value
            Me.m_blocks.UIContext = value
        End Set
    End Property

    Protected Overrides ReadOnly Property ToolStrip() As System.Windows.Forms.ToolStrip
        Get
            Return Me.m_tsMain
        End Get
    End Property

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Dim ds As New cMSEFishingColorBlockDataSource(Me.UIContext)
        'load the datasource and the block selector into the ucPolicyColorBlocks
        Me.m_blocks.Attach(ds, New ucCVBlockSelector)

        Try
            Dim pm As cPropertyManager = Me.PropertyManager
            Me.m_fpStartYear = DirectCast(pm.GetProperty(Me.UIContext.Core.MSEManager.ModelParameters, EwEUtils.Core.eVarNameFlags.MSEStartYear), cIntegerProperty)

            ' Track styleguide changes
            AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

            AddHandler Me.m_fpStartYear.PropertyChanged, AddressOf OnLastYearChanged

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".OnLoad() Failed to add handlers!")
        End Try

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        'Calling MyBase.OnFormClosed(e) before removing the handlers is setting Me.StyleGuide to nothing
        'then the handler can not be removed
        Try
            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            RemoveHandler Me.m_fpStartYear.PropertyChanged, AddressOf OnLastYearChanged
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & " Exception: " & ex.Message)
        End Try

        Try
            MyBase.OnFormClosed(e)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & " Exception: " & ex.Message)
        End Try
    End Sub

    Protected Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)

        If (ct And cStyleGuide.eChangeType.Colours) > 0 Then
            Me.m_blocks.Refresh()
        End If

    End Sub

    Private Sub OnLastYearChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags)

        Try
            'update the controls
            Me.m_blocks.UpdateControls()
            'redraw the updated data
            Me.m_blocks.Refresh()
        Catch ex As Exception
            EwECore.cLog.Write(ex)
        End Try

    End Sub

End Class

