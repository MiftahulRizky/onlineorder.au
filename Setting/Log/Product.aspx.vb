
Partial Class Setting_Log_Product
    Inherits Page

    Dim logCfg As New LogConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Try
            Dim search As String = String.Empty

            If Not SearchText = "" Then
                search = "WHERE Log_Products.Id LIKE '%" + SearchText.Trim() + "%' OR Log_Products.ActionBy LIKE '%" + SearchText.Trim() + "%' OR Log_Products.Description LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.UserName LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.Id LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.CustomerId LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.FullName LIKE '%" + SearchText.Trim() + "%'"
            End If

            Dim stringQuery As String = String.Format("SELECT Log_Products.*, CustomerLogins.FullName AS ActionName FROM Log_Products LEFT JOIN CustomerLogins ON Log_Products.ActionBy = CustomerLogins.Id {0} ORDER BY Log_Products.ActionDate DESC", search)

            gvList.DataSource = logCfg.GetListData(stringQuery)
            gvList.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Protected Function TeksName(Id As String, Type As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisString = String.Empty
            If Type = "Designs" Then
                thisString = "SELECT Name FROM Designs WHERE Id='" + Id + "'"
            End If
            If Type = "Blinds" Then
                thisString = "SELECT Name FROM Blinds WHERE Id='" + Id + "'"
            End If
            If Type = "Products" Then
                thisString = "SELECT Name FROM Products WHERE Id='" + Id + "'"
            End If
            If Type = "ProductControls" Then
                thisString = "SELECT Name FROM ProductControls WHERE Id='" + Id + "'"
            End If
            If Type = "ProductColours" Then
                thisString = "SELECT Name FROM ProductColours WHERE Id='" + Id + "'"
            End If
            If Type = "ProductTubes" Then
                thisString = "SELECT Name FROM ProductTubes WHERE Id='" + Id + "'"
            End If
            If Type = "Mountings" Then
                thisString = "SELECT Name FROM Mountings WHERE Id='" + Id + "'"
            End If
            If Type = "Bottoms" Then
                thisString = "SELECT Name FROM Bottoms WHERE Id='" + Id + "'"
            End If
            If Type = "Chains" Then
                thisString = "SELECT Name FROM Chains WHERE Id='" + Id + "'"
            End If
            If Type = "Fabrics" Then
                thisString = "SELECT Name FROM Fabrics WHERE Id='" + Id + "'"
            End If
            If Type = "FabricColours" Then
                thisString = "SELECT Name FROM FabricColours WHERE Id='" + Id + "'"
            End If

            If Not String.IsNullOrEmpty(thisString) Then
                result = logCfg.GetItemData(thisString)
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function
End Class
