Partial Class PrintQuote
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("Reprint") <> "" Then
            ' Dim thisString As String = String.Format("https://onlineorder.au/file/order/preview/{0}", Session("Reprint"))
            Dim thisString As String = String.Format("http://10.0.209.168:8888/file/order/quote/{0}", Session("Reprint"))
            embPrint.Attributes.Add("src", thisString)
        End If
    End Sub
End Class