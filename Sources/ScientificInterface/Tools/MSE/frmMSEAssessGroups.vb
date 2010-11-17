#Region " Imports "

Option Strict On
Imports ScientificInterface.Ecosim
Imports EwECore.MSE

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Form implementing the MSE Group CV / Assessment interface.
''' </summary>
''' ===========================================================================
Public Class frmMSEAssessGroups

    Private m_fpStartYear As cIntegerProperty
    'Private m_MSEDataSource As cMSEGroupColorBlockDataSource

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
        MyBase.OnLoad(e)
        ' Attach the datasource and the block selector to the ucPolicyColorBlocks control
        Dim ds As New cMSEGroupColorBlockDataSource(Me.UIContext)
        '  m_MSEDataSource = New cMSEGroupColorBlockDataSource(Me.UIContext)
        Me.m_blocks.Attach(ds, New ucCVBlockSelector)

        Dim pm As cPropertyManager = Me.PropertyManager

        Me.m_fpStartYear = DirectCast(pm.GetProperty(Me.UIContext.Core.MSEManager.ModelParameters, EwEUtils.Core.eVarNameFlags.MSEStartYear), cIntegerProperty)

        ' Track styleguide changes
        AddHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged

        AddHandler Me.m_fpStartYear.PropertyChanged, AddressOf OnLastYearChanged

    End Sub


    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        Try
            MyBase.OnFormClosed(e)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & " Exception: " & ex.Message)
        End Try

        Try
            RemoveHandler Me.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            RemoveHandler Me.m_fpStartYear.PropertyChanged, AddressOf OnLastYearChanged
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

