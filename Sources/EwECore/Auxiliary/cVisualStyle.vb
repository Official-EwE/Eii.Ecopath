#Region " Imports  "

Option Strict On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Text
Imports System.Runtime.Serialization.Formatters.Binary

#End Region ' Imports 

<Serializable()> _
Public Class cVisualStyle

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, stores custom visualization information for data entities.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eVisualStyleTypes As Integer
        NotSet = 0
        ForeColor = 1
        BackColor = 2
        Hatch = 4
        Image = 8
        Font = 16
        Gradient = 32
    End Enum

    Private m_strID As String = ""
    Private m_hatchStyle As HatchStyle = Drawing2D.HatchStyle.DiagonalCross
    Private m_clrFore As Color = Color.Black
    Private m_clrBack As Color = Color.Transparent
    Private m_img As Image = Nothing
    Private m_strFontName As String = "Arial"
    Private m_sFontSize As Single = 8.0!
    Private m_eFontStyle As FontStyle = FontStyle.Regular

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Default constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a clone of a Visual Style instance.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function Clone() As cVisualStyle

        Dim vs As New cVisualStyle()

        ' DO NOT CLONE ID!

        vs.ForeColour = Me.ForeColour
        vs.BackColour = Me.BackColour
        vs.HatchStyle = Me.HatchStyle
        vs.FontName = Me.FontName
        vs.FontSize = Me.FontSize
        vs.FontStyle = Me.FontStyle
        If Not Object.ReferenceEquals(Me.Image, Nothing) Then
            vs.Image = DirectCast(Me.Image.Clone(), Image)
        Else
            vs.Image = Nothing
        End If
        Return vs

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the unique ID for a visual style.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Property ID() As String
        Get
            Return Me.m_strID
        End Get
        Set(ByVal value As String)
            Me.m_strID = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="Color">foreground colour</see> for a visual style, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ForeColour() As Color
        Get
            Return Me.m_clrFore
        End Get
        Set(ByVal value As Color)
            Me.m_clrFore = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="Color">background colour</see> for a visual style, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property BackColour() As Color
        Get
            Return Me.m_clrBack
        End Get
        Set(ByVal value As Color)
            Me.m_clrBack = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the image for a visual style, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Image() As Image
        Get
            Return Me.m_img
        End Get
        Set(ByVal value As Image)
            Me.m_img = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="HatchStyle">hatch style</see> for a visual style, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property HatchStyle() As HatchStyle
        Get
            Return Me.m_hatchStyle
        End Get
        Set(ByVal value As HatchStyle)
            Me.m_hatchStyle = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="Font.Name">font name</see> for a visual style, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FontName() As String
        Get
            Return Me.m_strFontName
        End Get
        Set(ByVal value As String)
            Me.m_strFontName = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="Font.Size">font size</see> for a visual style, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FontSize() As Single
        Get
            Return Me.m_sFontSize
        End Get
        Set(ByVal value As Single)
            Me.m_sFontSize = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="Font.Style">font style</see> for a visual style, if any.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property FontStyle() As FontStyle
        Get
            Return Me.m_eFontStyle
        End Get
        Set(ByVal value As FontStyle)
            Me.m_eFontStyle = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether a style equals another.
    ''' </summary>
    ''' <param name="obj">The visual style to compare to.</param>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If Not (TypeOf obj Is cVisualStyle) Then Return False

        Dim vs As cVisualStyle = DirectCast(obj, cVisualStyle)
        If Me.ForeColour <> vs.ForeColour Then Return False
        If Me.BackColour <> vs.BackColour Then Return False
        If Me.HatchStyle <> vs.HatchStyle Then Return False
        If String.Compare(Me.FontName, vs.FontName, True) <> 0 Then Return False
        If Me.FontSize <> vs.FontSize Then Return False
        If Me.FontStyle <> vs.FontStyle Then Return False
        If Me.Image IsNot Nothing Or vs.Image IsNot Nothing Then
            If Me.Image Is Nothing Then Return False
            If vs.Image Is Nothing Then Return False
            Return Me.Image.Equals(vs.Image)
        End If
        Return True

    End Function

End Class ' cVisualStyle

''' ===========================================================================
''' <summary>
''' Helper class for serializing a visual style to text.
''' </summary>
''' ===========================================================================
Friend Class cVisualStyleReader

    Public Shared Function StyleToString(ByVal vs As cVisualStyle) As String

        Dim strResult As String = String.Empty
        Dim bf As New BinaryFormatter()
        Dim ms As New MemoryStream()

        ' Write object to mem stream
        bf.Serialize(ms, vs)
        strResult = System.Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.None)

        ms.Close()
        ms = Nothing

        Return strResult

    End Function

    Public Shared Function StringToStyle(ByVal str As String) As cVisualStyle

        Dim vsResult As cVisualStyle = Nothing
        Dim bf As New BinaryFormatter()
        Dim ms As MemoryStream = Nothing
        Dim ab As Byte() = Nothing

        If String.IsNullOrEmpty(str) Then Return vsResult

        Try
            ab = System.Convert.FromBase64String(str)
            ms = New MemoryStream(ab)
            vsResult = CType(bf.Deserialize(ms), cVisualStyle)
        Catch ex As Exception

        End Try
        Return vsResult

    End Function
End Class