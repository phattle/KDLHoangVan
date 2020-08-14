Imports System.Drawing
Imports ThoughtWorks.QRCode.Codec

Public Class MyQRCode
    Private _QREncoder As ThoughtWorks.QRCode.Codec.QRCodeEncoder

    'Private _QRImg As Image
    'Public Property QRimg() As Image
    '    Get
    '        Return _QRImg
    '    End Get
    '    Set(ByVal value As Image)
    '        _QRImg = value
    '    End Set
    'End Property
    'Private _QRbitmap As Bitmap

    'Public Property QRbitmap() As Bitmap
    '    Get
    '        Return _QRbitmap
    '    End Get
    '    Set(ByVal value As Bitmap)
    '        _QRbitmap = value
    '    End Set
    'End Property
    Public Sub New()
        _QREncoder = New ThoughtWorks.QRCode.Codec.QRCodeEncoder
        _QREncoder.QRCodeForegroundColor = Color.Black
        _QREncoder.QRCodeBackgroundColor = Color.White
        _QREncoder.QRCodeScale = 2
        _QREncoder.QRCodeVersion = 5
        _QREncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE
        _QREncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H
    End Sub
    Public Sub New(ByVal QRCodeScale As Integer, ByVal QRCodeVersion As Integer, ByVal QRCodeErrorCorrect As Integer)
        _QREncoder = New ThoughtWorks.QRCode.Codec.QRCodeEncoder
        _QREncoder.QRCodeForegroundColor = Color.Black
        _QREncoder.QRCodeBackgroundColor = Color.White
        _QREncoder.QRCodeScale = QRCodeScale
        _QREncoder.QRCodeVersion = QRCodeVersion
        _QREncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE
        _QREncoder.QRCodeErrorCorrect = QRCodeErrorCorrect
    End Sub
    Public Function getBitMapFromString(ByVal inputString As String, ByVal xoay As Boolean) As Bitmap
        Dim outPut As Bitmap
        Try
            outPut = getImageFromString(inputString, xoay)
        Catch ex As Exception
        End Try
        Return outPut '.RotateFlip(RotateFlipType.Rotate180FlipY)
    End Function
    Public Function getImageFromString(ByVal inputString As String, ByVal xoay As Boolean) As Image
        Dim outPut As Image
        Try
            outPut = _QREncoder.Encode(inputString)
            If xoay Then
                If outPut IsNot Nothing Then
                    outPut.RotateFlip(RotateFlipType.Rotate90FlipNone)
                End If
            End If
        Catch ex As Exception
        End Try
        Return outPut
    End Function

    Private mix As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
    Private rnd As New Random
    Public Function chenKyTuTaoLao(ByVal str As String) As String
        Dim kq As String = str
        'Dim rnd As New Random
        Dim len As Integer = mix.Length - 1
        For i As Integer = 0 To 1
            Dim x As Integer = rnd.Next(0, len)
            kq = kq.Insert(i * 9, mix.Substring(x, 1))
        Next
        Return kq
    End Function
End Class
