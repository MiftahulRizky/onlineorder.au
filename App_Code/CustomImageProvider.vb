Imports Microsoft.VisualBasic
Imports iTextSharp.text
Imports iTextSharp.tool.xml.pipeline.html
Imports System.IO
Imports System.Net
Public Class CustomImageProvider
   Implements IImageProvider

    Public Function Retrieve(src As String) As iTextSharp.text.Image Implements IImageProvider.Retrieve
        Try
            ' Jika gambar base64
            If src.StartsWith("data:image") Then
                Dim base64Data As String = src.Substring(src.IndexOf(",") + 1)
                Dim imageBytes As Byte() = Convert.FromBase64String(base64Data)
                Return iTextSharp.text.Image.GetInstance(imageBytes)

            ' Jika gambar dari URL (http/https)
            ElseIf src.StartsWith("http") Then
                Dim request As HttpWebRequest = CType(WebRequest.Create(src), HttpWebRequest)
                request.Timeout = 10000
                Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                    Using stream As Stream = response.GetResponseStream()
                        Return iTextSharp.text.Image.GetInstance(stream)
                    End Using
                End Using

            ' Jika path lokal (misal Server.MapPath)
            ElseIf File.Exists(src) Then
                Return iTextSharp.text.Image.GetInstance(src)
            End If

        Catch ex As Exception
            ' Optional: tulis log kalau mau debug
        End Try

        Return Nothing
    End Function

    Public Sub Store(src As String, img As iTextSharp.text.Image) Implements IImageProvider.Store
        ' Tidak digunakan
    End Sub

    Public Property BaseUri As String Implements IImageProvider.BaseUri
        Get
            Return Nothing
        End Get
        Set(value As String)
        End Set
    End Property

    Public Function GetImageRootPath() As String Implements IImageProvider.GetImageRootPath
        Return String.Empty
    End Function
End Class
