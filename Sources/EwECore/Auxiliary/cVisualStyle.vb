' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports EwEUtils
Imports EwEUtils.UserInterface
Imports Newtonsoft.Json

#End Region ' Imports 

Namespace Auxiliary


    Public Class VisualStyleDto
        Public Property foreColor As String          ' "#RRGGBBAA"
        Public Property backColor As String
        Public Property hatch As VisualHatchStyle
        Public Property fontName As String
        Public Property fontSize As Single
        Public Property fontStyle As VisualFontStyle
        Public Property imageBase64 As String        ' PNG as base64 or Nothing
        Public Property colorRampId As Integer
        Public Property colorRampBreaks As Double()
        Public Property colorRampColors As String()  ' each "#RRGGBBAA"
        Public Property colorRampName As String
    End Class

    ' "#RRGGBBAA" <-> System.Drawing.Color
    Public Class ColorHexJsonConverter
        Inherits JsonConverter(Of System.Drawing.Color)

        Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As System.Drawing.Color, hasExistingValue As Boolean, serializer As JsonSerializer) As System.Drawing.Color
            Dim s = TryCast(reader.Value, String)
            If String.IsNullOrEmpty(s) Then Return System.Drawing.Color.Empty
            If s(0) = "#"c Then s = s.Substring(1)
            Dim r = Convert.ToByte(s.Substring(0, 2), 16)
            Dim g = Convert.ToByte(s.Substring(2, 2), 16)
            Dim b = Convert.ToByte(s.Substring(4, 2), 16)
            Dim a As Byte = If(s.Length >= 8, Convert.ToByte(s.Substring(6, 2), 16), CByte(255))
            Return System.Drawing.Color.FromArgb(a, r, g, b)
        End Function

        Public Overrides Sub WriteJson(writer As JsonWriter, value As System.Drawing.Color, serializer As JsonSerializer)
            Dim hex = $"#{value.R:X2}{value.G:X2}{value.B:X2}{value.A:X2}"
            writer.WriteValue(hex)
        End Sub
    End Class

    <Serializable()>
    Public NotInheritable Class cVisualStyle

