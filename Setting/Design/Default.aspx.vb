Partial Class Setting_Design_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
            txtSearch.Text = Session("designSearch")

            Call BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub btnAdd_Click(sender As Object, e As EventArgs)
        Session("designSearch") = txtSearch.Text
        Response.Redirect("~/setting/design/add", False)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        Call BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Session("designSearch") = txtSearch.Text
            Session("designDetail") = UCase(row.Cells(1).Text)
            Response.Redirect("~/setting/design/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblId.Text = txtIdDelete.Text
            sdsPage.Delete()


            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub linkActive_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim active As String = row.Cells(7).Text

            Dim newActive As Integer = 0
            If active = "False" Then : newActive = 1 : End If

            lblId.Text = UCase(row.Cells(1).Text)
            lblActive.Text = newActive

            sdsPage.Update()

            Call BindData(txtSearch.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(SearchText As String)
        Session("designDetail") = "" : Session("designSearch") = ""
        Try
            Dim bySearch As String = String.Empty
            If Not SearchText = "" Then
                bySearch = String.Format(" WHERE Id LIKE '%{0}%' OR Name LIKE '%{0}%' OR Company LIKE '%{0}%' OR Page LIKE '%{0}%' OR Description LIKE '%{0}%' ", SearchText)
            End If
            gvList.DataSource = publicCfg.GetListData(String.Format("SELECT * FROM Designs {0} ORDER BY Name ASC", bySearch))
            gvList.DataBind()
            For i As Integer = 0 To gvList.HeaderRow.Cells.Count - 1
                gvList.HeaderRow.Cells(i).Text = If(gvList.HeaderRow.Cells(i).Text = "", "Empty", gvList.HeaderRow.Cells(i).Text)
            Next
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function TextActive(Active As String) As String
        Dim result As String = "<i class='bi bi-arrow-repeat fs-3 me-2 opacity-50'></i>Switch Activated"
        ' Dim result As String = "<i class='fa-solid fa-rotate'></i>"
        ' If Active = "True" Then : Return "<i class='fa-solid fa-rotate'></i>" : End If
        Return result
    End Function

    ' Icon active before text name
    Protected Function IconActive(Active As String, Name As String) As String
        Dim result As String = "<i class='fa-regular text-danger fa-circle-xmark'></i> " + Name
        If Active = "True" Then : Return "<i class='fa-regular text-success fa-circle-check'></i> " + Name : End If
        Return result
    End Function

    Protected Function Descpription(Desc As String) As String
        Dim result As String = "<span class='badge badge-outline text-green'>Production</span>"
        If InStr(Desc, "Development") Then : Return "<span class='badge badge-outline text-red'>Development</span>" : End If
        If InStr(Desc, "Testing") Then : Return "<span class='badge badge-outline text-warning'>Testing</span>" : End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Protected Function CssActive(Active As Boolean) As String
        Dim result As String = "dropdown-item"
        ' Dim result As String = "btn btn-green"
        ' If Active = False Then : result = "btn btn-secondary" : End If
        Return result
    End Function
End Class
