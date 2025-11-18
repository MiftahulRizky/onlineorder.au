Imports System.Net

Partial Class Order_Preview
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("printPreview") = "" Then
            Response.Redirect("~/order/detail", False)
            Exit Sub
        End If

        Dim FilePath As String = Session("printPreview")
        Dim User As WebClient = New WebClient()
        Dim FileBuffer As Byte() = User.DownloadData(FilePath)

        If FileBuffer IsNot Nothing Then
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-length", FileBuffer.Length.ToString())
            Response.BinaryWrite(FileBuffer)
        End If
    End Sub
End Class