#Region " Private vars "

        Private m_hatchStyle As VisualHatchStyle = VisualHatchStyle.DiagonalCross
        Private m_clrFore As VisualColor = VisualColor.FromHex("#00000000")
        Private m_clrBack As VisualColor = VisualColor.FromHex("#000000FF")
        Private m_img As Image = Nothing
        Private m_strFontName As String = "Arial"
        Private m_sFontSize As Single = 8.0!
        Private m_fontstyle As VisualFontStyle = VisualFontStyle.Regular
        ''' <summary>To identify stock gradients</summary>
        Private m_gradientID As Integer = cCore.NULL_VALUE
        Private m_gradientColors As VisualColor() = Nothing
        Private m_gradientBreaks As Double() = Nothing
        Private m_gradientName As String = ""
        <NonSerialized()>
        Private m_container As cAuxiliaryData = Nothing

#End Region ' Private vars

        ' Windows migration, won;t work on .NET Core
        Public Shared ReadOnly FixedImageFormat As ImageFormat = ImageFormat.Png

        Private Shared Function ColorToHex(c As System.Drawing.Color) As String
            Return $"#{c.R:X2}{c.G:X2}{c.B:X2}{c.A:X2}"
        End Function

        Private Shared Function HexToColor(s As String) As VisualColor
            If String.IsNullOrEmpty(s) Then Return VisualColor.FromArgb(&HFFF0F0F0)
            If s(0) = "#"c Then s = s.Substring(1)
            Dim r = Convert.ToByte(s.Substring(0, 2), 16)
            Dim g = Convert.ToByte(s.Substring(2, 2), 16)
            Dim b = Convert.ToByte(s.Substring(4, 2), 16)
            Dim a As Byte = 255
            If s.Length >= 8 Then a = Convert.ToByte(s.Substring(6, 2), 16)
            Return VisualColor.FromArgb(a, r, g, b)
        End Function

        Public Shared Function ToDto(vs As cVisualStyle) As VisualStyleDto
            Dim dto As New VisualStyleDto With {
                .foreColor = vs.ForeColour.ToString(),
                .backColor = vs.BackColour.ToString(),
                .hatch = vs.HatchStyle,
                .fontName = vs.FontName,
                .fontSize = vs.FontSize,
                .fontStyle = vs.FontStyle,
                .imageBase64 = vs.ImageString,                           ' already PNG→base64
                .colorRampId = vs.ColorRampID,
                .colorRampBreaks = vs.ColorRampBreaks,
                .colorRampColors = If(vs.ColorRampColors Is Nothing, Nothing,
                                      vs.ColorRampColors.Select(Function(c) c.ToString()).ToArray()),
                .colorRampName = vs.ColorRampName
            }
            Return dto
        End Function

        ' Apply DTO back to runtime object
        Public Shared Sub ApplyDto(vs As cVisualStyle, dto As VisualStyleDto)
            If dto Is Nothing Then Return
            vs.ForeColour = HexToColor(dto.foreColor)
            vs.BackColour = HexToColor(dto.backColor)
            vs.HatchStyle = dto.hatch
            vs.FontName = dto.fontName
            vs.FontSize = dto.fontSize
            vs.FontStyle = dto.fontStyle
            vs.ImageString = dto.imageBase64
            vs.ColorRampID = dto.colorRampId
            vs.ColorRampBreaks = dto.colorRampBreaks
            vs.ColorRampColors = If(dto.colorRampColors Is Nothing, Nothing, dto.colorRampColors.Select(Function(s) HexToColor(s)).ToArray())
            vs.ColorRampName = dto.colorRampName
        End Sub

        Public Shared Function SerializeStyle(vs As cVisualStyle) As String
            Dim dto = ToDto(vs)
#If NETFRAMEWORK Then
            Dim json = Newtonsoft.Json.JsonConvert.SerializeObject(dto, Newtonsoft.Json.Formatting.None)
#Else
    Dim json = System.Text.Json.JsonSerializer.Serialize(dto)
#End If
            Dim bytes = System.Text.Encoding.UTF8.GetBytes(json)
            Return "v3:" & Convert.ToBase64String(bytes)
        End Function

        Public Shared Function DeserializeStyle(s As String) As cVisualStyle
            If String.IsNullOrEmpty(s) Then Return Nothing

            If s.StartsWith("v3:", StringComparison.Ordinal) Then
                Dim blob = Convert.FromBase64String(s.Substring(3))
                Dim jsonBytes As Byte() = blob
                Dim json = System.Text.Encoding.UTF8.GetString(jsonBytes)
#If NETFRAMEWORK Then
                Dim dto = Newtonsoft.Json.JsonConvert.DeserializeObject(Of VisualStyleDto)(json)
#Else
        Dim dto = System.Text.Json.JsonSerializer.Deserialize(Of VisualStyleDto)(json)
#End If
                Dim vs As New cVisualStyle()
                ApplyDto(vs, dto)
                Return vs
            End If

            ' Legacy fallback → BinaryFormatter (4.8 only)
#If NETFRAMEWORK Then
            Try
                Dim ab = Convert.FromBase64String(s)
                Using ms As New MemoryStream(ab)
                    Dim bf As New Runtime.Serialization.Formatters.Binary.BinaryFormatter() With {
                .AssemblyFormat = Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            }
                    Dim oldVs = TryCast(bf.Deserialize(ms), cVisualStyle)
                    If oldVs Is Nothing Then Return Nothing
                    ' Immediately upgrade to v2 DTO on first read if you like:
                    Return oldVs
                End Using
            Catch
            End Try
#End If

            Return Nothing
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Default constructor.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
        End Sub

        Public Sub New(container As cAuxiliaryData, Optional bUpdate As Boolean = False)
            Me.m_container = container
            container.AllowValidation = bUpdate
            container.VisualStyle = Me
            container.AllowValidation = True
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create a clone of a Visual Style instance.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Function Clone() As cVisualStyle

            Dim vs As New cVisualStyle()
            SyncLock GetType(cVisualStyle)

                vs.ForeColour = Me.ForeColour
                vs.BackColour = Me.BackColour
                vs.HatchStyle = Me.HatchStyle
                vs.FontName = Me.FontName
                vs.FontSize = Me.FontSize
                vs.FontStyle = Me.FontStyle
                If Me.Image IsNot Nothing Then
                    vs.Image = DirectCast(Me.Image.Clone(), Image)
                Else
                    vs.Image = Nothing
                End If
                vs.ColorRampID = Me.ColorRampID
                vs.ColorRampBreaks = Me.ColorRampBreaks
                vs.ColorRampColors = Me.ColorRampColors
                vs.ColorRampName = Me.ColorRampName

            End SyncLock

            Return vs

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper class, stores custom visualization information for data entities.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <Flags>
        Public Enum eVisualStyleTypes As Integer
            NotSet = 0
            ForeColor = 1
            BackColor = 2
            Hatch = 4
            Image = 8
            Font = 16
            Gradient = 32
        End Enum

        Public Sub Read(vs As cVisualStyle)
            If (vs Is Nothing) Then Return
            Me.ForeColour = vs.ForeColour
            Me.BackColour = vs.BackColour
            Me.HatchStyle = vs.HatchStyle
            Me.FontName = vs.FontName
            Me.FontStyle = vs.FontStyle
            If vs.Image IsNot Nothing Then
                Me.Image = DirectCast(vs.Image.Clone(), Image)
            Else
                Me.Image = Nothing
            End If
            Me.ColorRampID = vs.ColorRampID
            Me.ColorRampBreaks = vs.ColorRampBreaks
            Me.ColorRampColors = vs.ColorRampColors
            Me.ColorRampName = vs.ColorRampName
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="Color">foreground colour</see> for a visual style, if any.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ForeColour() As VisualColor
            Get
                Return Me.m_clrFore
            End Get
            Set(value As VisualColor)
                If (value <> Me.m_clrFore) Then
                    Me.m_clrFore = value
                    Me.Update()
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="Color">background colour</see> for a visual style, if any.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property BackColour() As VisualColor
            Get
                Return Me.m_clrBack
            End Get
            Set(value As VisualColor)
                If (value <> Me.m_clrBack) Then
                    Me.m_clrBack = value
                    Me.Update()
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the image for a visual style, if any.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        <JsonIgnore>
        Public Property Image() As Image
            Get
                Return Me.m_img
            End Get
            Set(value As Image)
                If Not Equals(value, Me.m_img) Then
                    Me.m_img = value
                    Me.Update()
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Serialize an image to and from a Hex string.
        ''' </summary>
        ''' <returns></returns>
        ''' -----------------------------------------------------------------------
        Public Property ImageString As String
            Get
                If Me.Image IsNot Nothing Then
                    Using ms As New MemoryStream()
                        Me.Image.Save(ms, cVisualStyle.FixedImageFormat) ' PNG
                        Return Convert.ToBase64String(ms.ToArray())
                    End Using
                End If
                Return Nothing
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    Me.Image = Nothing
                    Return
                End If
                Dim imageData As Byte() = Convert.FromBase64String(value)
                Using ms As New MemoryStream(imageData)
                    Using imgTemp As New Bitmap(ms)
                        ' Clone to detach from stream; also ensures we hold a real GDI+ image
                        Me.Image = New Bitmap(imgTemp)
                    End Using
                End Using
            End Set
        End Property


        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="HatchStyle">hatch style</see> for a visual style, if any.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property HatchStyle() As VisualHatchStyle
            Get
                Return Me.m_hatchStyle
            End Get
            Set(value As VisualHatchStyle)
                If (value <> Me.m_hatchStyle) Then
                    Me.m_hatchStyle = value
                    Me.Update()
                End If
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
            Set(value As String)
                If (value <> Me.m_strFontName) Then
                    Me.m_strFontName = value
                    Me.Update()
                End If
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
            Set(value As Single)
                If (value <> Me.m_sFontSize) Then
                    Me.m_sFontSize = value
                    Me.Update()
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="Font.Style">font style</see> for a visual style, if any.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property FontStyle() As VisualFontStyle
            Get
                Return Me.m_fontstyle
            End Get
            Set(value As VisualFontStyle)
                If (value <> Me.m_fontstyle) Then
                    Me.m_fontstyle = value
                    Me.Update()
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the break values for a gradient.
        ''' </summary>
        ''' <remarks>
        ''' The number of gradient breaks should match the number of <see cref="ColorRampColors">
        ''' gradient colours</see>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Property ColorRampBreaks As Double()
            Get
                Return Me.m_gradientBreaks
            End Get
            Set(value As Double())
                Me.m_gradientBreaks = value
                Me.Update()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the color values for a gradient.
        ''' </summary>
        ''' <remarks>
        ''' The number of gradient colours should match the number of <see cref="ColorRampBreaks">
        ''' gradient breaks</see>.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Property ColorRampColors As VisualColor()
            Get
                Return Me.m_gradientColors
            End Get
            Set(value As VisualColor())
                Me.m_gradientColors = value
                Me.Update()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the EwE stock gradient ID, where 0 is the standard EwE gradient.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ColorRampID As Integer
            Get
                Return Me.m_gradientID
            End Get
            Set(value As Integer)
                Me.m_gradientID = value
                Me.Update()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the EwE gradient name.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ColorRampName As String
            Get
                Return Me.m_gradientName
            End Get
            Set(value As String)
                Me.m_gradientName = value
                Me.Update()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' States whether a style equals another.
        ''' </summary>
        ''' <param name="obj">The visual style to compare to.</param>
        ''' -----------------------------------------------------------------------
        Public Overrides Function Equals(obj As Object) As Boolean
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
            If Me.ColorRampID <> vs.ColorRampID Then Return False
            If Me.ColorRampName <> vs.ColorRampName Then Return False
            If Me.ColorRampColors IsNot Nothing Then
                If Not Me.ColorRampColors.EqualsArray(vs.ColorRampColors) Then Return False
            End If
            If Me.ColorRampBreaks IsNot Nothing Then
                If Not Me.ColorRampBreaks.EqualsArray(vs.ColorRampBreaks) Then Return False
            End If
            Return True
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the Auxillary data that contains this visual style.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Friend Property Container() As cAuxiliaryData
            Get
                Return Me.m_container
            End Get
            Set(value As cAuxiliaryData)
                Me.m_container = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update visual style content change to the core.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub Update()
            If Me.m_container IsNot Nothing Then
                Me.m_container.Update()
            End If
        End Sub

    End Class ' cVisualStyle

End Namespace ' Auxillary
