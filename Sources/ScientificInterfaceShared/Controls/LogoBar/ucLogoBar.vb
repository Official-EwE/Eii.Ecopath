' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

''' <summary>
''' Simple control to display a bar of logos.
''' Future improvements:
''' - Look up logos and URLs from ecopath.org, in case they changed
''' </summary>
Public Class ucLogoBar
    Implements IUIElement

#Region " Private classes "

    Private Class cLogoItem
        Public Sub New(name As String, image As Image, url As String)
            Me.Name = name
            Me.Image = image
            Me.URL = url
            Me.HasHyperlink = Not String.IsNullOrWhiteSpace(url)
        End Sub

        Public ReadOnly Property Name As String
        Public ReadOnly Property Image As Image
        Public ReadOnly Property URL As String
        Public ReadOnly Property HasHyperlink As Boolean
        Public Property PlotRectangle As Rectangle
    End Class

#End Region ' Private classes

#Region " Private vars "

    ''' <summary>The logos.</summary>
    Private m_logos As New List(Of cLogoItem)
    ''' <summary>Flag that states if logo rectangles need to be recalculated.</summary>
    Private m_bCalculated As Boolean = False

#End Region ' Private vars

#Region " Construction / destruction "

    Public Sub New()
        Me.InitializeComponent()
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
    End Sub

#End Region ' Construction / destruction

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set <see cref="cUIContext"/>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UIContext As cUIContext Implements IUIElement.UIContext

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set margins around logos, in pixels.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property LogoMargin As Integer = 10

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set spacing between logos, in pixels.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property LogoSpacing As Integer = 10

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a logo to be rendered. Logos are only added to the end. Sorry.
    ''' </summary>
    ''' <param name="name">Name of the logo to add.</param>
    ''' <param name="img">Logo image</param>
    ''' <param name="url">Logo hyperlink, if any.</param>
    ''' -----------------------------------------------------------------------
    Public Sub AddLogo(name As String, img As Image, Optional url As String = "")
        Me.RemoveLogo(name)
        Me.m_logos.Add(New cLogoItem(name, img, url))
        ' Invalidate logo rectangles 
        Me.m_bCalculated = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a logo.
    ''' </summary>
    ''' <param name="name">Name of the logo to remove.</param>
    ''' -----------------------------------------------------------------------
    Public Sub RemoveLogo(name As String)
        Dim iFound As Integer = -1
        For i As Integer = 0 To Me.m_logos.Count - 1
            If (String.Compare(Me.m_logos(i).Name, name, True) = 0) Then
                iFound = i
            End If
        Next
        If (iFound >= 0) Then Me.m_logos.RemoveAt(iFound)
        ' Invalidate logo rectangles 
        Me.m_bCalculated = False
    End Sub

#End Region ' Public access

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        ' Invalidate logo rectangles 
        Me.m_bCalculated = False
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        If (Me.UIContext Is Nothing) Then Return

        If Not Me.m_bCalculated Then
            Me.CalculateLogoRects()
        End If

        ' Erase background
        e.Graphics.FillRectangle(Brushes.White, Me.ClientRectangle)
        ' Draw the logo images
        For i As Integer = 0 To Me.m_logos.Count - 1
            Dim l As cLogoItem = Me.m_logos(i)
            e.Graphics.DrawImage(l.Image, l.PlotRectangle)
        Next

    End Sub

    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        ' NOP
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        ' Invalidate logo rectangles 
        Me.m_bCalculated = False
        MyBase.OnResize(e)
    End Sub

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)

        If (Me.UIContext Is Nothing) Then Return

        For i As Integer = 0 To Me.m_logos.Count - 1
            Dim l As cLogoItem = Me.m_logos(i)
            If l.PlotRectangle.Contains(e.Location) And (l.HasHyperlink) Then
                Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
                Dim cmd As cBrowserCommand = CType(cmdh.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
                If (cmd IsNot Nothing) Then cmd.Invoke(l.URL)
            End If
        Next

    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

        If (Me.UIContext Is Nothing) Then Return

        Dim bHit As Boolean = True
        For i As Integer = 0 To Me.m_logos.Count - 1
            Dim l As cLogoItem = Me.m_logos(i)
            If l.PlotRectangle.Contains(e.Location) And (l.HasHyperlink) Then
                bHit = True
            End If
        Next
        Me.Cursor = If(bHit, Cursors.Hand, Cursors.Default)

    End Sub

#End Region ' Overrides

#Region " Internals "

    ''' <summary>
    ''' Somewhat convoluted logic to calculate logo areas.
    ''' </summary>
    Private Sub CalculateLogoRects()

        Dim rcControl As Rectangle = Me.ClientRectangle
        ' Horizontal logo space is control width minus (2 margins + spacing for all but one logo)
        Dim Xspace As Integer = rcControl.Width - 2 * Me.LogoMargin - Math.Max(0, Me.m_logos.Count - 1) * Me.LogoSpacing
        ' Vertical logo space is control height minus 2 margins
        Dim Yspace As Integer = rcControl.Height - 2 * Me.LogoMargin
        ' To do the calculations with
        Dim scale(Me.m_logos.Count) As Single

        ' First, assess the logos 
        Dim Xwidth As Integer = 0
        For i As Integer = 0 To Me.m_logos.Count - 1
            Dim l As cLogoItem = Me.m_logos(i)
            ' Calculate base scalar based on Yspace/height ratio
            scale(i) = CSng(Yspace / l.Image.Height)
            ' Tally what this scale would mean for total horizontal logo space
            Xwidth += CInt(l.Image.Width * scale(i))
        Next
        ' Now calculate if total Xwidth needs scaling to fit in Xspace
        Dim xfit As Single = CSng(Xspace / Math.Max(1, Xwidth))

        ' Start calculating the logo rectangles
        Dim xpos As Integer = Me.LogoMargin
        Dim yPos As Integer = Me.LogoMargin

        For i As Integer = 0 To Me.m_logos.Count - 1
            Dim l As cLogoItem = Me.m_logos(i)

            ' Logo allowed width
            Dim width As Integer = CInt(l.Image.Width * scale(i) * xfit)
            ' Logo render width, centered within width
            Dim plotwidth As Integer = CInt(l.Image.Width * scale(i) * Math.Min(1, xfit))

            ' Logo allowed height (which is Yspace)
            Dim height As Integer = Yspace
            ' Logo render height, centered within height
            Dim plotheight As Integer = CInt(l.Image.Height * scale(i) * Math.Min(1, xfit))

            ' Finally, determine the rectangle for actually drawing the logo image
            l.PlotRectangle = New Rectangle(xpos + CInt((width - plotwidth) / 2), yPos + CInt((height - plotheight) / 2), plotwidth, plotheight)

            ' Get ready for the next logo
            xpos += Me.LogoSpacing + width
        Next

        Me.m_bCalculated = True

    End Sub

#End Region ' Internals

End Class
