Partial Class Setting_Log_Customer
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
                search = "WHERE Log_Customers.Id LIKE '%" + SearchText.Trim() + "%' OR Log_Customers.ActionBy LIKE '%" + SearchText.Trim() + "%' OR Log_Customers.Description LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.UserName LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.Id LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.CustomerId LIKE '%" + SearchText.Trim() + "%' OR CustomerLogins.FullName LIKE '%" + SearchText.Trim() + "%'"
            End If

            Dim stringQuery As String = String.Format("SELECT Log_Customers.*, CustomerLogins.FullName AS ActionName FROM Log_Customers LEFT JOIN CustomerLogins ON Log_Customers.ActionBy = CustomerLogins.Id {0} ORDER BY Log_Customers.ActionDate DESC", search)

            gvList.DataSource = logCfg.GetListData(stringQuery)
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Protected Function TeksName(Id As String, Type As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisString = String.Empty
            If Type = "Customers" Then
                thisString = "SELECT Name FROM Customers WHERE Id='" + Id + "'"
            End If
            If Type = "CustomerContacts" Then
                thisString = "SELECT Name FROM CustomerContacts WHERE Id='" + Id + "'"
            End If
            If Type = "CustomerAddress" Then
                thisString = "SELECT Name FROM CustomerAddress WHERE Id='" + Id + "'"
            End If
            If Type = "CustomerLogins" Then
                thisString = "SELECT Name FROM CustomerLogins WHERE Id='" + Id + "'"
            End If
            If Type = "CustomerDiscounts" Then
                thisString = "SELECT Name FROM CustomerDiscounts WHERE Id='" + Id + "'"
            End If
            If Type = "CustomerGroups" Then
                thisString = "SELECT Name FROM CustomerGroups WHERE Id='" + Id + "'"
            End If
            If Type = "CUstomerPriceGroups" Then
                thisString = "SELECT Name FROM CUstomerPriceGroups WHERE Id='" + Id + "'"
            End If
            If Type = "CustomerProductAccess" Then
                thisString = "SELECT Name FROM CustomerProductAccess WHERE Id='" + Id + "'"
            End If
            If Type = "CustomerQuotes" Then
                thisString = "SELECT Name FROM CustomerQuotes WHERE Id='" + Id + "'"
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
