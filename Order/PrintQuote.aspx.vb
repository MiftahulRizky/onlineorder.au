Partial Class PrintQuote
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Reprint") <> "" Then
            Dim request As HttpRequest = HttpContext.Current.Request
            Dim baseUrl As String = request.Url.Scheme & "://" & request.Url.Authority & request.ApplicationPath.TrimEnd("/"c)
            Dim thisString As String = String.Format("{0}/file/order/quote/{1}", baseUrl, Session("Reprint"))
            IF Session("KeyReprint") = "Origin" Then
                thisString = String.Format("{0}/file/order/quote/origin/{1}", baseUrl, Session("Reprint"))
            End If
            embPrint.Attributes.Add("src", thisString)
        End If
    End Sub
End Class