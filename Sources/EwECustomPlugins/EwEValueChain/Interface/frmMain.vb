#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Database
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmMain

#Region " Vars "

    ''' <summary>
    ''' The pages supported by the value chain.
    ''' </summary>
    Public Enum eValueChainPageTypes As Integer
        NotSet = 0
        Parameters
        Flow
        Defaults
        TableProducers
        TableProcessors
        TableDistributors
        TableWholesellers
        TableRetailer
        TableConsumers
        TableLinks
        TableLandingsLinks
        Run
    End Enum

    Private m_plugin As cPluginPoint = Nothing
    Private m_pageCurrent As eValueChainPageTypes = eValueChainPageTypes.NotSet
    Private m_bInUpdate As Boolean = False

#End Region ' Vars

#Region " Constructor "

    Public Sub New(ByVal plugin As cPluginPoint)

        Me.InitializeComponent()

        Me.m_plugin = plugin

        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.UserPaint, True)

        Me.Text = My.Resources.GENERIC_CAPTION
        Me.TabText = My.Resources.GENERIC_CAPTION

    End Sub

#End Region ' Constructor

#Region " Public interfaces "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Switch to a form within the value chain plug-in with a given name.
    ''' </summary>
    ''' <param name="page">Indicator of the page to show.</param>
    ''' -----------------------------------------------------------------------
    Public Sub ShowForm(ByVal page As eValueChainPageTypes)

        If Me.m_pageCurrent = page Then Return
        If Me.m_bInUpdate Then Return

        Me.m_bInUpdate = True
        Me.m_pageCurrent = page

        Select Case Me.m_pageCurrent
            Case eValueChainPageTypes.Parameters
                Me.ShowForm(New ucParameters(Me.m_plugin.Data, Me.m_plugin.Context))
            Case eValueChainPageTypes.TableProducers
                Dim grid As New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Producer)
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.TableProcessors
                Dim grid As New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Processing)
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.TableDistributors
                Dim grid As New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Distribution)
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.TableWholesellers
                Dim grid As New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Wholesaler)
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.TableRetailer
                Dim grid As New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Retailer)
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.TableConsumers
                Dim grid As New ucUnitGrid(Me.m_plugin.Context, Me.m_plugin.Data, cUnitFactory.eUnitType.Consumer)
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.Flow
                Me.ShowForm(New ucEditFlow(Me.m_plugin.Context, Me.m_plugin.Data, Me.m_plugin.Data.FlowDiagram(0)))
            Case eValueChainPageTypes.Defaults
                Me.ShowForm(New ucDefaults(Me.m_plugin.Context, Me.m_plugin.Data))
            Case eValueChainPageTypes.TableLinks
                Dim grid As New ucLinkGrid(Me.m_plugin.Context, Me.m_plugin.Data, GetType(cLink))
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.TableLandingsLinks
                Dim grid As New ucLinkGrid(Me.m_plugin.Context, Me.m_plugin.Data, GetType(cLinkLandings))
                Dim view As New ucGridView(grid)
                Me.ShowForm(view)
            Case eValueChainPageTypes.Run
                Me.ShowForm(New ucResults(Me.m_plugin.Context, Me.m_plugin.Data, Me.m_plugin.Model, Me.m_plugin.Results))
            Case Else
                Debug.Assert(False)
        End Select

        Me.m_bInUpdate = False

    End Sub

#End Region ' Public interfaces

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Translate pageless node names to valid pages.
    ''' </summary>
    ''' <param name="strFormName"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ResolveFormName(ByVal strFormName As String) As String
        Select Case strFormName
            Case "" : Return "ndParameters"
            Case "ndTables" : Return "ndProducer"
        End Select
        Return strFormName
    End Function

    Private Sub ShowForm(ByVal f As Control)

        Dim ctrl As Control = Nothing

        Me.SuspendLayout()

        Try
            If TypeOf f Is IUIElement Then
                DirectCast(f, IUIElement).UIContext = Me.m_plugin.Context
            End If

            f.Dock = DockStyle.Fill
            While Me.Controls.Count > 0
                ctrl = Me.Controls(0)
                Me.Controls.Remove(ctrl)
                ctrl.Dispose()
            End While

            Me.Controls.Add(f)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Me.ResumeLayout()

    End Sub

#End Region ' Event handlers

End Class