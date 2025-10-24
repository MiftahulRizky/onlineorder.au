Partial Class _Default
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            pageTitle.InnerText = "Sunlight Product Online Order"
            If Session("Company") = "SG" Then
                pageTitle.InnerText = "Sunlight Global Online Order"
            End If
        End If
    End Sub
End Class