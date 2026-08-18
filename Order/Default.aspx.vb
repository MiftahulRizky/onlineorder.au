Partial Class Order_Default
    Inherits Page

    Public Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim isAdmin As Boolean = Session("RoleName") = "Administrator"
        Dim isPPIC As Boolean = Session("RoleName") = "PPIC & DE"
        Dim isDE As Boolean = Session("RoleName") = "Data Entry"
        Dim isCS As Boolean = Session("RoleName") = "Customer Service"
        Dim isCustomer As Boolean = Session("RoleName") = "Customer"
        Dim isRepresentative As Boolean = Session("RoleName") = "Representative"
        
        Dim Url = "/order/orderheaders"
        Response.Redirect(Url, False)
    End Sub

End Class






